using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using WebEmployeesDPepelyaev.Data;
using WebEmployeesDPepelyaev.Entitys;
using WebEmployeesDPepelyaev.Models;

namespace WebEmployeesDPepelyaev.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataEmployee dataEmployee;

        public HomeController(IDataEmployee data)
        {
            dataEmployee = data;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        //страница добавления нового сотрудника
        public IActionResult AddEmployee()
        {
            //передача департаментов, типов паспортов и компаний для выпадающих списков
            EmployeeModel model = GetEmployeeModel();
            return View(model);
        }
        //метод обработки данных со страницы
        [HttpPost]
        public IActionResult AddNewEmployeeMethod(EmployeeModel model)
        {
            //проверка на валидность
            if (ModelState.IsValid)
            {
                //добавление сущности в бд
                int id = dataEmployee.AddNewEmployee(model);
                //сохранение id для отображения в виде всплывающего уведомления на следующей странице
                TempData["newEmployeeId"] = id;
            }
            else
            {
                //вывод сообщения об ошибке валидации
                ModelState.AddModelError("", "Заполните все обязательные поля");
                EmployeeModel baseModel = GetEmployeeModel();
                model.Departments = baseModel.Departments;
                model.Companies = baseModel.Companies;
                model.Passports = baseModel.Passports;
                return View("AddEmployee", model); //повторная попытка
            }
            //после успешного добавления, возврат на главную страницу
            return RedirectToAction("Index");
        }
        //страница со списком сотрудников
        public IActionResult GetEmployees()
        {
            //данные для фильтров
            IEnumerable<Company> companies = dataEmployee.GetCompanies();
            SelectList selectListCompanies = new SelectList(companies, "Id", "Name");
            IEnumerable<Department> departments = dataEmployee.GetDepartments();
            SelectList selectListDepartments = new SelectList(departments, "Id", "Name");

            GetEmployeesModel model = new GetEmployeesModel()
            {
                Companies = selectListCompanies,
                Departments = selectListDepartments,
                Employees = dataEmployee.GetAllEmployees()
            };
            //вывод всех сотрудников 
            return View(model);
        }
        //страница со списком сотрудников с фильтром по компании
        public IActionResult GetEmployeesCompany(int companyId)
        {
            IEnumerable<Company> companies = dataEmployee.GetCompanies();
            SelectList selectListCompanies = new SelectList(companies, "Id", "Name");
            IEnumerable<Department> departments = dataEmployee.GetDepartments();
            SelectList selectListDepartments = new SelectList(departments, "Id", "Name");

            GetEmployeesModel model = new GetEmployeesModel()
            {
                Companies = selectListCompanies,
                Departments = selectListDepartments,
                Employees = dataEmployee.GetAllEmployees().Where(i => i.CompanyId == companyId),
            };
            //вывод сотрудников из выбранной компании 
            return View("GetEmployees", model);
        }
        //страница со списком сотрудников с фильтром по департаменту
        public IActionResult GetEmployeesDepartment(int departmentId)
        {
            IEnumerable<Company> companies = dataEmployee.GetCompanies();
            SelectList selectListCompanies = new SelectList(companies, "Id", "Name");
            IEnumerable<Department> departments = dataEmployee.GetDepartments();
            SelectList selectListDepartments = new SelectList(departments, "Id", "Name");

            GetEmployeesModel model = new GetEmployeesModel()
            {
                Companies = selectListCompanies,
                Departments = selectListDepartments,
                Employees = dataEmployee.GetAllEmployees().Where(i => i.DepartmentId == departmentId),
            };
            //вывод сотрудников из выбранного департамента 
            return View("GetEmployees", model);
        }
        //страница изменения данных сотрудника
        public IActionResult EditEmployee(int id)
        {
            Employee employeeNow = dataEmployee.GetEmployee(id);
            EmployeeModel model = GetEmployeeModel();
            model.Id = id;
            model.Name = employeeNow.Name;
            model.Surname = employeeNow.Surname;
            model.Phone = employeeNow.Phone;
            model.CompanyId = employeeNow.CompanyId.ToString();
            model.DepartmentId = employeeNow.DepartmentId.ToString();
            model.PassportType = employeeNow.Passport.Type;
            model.PassportNumber = employeeNow.Passport.Number;
            return View(model);
        }
        [HttpPost]
        //метод обработки данных со страницы изменения
        public IActionResult EditEmployeeMethod(EmployeeModel model)
        {
            //проверка на валидность
            if (ModelState.IsValid)
            {
                //изменение данных в бд
                dataEmployee.UpdateEmployee(model);
            }
            else
            {
                ModelState.AddModelError("", "Заполните все обязательные поля");
                EmployeeModel baseModel = GetEmployeeModel();
                model.Departments = baseModel.Departments;
                model.Companies = baseModel.Companies;
                model.Passports = baseModel.Passports;
                return View("AddEmployee", model); //повторная попытка
            }

            return RedirectToAction("GetEmployees");
        }
        //метод обработки запроса на удаление
        public IActionResult RemoveEmployee(int id)
        {
            dataEmployee.RemoveEmployee(id);
            var r = Request.Headers["Referer"].ToString();
            return Redirect(r);
        }
        //метод для формирования модели сотрудников для добавления/изменения
        private EmployeeModel GetEmployeeModel()
        {
            IEnumerable<Company> companies = dataEmployee.GetCompanies();
            SelectList selectListCompanies = new SelectList(companies, "Id", "Name");
            IEnumerable<Department> departments = dataEmployee.GetDepartments();
            SelectList selectListDepartments = new SelectList(departments, "Id", "Name");
            IEnumerable<PassportType> passportTypes = dataEmployee.GetTypesPassports();
            SelectList selectListPassportTypes = new SelectList(passportTypes, "Type", "Description");
            return new EmployeeModel()
            {
                Departments = selectListDepartments,
                Passports = selectListPassportTypes,
                Companies = selectListCompanies
            };
        }
    }
}