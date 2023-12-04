using Dapper;
using Microsoft.Identity.Client;
using System.Data;
using System.Reflection;
using WebEmployeesDPepelyaev.Entitys;

namespace WebEmployeesDPepelyaev.Data
{
    public class PassportRepository : Repository
    {
        private readonly IDbConnection _dbConnection;
        private const string insertQueryPassport = "INSERT INTO Passports (Type, Number) VALUES (@Type, @Number); " +
                             "SELECT CAST(SCOPE_IDENTITY() as int)";
        private const string sqlUpdate = @"UPDATE Passports 
                SET
                    Type = @Type,
                    Number = @Number
                WHERE Id = @Id";

        public PassportRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<int> Create(Passport newPassport)
        {
            return await _dbConnection.QuerySingleAsync<int>(insertQueryPassport, newPassport);
        }
        public async Task Update(Passport editPassport)
        {
            await _dbConnection.ExecuteAsync(sqlUpdate, editPassport);
        }
    }
}
