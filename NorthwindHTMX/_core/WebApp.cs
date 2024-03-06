public static class WebAppHelpers
{
    public static T Then<T>(this T source, Action<T> body) where T : class
    {
        body(source);
        return source;
    }

    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder, Action<IHostApplicationBuilder, IServiceCollection, IConfiguration, IWebHostEnvironment> body)
    {
        body(builder, builder.Services, builder.Configuration, builder.Environment);
        return builder;
    }

    public static WebApplication ConfigureRequestPipeline(this WebApplication app, Action<WebApplication> body)
    {
        body(app);
        return app;
    }
}
