using System.Data;

namespace NorthwindHTMX.Data;

public abstract class DbConnectionFactory(string connectionString)
{
    public abstract IDbConnection CreateConnection();
    protected string ConnectionString { get; } = connectionString;
}

