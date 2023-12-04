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
        private readonly IDataWork dataWork;

        public HomeController(IDataWork data)
        {
            dataWork = data;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        //страница добавления нового сотрудника
        public async Task<IActionResult> AddEmployee()
        {
            //передача департаментов, типов паспортов и компаний для выпадающих списков
            EmployeeModel model = await dataWork.GetEmployeeModel();
            return View(model);
        }
        //метод обработки данных со страницы
        [HttpPost]
        public async Task<IActionResult> AddNewEmployeeMethod(EmployeeModel model)
        {
            //проверка на валидность
            if (ModelState.IsValid)
            {
                int id = await dataWork.AddNewEmployee(model);
                //сохранение id для отображения в виде всплывающего уведомления на следующей странице
                TempData["newEmployeeId"] = id;
            }
            else
            {
                //вывод сообщения об ошибке валидации
                ModelState.AddModelError("", "Заполните все обязательные поля");
                await dataWork.GetEmployeeModel(model);                
                return View("AddEmployee", model); //повторная попытка
            }
            //после успешного добавления, возврат на главную страницу
            return RedirectToAction("Index");
        }
        //страница со списком всех сотрудников
        public async Task<IActionResult> GetEmployees()
        {
            GetEmployeesModel model = await dataWork.GetEmployeesModel();
            return View(model);
        }
        //страница со списком сотрудников с фильтром по компании
        public async Task<IActionResult> GetEmployeesCompany(int companyId)
        {
            GetEmployeesModel model = await dataWork.GetEmployeesModelCompany(companyId);
            //вывод сотрудников из выбранной компании 
            return View("GetEmployees", model);
        }
        //страница со списком сотрудников с фильтром по департаменту
        public async Task<IActionResult> GetEmployeesDepartment(int departmentId)
        {
            GetEmployeesModel model = await dataWork.GetEmployeesModelDepartment(departmentId);
            //вывод сотрудников из выбранного департамента 
            return View("GetEmployees", model);
        }
        //страница изменения данных сотрудника
        public async Task<IActionResult> EditEmployee(int id)
        {
            EmployeeModel model = await dataWork.GetEmployeeModel(id);
            return View(model);
        }
        [HttpPost]
        //метод обработки данных со страницы изменения
        public async Task<IActionResult> EditEmployeeMethod(EmployeeModel model)
        {
            //проверка на валидность
            if (ModelState.IsValid)
            {
                await dataWork.UpdateEmployee(model);
            }
            else
            {
                ModelState.AddModelError("", "Заполните все обязательные поля");
                await dataWork.GetEmployeeModel(model);
                return View("AddEmployee", model); //повторная попытка
            }

            return RedirectToAction("GetEmployees");
        }
        //метод обработки запроса на удаление
        public IActionResult RemoveEmployee(int id)
        {
            dataWork.RemoveEmployee(id);
            var r = Request.Headers["Referer"].ToString();
            return Redirect(r);
        }
        
    }
}