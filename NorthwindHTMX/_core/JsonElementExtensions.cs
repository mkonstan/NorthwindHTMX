using System.Text.Json;

public static class JsonElementExtensions
{
	private static readonly Dictionary<Type, Func<JsonElement, object?>> TypeConverters = new()
	{
		{ typeof(string), element => element.GetString() },
		{ typeof(int), element => element.GetInt32() },
		{ typeof(int?), element => element.ValueKind == JsonValueKind.Null ? (int?)null : element.GetInt32() },
		{ typeof(long), element => element.GetInt64() },
		{ typeof(long?), element => element.ValueKind == JsonValueKind.Null ? (long?)null : element.GetInt64() },
		{ typeof(bool), element => element.GetBoolean() },
		{ typeof(bool?), element => element.ValueKind == JsonValueKind.Null ? (bool?)null : element.GetBoolean() },
		{ typeof(double), element => element.GetDouble() },
		{ typeof(double?), element => element.ValueKind == JsonValueKind.Null ? (double?)null : element.GetDouble() },
		{ typeof(decimal), element => element.GetDecimal() },
		{ typeof(decimal?), element => element.ValueKind == JsonValueKind.Null ? (decimal?)null : element.GetDecimal() },
		{ typeof(Guid), element => element.GetGuid() },
		{ typeof(Guid?), element => element.ValueKind == JsonValueKind.Null ? (Guid?)null : element.GetGuid() },
		{ typeof(DateTime), element => element.GetDateTime() },
		{ typeof(DateTime?), element => element.ValueKind == JsonValueKind.Null ? (DateTime?)null : element.GetDateTime() },
		{ typeof(DateTimeOffset), element => element.GetDateTimeOffset() },
		{ typeof(DateTimeOffset?), element => element.ValueKind == JsonValueKind.Null ? (DateTimeOffset?)null : element.GetDateTimeOffset() }
	};

	public static T GetProperty<T>(this JsonElement element, string propertyName)
	{
		if (!element.TryGetProperty(propertyName, out var property))
		{
			throw new KeyNotFoundException($"Property '{propertyName}' not found.");
		}

		var type = typeof(T);
		if (TypeConverters.TryGetValue(type, out var converter))
		{
			try
			{
				return (T)converter(property);
			}
			catch (Exception ex)
			{
				throw new InvalidCastException($"Failed to convert property '{propertyName}' to type {typeof(T).Name}.", ex);
			}
		}
		else
		{
			// Attempt to deserialize for unsupported types
			try
			{
				return JsonSerializer.Deserialize<T>(property.GetRawText()) ?? throw new InvalidOperationException($"Deserialization of property '{propertyName}' returned null.");
			}
			catch (JsonException ex)
			{
				throw new InvalidCastException($"Failed to deserialize property '{propertyName}' to type {typeof(T).Name}.", ex);
			}
		}
	}

	public static bool TryGetProperty<T>(this JsonElement element, string propertyName, out T value)
	{
		if (element.TryGetProperty(propertyName, out var property))
		{
			var type = typeof(T);
			if (TypeConverters.TryGetValue(type, out var converter))
			{
				try
				{
					value = (T)converter(property);
					return true;
				}
				catch
				{
					// Conversion failed, handle gracefully
					value = default;
					return false;
				}
			}
			else
			{
				// Attempt to deserialize for unsupported types
				try
				{
					value = JsonSerializer.Deserialize<T>(property.GetRawText());
					return true;
				}
				catch
				{
					// Deserialization failed
					value = default;
					return false;
				}
			}
		}
		value = default;
		return false;
	}

	public static object? GetValue(this JsonElement element)
	{
		switch (element.ValueKind)
		{
			case JsonValueKind.String:
				return element.GetString();
			case JsonValueKind.Number:
				return element.GetDouble();
			case JsonValueKind.True:
			case JsonValueKind.False:
				return element.GetBoolean();
			case JsonValueKind.Undefined:
			case JsonValueKind.Null:
				return null;
			default:
				throw new NotSupportedException($"The Value Type[{element.ValueKind}] of JsonElement is not supported.");
		}
	}
}