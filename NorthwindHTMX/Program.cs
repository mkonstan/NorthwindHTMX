using NorthwindHTMX.Data.SQLite;
using NorthwindHTMX.Templates;
using FastEndpoints;
using FastEndpoints.Swagger;
using NLog;
using NLog.Web;
using System.Text.Json.Serialization;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    // Add services to the container.
    WebApplication
        .CreateBuilder(args)
        .Then(builder =>
        {
            var env = builder.Environment;
            logger.Info($"Application Name: {env.ApplicationName}");
            logger.Info($"Environment Name: {env.EnvironmentName}");
            logger.Info($"ContentRoot Path: {env.ContentRootPath}");
            logger.Info($"WebRoot Path: {env.WebRootPath}");
        })
        .RegisterServices((app, services, configuration, environment) =>
        {
			services.ConfigureHttpJsonOptions(options =>
			{
				options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.SerializerOptions.Converters.Add(new ExpressionConverter());
			});

			services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
			{
				options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new ExpressionConverter());
			});

			services.AddRazorPages();
            services.AddFastEndpoints();

            services.AddResponseCompression();
            services.AddResponseCaching();
            services.AddOptions();
            services.AddSwaggerDocument();
            services.AddSingleton(new NorthwindConnectionFactory(configuration, "NorthwindConnection"));
            services.AddSingleton(new TemplateStore(
                Path.Combine(
                    environment.ContentRootPath,
                    configuration.GetValue<string>("TemplatesDirectory"))));
        })
        .Then(builder =>
        {
            // NLog: Setup NLog for Dependency injection
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();
        })
        .Build()
        .ConfigureRequestPipeline(app =>
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseResponseCompression();

            app.UseFastEndpoints();
            app.UseSwaggerGen();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.MapRazorPages();
        })
        .Run();
}
catch (Exception ex)
{
    //NLog: catch setup errors
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
