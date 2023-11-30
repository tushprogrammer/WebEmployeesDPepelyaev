using Microsoft.AspNetCore.Mvc.Rendering;
using WebEmployeesDPepelyaev.Entitys;
namespace WebEmployeesDPepelyaev.Models

{
    public class GetEmployeesModel
    {
        public IEnumerable<Employee> Employees { get; set; }
        public SelectList Departments { get; set; }
        public SelectList Companies { get; set; }
    }
}
