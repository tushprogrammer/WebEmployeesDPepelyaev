using Dapper;
using System.Data;
using WebEmployeesDPepelyaev.Entitys;

namespace WebEmployeesDPepelyaev.Data
{
    public class PassportTypeRepository : Repository
    {
        private readonly IDbConnection _dbConnection;
        private const string sqlGetAll = "SELECT Id, Type, Description FROM PassportTypes";
        public PassportTypeRepository(IDbConnection dbConnection) 
        {
            _dbConnection = dbConnection;
        }
        public async Task<IEnumerable<PassportType>> GetAll()
        {
            return await _dbConnection.QueryAsync<PassportType>(sqlGetAll);
        }
    }
}
