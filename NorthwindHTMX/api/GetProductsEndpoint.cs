using NorthwindHTMX.Data.SQLite;
using NorthwindHTMX.FastEndpoints;
using NorthwindHTMX.Templates;
using SqlKata.Compilers;

namespace NorthwindHTMX.api;

public class GetProductsEndpoint(NorthwindConnectionFactory connectionFactory, TemplateStore templateStore)
    : NamedQueryEndpoint("Products", "Products", connectionFactory, templateStore)
{
    protected override Compiler CreateCompiler()
    {
        return new SqliteCompiler();
    }
}
