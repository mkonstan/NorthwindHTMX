namespace NorthwindHTMX.Data.SQLite;

public class NorthwindConnectionFactory(IConfiguration configuration, string connectionName)
    : SQLiteConnectionFactory(configuration.GetConnectionString(connectionName))
{
}