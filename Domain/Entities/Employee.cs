using CWM.DotNetCore.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Employee : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
