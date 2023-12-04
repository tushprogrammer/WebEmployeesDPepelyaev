using WebEmployeesDPepelyaev.Entitys;
using WebEmployeesDPepelyaev.Models;

namespace WebEmployeesDPepelyaev.Data
{
    public interface IDataWork 
    {
        Task<int> AddNewEmployee(EmployeeModel model);
        Task UpdateEmployee(EmployeeModel model);
        Task<IEnumerable<Employee>> GetAllEmployees();
        Task<IEnumerable<Company>> GetCompanies();
        Task<IEnumerable<Department>> GetDepartments();
        Task<Employee> GetEmployee(int id);
        Task<IEnumerable<PassportType>> GetTypesPassports();
        Task RemoveEmployee(int id);
        Task<EmployeeModel> GetEmployeeModel(EmployeeModel model);
        Task<EmployeeModel> GetEmployeeModel();
        Task<EmployeeModel> GetEmployeeModel(int idEmployee);
        Task<GetEmployeesModel> GetEmployeesModel();
        Task<GetEmployeesModel> GetEmployeesModelCompany(int companyId);
        Task<GetEmployeesModel> GetEmployeesModelDepartment(int departmentId);
    }
}
