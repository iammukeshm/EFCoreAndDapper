using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using WebApi.DTOs;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        public EmployeeController(IApplicationDbContext dbContext, IApplicationReadDbConnection readDbConnection, IApplicationWriteDbConnection writeDbConnection)
        {
            _dbContext = dbContext;
            _readDbConnection = readDbConnection;
            _writeDbConnection = writeDbConnection;
        }

        public IApplicationDbContext _dbContext { get; }
        public IApplicationReadDbConnection _readDbConnection { get; }
        public IApplicationWriteDbConnection _writeDbConnection { get; }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var query = $"Select * from Employees";
            var employees = await _readDbConnection.QueryAsync<Employee>(query);
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllEmployeesById(int id)
        {
            var employees = await _dbContext.Employees.Include(a => a.Department).Where(a => a.Id == id).ToListAsync();
            return Ok(employees);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewEmployeeWithDepartment(EmployeeDto employeeDto)
        {
            _dbContext.Connection.Open();
            using (var transaction = _dbContext.Connection.BeginTransaction())
            {
                try
                {
                    _dbContext.Database.UseTransaction(transaction as DbTransaction);
                    //Check if Department Exists (By Name)
                    bool DepartmentExists = await _dbContext.Departments.AnyAsync(a => a.Name == employeeDto.Department.Name);
                    if (DepartmentExists)
                    {
                        throw new Exception("Department Already Exists");
                    }
                    //Add Department
                    var addDepartmentQuery = $"INSERT INTO Departments(Name,Description) VALUES('{employeeDto.Department.Name}','{employeeDto.Department.Description}');SELECT CAST(SCOPE_IDENTITY() as int)";
                    var departmentId = await _writeDbConnection.QuerySingleAsync<int>(addDepartmentQuery, transaction: transaction);
                    //Check if Department Id is not Zero.
                    if (departmentId == 0)
                    {
                        throw new Exception("Department Id");
                    }
                    //Add Employee
                    var employee = new Employee
                    {
                        DepartmentId = departmentId,
                        Name = employeeDto.Name,
                        Email = employeeDto.Email
                    };
                    await _dbContext.Employees.AddAsync(employee);
                    await _dbContext.SaveChangesAsync(default);
                    //Commmit
                    transaction.Commit();
                    //Return EmployeeId
                    return Ok(employee.Id);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    _dbContext.Connection.Close();
                }
            }
        }
    }
}