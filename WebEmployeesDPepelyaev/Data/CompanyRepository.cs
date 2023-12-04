using Dapper;
using System.Data;
using WebEmployeesDPepelyaev.Entitys;

namespace WebEmployeesDPepelyaev.Data
{
    public class CompanyRepository : Repository
    {
        private readonly IDbConnection _dbConnection;
        private const string sqlGetAll = "SELECT Id, Name FROM Companies";
        public CompanyRepository(IDbConnection dbConnection) 
        {
            _dbConnection = dbConnection;
        }
        public async Task<IEnumerable<Company>> GetAll()
        {
            return await _dbConnection.QueryAsync<Company>(sqlGetAll);
        }
    }
}
