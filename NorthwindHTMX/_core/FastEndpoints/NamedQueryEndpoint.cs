using NorthwindHTMX.Data;
using NorthwindHTMX.Templates;
using FastEndpoints;
using NLog.Filters;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NorthwindHTMX.FastEndpoints;

public abstract class NamedQueryEndpoint : Endpoint<NamedQueryEndpoint.QueryRequest, object?>
{
    protected NamedQueryEndpoint(string sourceName, string endpointName, DbConnectionFactory connectionFactory, TemplateStore templateStore)
    {
		SourceName = sourceName;
        EndpointName = endpointName;
        ConnectionFactory = connectionFactory;
		TemplateStore = templateStore;
    }

    public override void Configure()
	{
		Get($"/api/{EndpointName}");
		AllowAnonymous();
	}

	public sealed override async Task HandleAsync(QueryRequest request, CancellationToken cancellationToken)
	{
		try
		{
			using (var con = ConnectionFactory.CreateConnection())
			{
				var compiler = CreateCompiler();
				var queryFactory = new QueryFactory(con, compiler);
				queryFactory.Logger = compiled =>
				{
					Logger.LogDebug($"Query: {compiled.Sql}");
				};
				var query = queryFactory.Query(SourceName);

				if (request.Fields is not null && request.Fields.Any())
				{
					query = query.Select(request.Fields.ToArray());
				}

				if (request.Where is not null)
				{
					query = query.ConstructWhere(request.Where);
				}

				if (request.OrderBy is not null && request.OrderBy.Any())
				{
					foreach (var orderBy in request.OrderBy)
					{
						if (orderBy.Direction == QueryRequest.OrderByDirection.Asc)
							query = query.OrderBy(orderBy.Field);
						else
							query = query.OrderByDesc(orderBy.Field);
					}
				}

				if (request.Limit is not null)
				{
					query = query.Limit(request.Limit.Value);
				}

				if (request.Offset is not null)
				{
					query = query.Offset(request.Offset.Value);
				}

				var result = new { result = await query.GetAsync() };
				if(request.Template is not null)
				{
					if(!TemplateStore.TryGetTemplate(request.Template, out var template) || template is null)
					{
						throw new InvalidOperationException($"Template {request.Template} not found");
					}
					await SendStringAsync(template(result));
					return;
                }
                await SendAsync(result);
            }
        }
		catch (Exception ex)
		{
			Logger.LogError(ex, "Error executing query");
			throw;
		}
	}

	protected abstract Compiler CreateCompiler();

    protected string SourceName { get; }

    protected string EndpointName { get; }

    public TemplateStore TemplateStore { get; }

    protected DbConnectionFactory ConnectionFactory { get; }

	public class QueryRequest
	{
		[FromHeader(headerName: "x-htmx-template", isRequired: false)]
		public string? Template { get; set; }

        [QueryParam]
		public IEnumerable<string>? Fields { get; set; }
		[QueryParam]
		public IExpression? Where { get; set; }
		[QueryParam]
		public IEnumerable<OrderByField>? OrderBy { get; set; }
		[QueryParam]
		public int? Limit { get; set; }

		public int? Offset { get; set; }

		public class OrderByField
		{
			public required string Field { get; set; }

			[DefaultValue(OrderByDirection.Asc), BindFrom("dir"), JsonPropertyName("dir")]
			public OrderByDirection? Direction { get; set; } = OrderByDirection.Asc;
		}

		public enum OrderByDirection
		{
			Asc,
			Desc
		}
	}

	public enum ExpressionType
	{
		Comparison,
		Logical
	}

	public interface IExpression
	{
		public ExpressionType Type { get; }
		public string Operator { get; set; }
	}

	public class ComparisonExpression : IExpression
	{
		public required string Field { get; set; }
		public required string Operator { get; set; }
		private object? _value;
		public required object? Value
		{
			get => _value;
			set
			{
				_value = value switch
				{
					JsonElement e => e.GetValue(),
					_ => value,
				};
			}
		}

		public ExpressionType Type => ExpressionType.Comparison;
	}

	public class LogicalExpression : IExpression
	{
		private readonly IList<IExpression> _expressions = new List<IExpression>();

		public IEnumerable<IExpression> Expressions => _expressions;

		public required string Operator { get; set; }

		public ExpressionType Type => ExpressionType.Logical;

		public void Add(IExpression expression)
		{
			_expressions.Add(expression);
		}
	}

}
