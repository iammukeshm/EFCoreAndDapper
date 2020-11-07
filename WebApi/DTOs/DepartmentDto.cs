using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class DepartmentDto
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}