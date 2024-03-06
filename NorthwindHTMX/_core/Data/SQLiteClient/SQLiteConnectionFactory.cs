using System.Data;
using System.Data.SQLite;

namespace NorthwindHTMX.Data.SQLite;

public abstract class SQLiteConnectionFactory(string connectionString) : DbConnectionFactory(connectionString)
{
    public override IDbConnection CreateConnection()
    {
        return new SQLiteConnection(ConnectionString);
    }
}
