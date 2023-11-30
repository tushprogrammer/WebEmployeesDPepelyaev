using WebEmployeesDPepelyaev.Entitys;
using WebEmployeesDPepelyaev.Models;

namespace WebEmployeesDPepelyaev.Data
{
    public interface IDataEmployee
    {
        int AddNewEmployee(EmployeeModel model);
        void UpdateEmployee(EmployeeModel model);
        IEnumerable<Employee> GetAllEmployees();
        IEnumerable<Company> GetCompanies();
        IEnumerable<Department> GetDepartments();
        Employee GetEmployee(int id);
        IEnumerable<PassportType> GetTypesPassports();
        void RemoveEmployee(int id);
    }
}
