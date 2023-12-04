using Dapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.Design;
using System.Data;
using System.Reflection;
using System.Transactions;
using WebEmployeesDPepelyaev.Entitys;
using WebEmployeesDPepelyaev.Models;

namespace WebEmployeesDPepelyaev.Data
{
    public class DataWork : IDataWork
    {
        private readonly CompanyRepository _companyRepository;
        private readonly DepartmentRepository _departmentRepository;
        private readonly PassportTypeRepository _passportTypeRepository;
        private readonly EmployeeRepository _employeeRepository;
        private readonly PassportRepository _passportRepository;

        public DataWork(CompanyRepository companyRepository,
            DepartmentRepository departmentRepository, PassportTypeRepository passportTypeRepository,
            EmployeeRepository employeeRepository, PassportRepository passportRepository)
        {
            _companyRepository = companyRepository;
            _departmentRepository = departmentRepository;
            _passportTypeRepository = passportTypeRepository;
            _employeeRepository = employeeRepository;
            _passportRepository = passportRepository;
        }
        public async Task<IEnumerable<Company>> GetCompanies()
        {
            return await _companyRepository.GetAll();
        }
        public async Task<IEnumerable<Department>> GetDepartments()
        {
            return await _departmentRepository.GetAll();
        }
        public async Task<IEnumerable<PassportType>> GetTypesPassports()
        {
            return await _passportTypeRepository.GetAll();
        }
        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            return await _employeeRepository.GetAll();
        }
        public async Task<Employee> GetEmployee(int id)
        {
            return await _employeeRepository.Get(id);
        }
        public async Task<int> AddNewEmployee(EmployeeModel model)
        {
            Passport passport = new Passport()
            {
                Number = model.PassportNumber,
                Type = model.PassportType
            };
            int idPassprot = await _passportRepository.Create(passport);


            Employee newEmployee = new Employee()
            {
                Name = model.Name,
                Surname = model.Surname,
                Phone = model.Phone,
                CompanyId = Convert.ToInt32(model.CompanyId),
                DepartmentId = Convert.ToInt32(model.DepartmentId),
                PassportId = idPassprot,
            };

            int idEmployee = await _employeeRepository.Create(newEmployee);
            return idEmployee;
        }
        public async Task UpdateEmployee(EmployeeModel model)
        {
            Passport editPassport = new()
            {
                Id = model.Id,
                Type = model.PassportType,
                Number = model.PassportNumber
            };
            Employee editEmployee = new()
            {
                Id = model.Id,
                Name = model.Name,
                Surname = model.Surname,
                Phone = model.Phone,
                CompanyId = Convert.ToInt32(model.CompanyId),
                DepartmentId = Convert.ToInt32(model.DepartmentId),
            };

            await _employeeRepository.Update(editEmployee);
            await _passportRepository.Update(editPassport);

        }
        public async Task RemoveEmployee(int id)
        {
            await _employeeRepository.Delete(id);
        }
        public async Task<EmployeeModel> GetEmployeeModel()
        {
            IEnumerable<Company> companies = await GetCompanies();
            SelectList selectListCompanies = new SelectList(companies, "Id", "Name");
            IEnumerable<Department> departments = await GetDepartments();
            SelectList selectListDepartments = new SelectList(departments, "Id", "Name");
            IEnumerable<PassportType> passportTypes = await GetTypesPassports();
            SelectList selectListPassportTypes = new SelectList(passportTypes, "Type", "Description");
            return new EmployeeModel()
            {
                Departments = selectListDepartments,
                Passports = selectListPassportTypes,
                Companies = selectListCompanies
            };
        }
        //обновление внутренних списков сортировки
        public async Task<EmployeeModel> GetEmployeeModel(EmployeeModel model)
        {
            EmployeeModel baseModel = await GetEmployeeModel();
            model.Departments = baseModel.Departments;
            model.Companies = baseModel.Companies;
            model.Passports = baseModel.Passports;
            return model;
        }
        public async Task<GetEmployeesModel> GetEmployeesModel()
        {
            IEnumerable<Company> companies = await GetCompanies();
            SelectList selectListCompanies = new SelectList(companies, "Id", "Name");
            IEnumerable<Department> departments = await GetDepartments();
            SelectList selectListDepartments = new SelectList(departments, "Id", "Name");

            GetEmployeesModel model = new GetEmployeesModel()
            {
                Companies = selectListCompanies,
                Departments = selectListDepartments,
                Employees = await GetAllEmployees()
            };
            return model;
        }
        public async Task<GetEmployeesModel> GetEmployeesModelCompany(int companyId)
        {
            IEnumerable<Company> companies = await GetCompanies();
            SelectList selectListCompanies = new SelectList(companies, "Id", "Name");
            IEnumerable<Department> departments = await GetDepartments();
            SelectList selectListDepartments = new SelectList(departments, "Id", "Name");

            IEnumerable<Employee> employees = await GetAllEmployees();
            GetEmployeesModel model = new GetEmployeesModel()
            {
                Companies = selectListCompanies,
                Departments = selectListDepartments,
                Employees = employees.Where(i => i.CompanyId == companyId),
            };
            return model;
        }
        public async Task<GetEmployeesModel> GetEmployeesModelDepartment(int departmentId)
        {
            IEnumerable<Company> companies = await GetCompanies();
            SelectList selectListCompanies = new SelectList(companies, "Id", "Name");
            IEnumerable<Department> departments = await GetDepartments();
            SelectList selectListDepartments = new SelectList(departments, "Id", "Name");
            IEnumerable<Employee> employees = await GetAllEmployees();
            GetEmployeesModel model = new GetEmployeesModel()
            {
                Companies = selectListCompanies,
                Departments = selectListDepartments,
                Employees = employees.Where(i => i.DepartmentId == departmentId),
            };
            return model;
        }
        public async Task<EmployeeModel> GetEmployeeModel(int idEmployee){
            Employee employeeNow = await GetEmployee(idEmployee);
            IEnumerable<Company> companies = await GetCompanies();
            SelectList selectListCompanies = new SelectList(companies, "Id", "Name");
            IEnumerable<Department> departments = await GetDepartments();
            SelectList selectListDepartments = new SelectList(departments, "Id", "Name");
            IEnumerable<PassportType> passportTypes = await GetTypesPassports();
            SelectList selectListPassportTypes = new SelectList(passportTypes, "Type", "Description");

            EmployeeModel model = new EmployeeModel()
            {
                Id = employeeNow.Id,
                Name = employeeNow.Name,
                Surname = employeeNow.Surname,
                Phone = employeeNow.Phone,
                CompanyId = employeeNow.CompanyId.ToString(),
                DepartmentId = employeeNow.DepartmentId.ToString(),
                PassportType = employeeNow.Passport.Type,
                PassportNumber = employeeNow.Passport.Number,
                Companies = selectListCompanies,
                Departments = selectListDepartments,
                Passports = selectListPassportTypes
            };

            return model; 
        }
    }
}
