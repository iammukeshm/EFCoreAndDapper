using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class EmployeeDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public DepartmentDto Department { get; set; }
    }
}