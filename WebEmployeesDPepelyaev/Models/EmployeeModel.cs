using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WebEmployeesDPepelyaev.Entitys;

namespace WebEmployeesDPepelyaev.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Поле 'Имя' является обязательным")]
        [StringLength(30, ErrorMessage = "Поле 'Имя' должно быть не более 30 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле 'Фамилия' является обязательным")]
        [StringLength(30, ErrorMessage = "Поле 'Фамилия' должно быть не более 30 символов")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Поле 'Номер' телефона является обязательным")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Поле 'Компания' является обязательным")]
        public string CompanyId { get; set; }
        [StringLength(15, ErrorMessage = "Поле 'Номер паспорта' должно быть не более 15 символов")]
        [Required(ErrorMessage = "Поле 'Номер паспорта' является обязательным")]
        public string PassportNumber { get; set; }
        
        [Required(ErrorMessage = "Поле 'Тип паспорта' является обязательным")]
        public string PassportType { get; set; }

        [Required(ErrorMessage = "Поле 'Департамент' является обязательным")]
        public string DepartmentId { get; set; }
        public SelectList? Departments { get; set; }
        public SelectList? Passports { get; set; }
        public SelectList? Companies { get; set; }
    }
}
