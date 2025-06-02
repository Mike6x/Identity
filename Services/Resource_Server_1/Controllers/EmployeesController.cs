using Identity.Shared.Authorization;
using Identity.Shared.Resource_Server_1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resource_Server_1.Models;

namespace Resource_Server_1.Controllers;

public class EmployeesController : ApiControllerBase
{
    private static readonly List<Employee> EmployeeList =
    [
        new() { Name = "John", Surname = "Doe", Email = "johnemail@mail.com", Phone = "900000001" },
        new() { Name = "Johny", Surname = "Be Good", Email = "johnygoodemail@mail.com", Phone = "800000002" },
        new() { Name = "Ann", Surname = "Funny", Email = "annfunny@mail.com", Phone = "700000005" }
    ];

    [HttpGet("GetList")]
    [Authorize(Policy = AppScopes.EmployeeReadScope)]
    public IActionResult GetList() => Ok(EmployeeList);
    
    [HttpGet("Search")]
    [Authorize(Roles = "admin, user")]
   
    public IActionResult Search() => Ok(EmployeeList.Select(x => new EmployeeDto() { Name = x.Name, Surname = x.Surname}));
    
    [HttpDelete("{id:Guid}")]
    [Authorize(Roles = "admin")]
    public IActionResult DeleteEmployee(Guid id)
    {
        var result = EmployeeList.FirstOrDefault(x => x.Id == id);
        if (result != null) EmployeeList.Remove(result);
        
        return Ok(id);
    }
    
    [HttpGet("{id:Guid}")]
    [AllowAnonymous]
    public Task<ActionResult<Employee>> GetById(Guid id)
    {
        try
        {
            var result = EmployeeList.FirstOrDefault(x => x.Id == id);

            if (result == null)
            {
                return Task.FromResult<ActionResult<Employee>>(NotFound());
            }

            return Task.FromResult<ActionResult<Employee>>(result);
        }
        catch (Exception)
        {
            return Task.FromResult<ActionResult<Employee>>(StatusCode(StatusCodes.Status500InternalServerError,
                "Error retrieving data from the database"));
        }
    }
    
    [HttpGet("")]
    [AllowAnonymous]
    public IActionResult GetAll() => Ok(EmployeeList.Select(x => new Employee() { Id = x.Id, Name = x.Name, Surname = x.Surname}));
    
    //Anonymous on purpose to test api gateway also for anonymous endpoints
    [HttpPost("")]
    [AllowAnonymous]
    public IActionResult CreateEmployee([FromBody] EmployeeDto request)
    {
        var newEmployee = new Employee
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Surname = request.Surname,
            Email = request.Email,
            Phone = request.Phone
        };

        EmployeeList.Add(newEmployee);
        return Ok();
    }
    
    [HttpPut("")]
    [AllowAnonymous]

    public Task<ActionResult<Employee>> UpdateEmployee(Guid id,[FromBody] EmployeeDto request)
    {
        try
        {
            var result = EmployeeList.FirstOrDefault(x => x.Id == id);

            if (result == null)
            {
                return Task.FromResult<ActionResult<Employee>>(NotFound($"Employee with Id = {id} not found"));
            }
            
            var newEmployee = new Employee
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                Phone = request.Phone
            };
            
            EmployeeList.Remove(result);
            
            EmployeeList.Add(newEmployee);
            
            return Task.FromResult<ActionResult<Employee>>(Ok(newEmployee));
        }
        catch (Exception)
        {
            return Task.FromResult<ActionResult<Employee>>(StatusCode(StatusCodes.Status500InternalServerError,
                "Error updating data"));
        }
    }
}