using System.Text.Json;
using System.Text.Json.Serialization;
using static NorthwindHTMX.FastEndpoints.NamedQueryEndpoint;

public class ExpressionConverter : JsonConverter<IExpression>
{
	public override IExpression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		using (var jsonDoc = JsonDocument.ParseValue(ref reader))
		{
			var root = jsonDoc.RootElement;

			if (!root.TryGetProperty<string>("operator", out var @operator))
			{
				throw new JsonException("JSON does not contain a operator property.");
			}

			switch (@operator.ToUpper())
			{
				case "AND":
				case "OR":
					var logicalExpression = new LogicalExpression
					{
						Operator = root.GetProperty<string>("operator")
					};

					if (root.TryGetProperty("expressions", out var expressions))
					{
						foreach (var expression in expressions.EnumerateArray())
						{
							logicalExpression.Add(JsonSerializer.Deserialize<IExpression>(expression.GetRawText(), options));
						}
					}

					return logicalExpression;
				default:
					return JsonSerializer.Deserialize<ComparisonExpression>(root.GetRawText(), options);
			}
		}
	}

	public override void Write(Utf8JsonWriter writer, IExpression value, JsonSerializerOptions options)
	{
		JsonSerializer.Serialize(writer, value, value.GetType(), options);
	}

	private LogicalExpression DeserializeLogicalExpression(JsonElement element, JsonSerializerOptions options)
	{
		var logicalExpression = new LogicalExpression
		{
			Operator = element.GetProperty<string>("operator")
		};

		if (element.TryGetProperty("expressions", out var expressions))
		{
			foreach (var expression in expressions.EnumerateArray())
			{
				logicalExpression.Add(JsonSerializer.Deserialize<IExpression>(expression.GetRawText(), options));
			}
		}

		return logicalExpression;
	}
}
