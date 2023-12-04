using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System.Data;
using WebEmployeesDPepelyaev.Entitys;

namespace WebEmployeesDPepelyaev.Data
{
    public class EmployeeRepository : Repository
    {
        private readonly IDbConnection _dbConnection;
        private const string sqlGetAll = @"
                SELECT e.Id, e.Name, e.Surname, e.Phone, e.CompanyId, e.PassportId, e.DepartmentId,
                       p.Id, p.Type, p.Number, 
                       d.Id, d.Name, d.Phone
                FROM Employees AS e
                INNER JOIN Passports AS p ON e.PassportId = p.Id
                INNER JOIN Departments AS d ON e.DepartmentId = d.Id";
        private const string sqlGet = @"
                SELECT e.Id, e.Name, e.Surname, e.Phone, e.CompanyId, e.PassportId, e.DepartmentId,
                       p.Id, p.Type, p.Number, 
                       d.Id, d.Name, d.Phone
                FROM Employees AS e
                INNER JOIN Passports AS p ON e.PassportId = p.Id
                INNER JOIN Departments AS d ON e.DepartmentId = d.Id 
                WHERE e.Id = @Id";
        private const string sqlInsert = "INSERT INTO Employees (Name, Surname, Phone, CompanyId, PassportId, DepartmentId) " +
                "VALUES (@Name, @Surname, @Phone, @CompanyId, @PassportId, @DepartmentId); " +
                             "SELECT CAST(SCOPE_IDENTITY() as int)";
        private const string sqlUpdate = @"
                UPDATE employees 
                SET 
                    Name = @Name, 
                    Surname = @Surname, 
                    Phone = @Phone, 
                    CompanyId = @CompanyId,                     
                    DepartmentId = @DepartmentId 
                WHERE id = @Id";
        private const string sqlDelete =  "DELETE FROM Employees WHERE Id = @id";
        public EmployeeRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<IEnumerable<Employee>> GetAll()
        {
            var employees = await _dbConnection.QueryAsync<Employee, Passport, Department, Employee>(
                sqlGetAll,
                (employee, passport, department) =>
                {
                    employee.Passport = passport;
                    employee.PassportId = passport.Id;
                    employee.Department = department;
                    employee.DepartmentId = department.Id;
                    return employee;
                },
                 splitOn: "Id, Id"
            );
            return employees;
        }
        public async Task<Employee> Get( int id )
        {
            var result = await _dbConnection.QueryAsync<Employee, Passport, Department, Employee>(
                sqlGet,
                (employee, passport, department) =>
                {
                    employee.Passport = passport;
                    employee.PassportId = passport.Id;
                    employee.Department = department;
                    employee.DepartmentId = department.Id;
                    return employee;
                },
                 new { id },
                 splitOn: "Id, Id"
            );
            return result.FirstOrDefault();
        }
        public async Task<int> Create(Employee newEmployee)
        {
            return await _dbConnection.QuerySingleAsync<int>(sqlInsert, newEmployee);
        }
        public async Task Update(Employee editEmployee)
        {
            await _dbConnection.ExecuteAsync(sqlUpdate, editEmployee);
        }
        public async Task Delete(int id)
        {
            await _dbConnection.ExecuteAsync(sqlDelete, new { id });
        }
    }
}
