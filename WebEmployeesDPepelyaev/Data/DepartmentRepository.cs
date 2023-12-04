using Dapper;
using System.Data;
using WebEmployeesDPepelyaev.Entitys;

namespace WebEmployeesDPepelyaev.Data
{
    public class DepartmentRepository : Repository
    {
        private readonly IDbConnection _dbConnection;
        private const string sqlGetAll = "Select Id, Name, Phone FROM Departments";
        public DepartmentRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<IEnumerable<Department>> GetAll()
        {
            return await _dbConnection.QueryAsync<Department>(sqlGetAll);
        }

    }
}
