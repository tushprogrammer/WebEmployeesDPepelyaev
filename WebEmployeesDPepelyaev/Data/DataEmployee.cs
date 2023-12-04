using Dapper;
using System.Data;
using System.Transactions;
using WebEmployeesDPepelyaev.Entitys;
using WebEmployeesDPepelyaev.Models;

namespace WebEmployeesDPepelyaev.Data
{
    public class DataEmployee : IDataEmployee
    {
        private readonly IDbConnection _dbConnection;

        public DataEmployee(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public IEnumerable<Company> GetCompanies()
        {
            return _dbConnection.Query<Company>("SELECT Id, Name FROM Companies");
        }
        public IEnumerable<Department> GetDepartments()
        {
            return _dbConnection.Query<Department>("Select Id, Name, Phone FROM Departments");
        }
        public IEnumerable<PassportType> GetTypesPassports()
        {
            return _dbConnection.Query<PassportType>("SELECT Id, Type, Description FROM PassportTypes");
        }
        public IEnumerable<Employee> GetAllEmployees()
        {
            //var empl = _dbConnection.Query<Employee>("SELECT Id, Name, Surname, Phone, CompanyId, PassportId, DepartmentId FROM Employees");
            //var something = _dbConnection.Query(sql);
            var sql = @"
                SELECT e.Id, e.Name, e.Surname, e.Phone, e.CompanyId, e.PassportId, e.DepartmentId,
                       p.Id, p.Type, p.Number, 
                       d.Id, d.Name, d.Phone
                FROM Employees AS e
                INNER JOIN Passports AS p ON e.PassportId = p.Id
                INNER JOIN Departments AS d ON e.DepartmentId = d.Id";

            var employees = _dbConnection.Query<Employee, Passport, Department, Employee>(
                sql,
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
        public Employee GetEmployee(int id)
        {
            var sql = @"
                SELECT e.Id, e.Name, e.Surname, e.Phone, e.CompanyId, e.PassportId, e.DepartmentId,
                       p.Id, p.Type, p.Number, 
                       d.Id, d.Name, d.Phone
                FROM Employees AS e
                INNER JOIN Passports AS p ON e.PassportId = p.Id
                INNER JOIN Departments AS d ON e.DepartmentId = d.Id 
                WHERE e.Id = @Id";
            Employee employee = _dbConnection.Query<Employee, Passport, Department, Employee>(
                sql,
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
            ).First();
            return employee;
        }
        public int AddNewEmployee(EmployeeModel model)
        {
            Passport passport = new Passport()
            {
                Number = model.PassportNumber,
                Type = model.PassportType
            };
            string insertQueryPassport = "INSERT INTO Passports (Type, Number) VALUES (@Type, @Number); " +
                             "SELECT CAST(SCOPE_IDENTITY() as int)";
            int idPassport = _dbConnection.QuerySingle<int>(insertQueryPassport, passport);


            Employee newEmployee = new Employee()
            {
                Name = model.Name,
                Surname = model.Surname,
                Phone = model.Phone,
                CompanyId = Convert.ToInt32(model.CompanyId),
                DepartmentId = Convert.ToInt32(model.DepartmentId),
                PassportId = idPassport,
            };
            string insertQueryEmployee = "INSERT INTO Employees (Name, Surname, Phone, CompanyId, PassportId, DepartmentId) " +
                "VALUES (@Name, @Surname, @Phone, @CompanyId, @PassportId, @DepartmentId); " +
                             "SELECT CAST(SCOPE_IDENTITY() as int)";
            int idEmployee = _dbConnection.QuerySingle<int>(insertQueryEmployee, newEmployee);
            return idEmployee;
        }
        public void UpdateEmployee(EmployeeModel model)
        {
            //изменения данных представлены в двух вариантах из-за расплывчатого описания
            //изменение сотрудника по id целиком
            Passport editPassport = new Passport()
            {
                Id = model.Id,
                Type = model.PassportType,
                Number = model.PassportNumber
            };
            Employee editEmployee = new Employee
            {
                Id = model.Id,
                Name = model.Name,
                Surname = model.Surname,
                Phone = model.Phone,
                CompanyId = Convert.ToInt32(model.CompanyId),
                DepartmentId = Convert.ToInt32(model.DepartmentId),
            };

            string sqlEmployee = @"
                UPDATE employees 
                SET 
                    Name = @Name, 
                    Surname = @Surname, 
                    Phone = @Phone, 
                    CompanyId = @CompanyId,                     
                    DepartmentId = @DepartmentId 
                WHERE id = @Id";
            string sqlPassport = @"UPDATE Passports 
                Set
                    Type = @Type,
                    Number = @Number
                WHERE Id = @Id";

            _dbConnection.Open();
            using (var editTransaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    _dbConnection.Execute(sqlEmployee, editEmployee, editTransaction);
                    _dbConnection.Execute(sqlPassport, editPassport, editTransaction);
                    editTransaction.Commit();
                }
                catch
                {
                    editTransaction.Rollback();
                }
            }
            _dbConnection.Close();
            //обновление сотрудника по частям (изменения должно быть только тех полей, которые указаны в запросе)
            //Employee oldEmployee = GetEmployee(model.Id);
            //using (var editTransaction = _dbConnection.BeginTransaction())
            //{
            //    try
            //    {
            //        if (oldEmployee.Name != model.Name)
            //        {
            //            var sql1 = "UPDATE employees SET Name = @newName WHERE id = @Id";
            //            _dbConnection.Execute(sql1, new { model.Name, model.Id }, transaction: editTransaction);
            //        }
            //        if (oldEmployee.Surname != model.Surname)
            //        {
            //            var sql2 = "UPDATE employees SET Surname = @Surname WHERE id = @Id";
            //            _dbConnection.Execute(sql2, new { model.Surname, model.Id }, transaction: editTransaction);
            //        }
            //        if (oldEmployee.Phone != model.Phone)
            //        {
            //            var sql3 = "UPDATE employees SET Surname = @Surname WHERE id = @Id";
            //            _dbConnection.Execute(sql3, new { model.Phone, model.Id }, transaction: editTransaction);
            //        }
            //        if (oldEmployee.CompanyId != Convert.ToInt32(model.CompanyId))
            //        {
            //            var sql4 = "UPDATE employees SET CompanyId = @CompanyId WHERE id = @Id";
            //            _dbConnection.Execute(sql4, new { model.CompanyId, model.Id }, transaction: editTransaction);
            //        }
            //        if (oldEmployee.DepartmentId != Convert.ToInt32(model.DepartmentId))
            //        {
            //            var sql5 = "UPDATE employees SET DepartmentId = @DepartmentId WHERE id = @Id";
            //            _dbConnection.Execute(sql5, new { model.DepartmentId, model.Id }, transaction: editTransaction);
            //        }
            //        if (oldEmployee.Passport.Type != model.PassportType)
            //        {
            //            var sql6 = "UPDATE Passports SET Type = @PassportType WHERE id = @Id";
            //            _dbConnection.Execute(sql6, new { model.PassportType, model.Id }, transaction: editTransaction);
            //        }
            //        if (oldEmployee.Passport.Number != model.PassportNumber)
            //        {
            //            var sql7 = "UPDATE Passports SET Number = @PassportNumber WHERE id = @Id";
            //            _dbConnection.Execute(sql7, new { model.PassportNumber, model.Id }, transaction: editTransaction);
            //        }
            //        editTransaction.Commit();
            //    }
            //    catch (Exception ex)
            //    {
            //        editTransaction.Rollback();
            //    }
            //}
        }
        public void RemoveEmployee(int id)
        {
            var sqlQuery = "DELETE FROM Employees WHERE Id = @id";
            _dbConnection.Execute(sqlQuery, new { id });
        }
    }
}
