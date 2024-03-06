using HandlebarsDotNet;

namespace NorthwindHTMX.Templates
{
    public class TemplateStore
    {
        private readonly IDictionary<string, Func<HandlebarsTemplate<object, object>>> _templates
            = new Dictionary<string, Func<HandlebarsTemplate<object, object>>>(StringComparer.OrdinalIgnoreCase);

        public TemplateStore(string templateDirectory)
        {
            foreach (var templateFile in Directory.EnumerateFiles(templateDirectory, "*.html", SearchOption.AllDirectories))
            {
                _templates.Add(
                    Path.GetFileNameWithoutExtension(templateFile),
                    () => Handlebars.Compile(File.ReadAllText(templateFile))
                );
            }
        }

        public bool TryGetTemplate(string name, out HandlebarsTemplate<object, object>? template)
        {
            if (_templates.TryGetValue(name, out var tp))
            {
                template = tp();
                return true;
            }
            template = null;
            return false;
        }
    }

    public class TemplateCache
    {
        private readonly IDictionary<string, Lazy<HandlebarsTemplate<object, object>>> _templates
            = new Dictionary<string, Lazy<HandlebarsTemplate<object, object>>>(StringComparer.OrdinalIgnoreCase);
        public TemplateCache(string templateDirectory)
        {
            foreach (var templateFile in Directory.EnumerateFiles(templateDirectory, "*.html", SearchOption.AllDirectories))
            {
                _templates.Add(
                    Path.GetFileNameWithoutExtension(templateFile),
                    new Lazy<HandlebarsTemplate<object, object>>(() => Handlebars.Compile(File.ReadAllText(templateFile)))
                );
            }
        }

        public bool TryGetTemplate(string name, out HandlebarsTemplate<object, object>? template)
        {
            if (_templates.TryGetValue(name, out var tp))
            {
                template = tp.Value;
                return true;
            }
            template = null;
            return false;
        }
    }

}
