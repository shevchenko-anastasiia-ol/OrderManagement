using System.Data;

namespace MarketplaceDAL.Connection;

public interface IConnectionFactory
{
    IDbConnection CreateConnection();
}