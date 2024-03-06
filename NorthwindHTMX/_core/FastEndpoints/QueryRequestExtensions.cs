using SqlKata;
using static NorthwindHTMX.FastEndpoints.NamedQueryEndpoint;

namespace NorthwindHTMX.FastEndpoints;

static class QueryRequestExtensions
{
	public static Query ConstructWhere(this Query query, IExpression expression)
	{
		switch (expression)
		{
			case LogicalExpression logicalExpression:
				return query.ConstructWhere(logicalExpression);
			case ComparisonExpression comparisonExpression:
				return query.ConstructWhere(comparisonExpression);
			default:
				throw new InvalidOperationException($"Unsupported expression type: {expression.GetType()}");
		}
	}

	private static Query ConstructWhere(this Query query, LogicalExpression expression)
	{
		var isAndOperator = expression.Operator.Equals("and", StringComparison.OrdinalIgnoreCase);
		var isOrOperator = expression.Operator.Equals("or", StringComparison.OrdinalIgnoreCase);

		if (!isAndOperator && !isOrOperator)
		{
			throw new InvalidOperationException($"Invalid operator: {expression.Operator}");
		}

		foreach (var element in expression.Expressions)
		{
			query = isAndOperator
				? query.Where(q => q.ConstructWhere(element))
				: query.OrWhere(q => q.ConstructWhere(element));
		}

		return query;
	}

	private static Query ConstructWhere(this Query query, ComparisonExpression expression)
	{
		return query.Where(expression.Field, expression.Operator, expression.Value);
	}
}
