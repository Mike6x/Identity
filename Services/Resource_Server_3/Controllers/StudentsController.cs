using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resource_Server_3.Services;
using Identity.Shared.Resource_Server_3.Dtos;

namespace Resource_Server_3.Controllers;

public class StudentsController(IStudentService studentService) : ApiControllerBase
{
    [HttpPost("GetList")]
    [AllowAnonymous]
    public async Task<IActionResult> GetList()
    {
        var students =await  studentService.GetListAsync();

        return Ok(students);
    }
    
    [HttpPost("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search(string searchTerm, int? minAge, int? maxAge, string sortBy, string sortOrder)
    {
        var students = await studentService.SearchAsync(searchTerm, minAge, maxAge, sortBy, sortOrder);

        return Ok(students);
    }
    
    [HttpPost("export")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ExportToJson()
    {
        var students = await studentService.GetAllAsync();
        var json = JsonSerializer.Serialize(students, new JsonSerializerOptions { WriteIndented = true });
        return File(Encoding.UTF8.GetBytes(json), "application/json", "students.json");
    }

    [HttpGet]
    [AllowAnonymous]

    public async Task<IActionResult> GetAll()
    {
        var result = await studentService.GetAllAsync();
        
        return Ok(result);
        
    }
    
    [HttpGet("{id:int}")]
    [AllowAnonymous]

    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await studentService.GetByIdAsync(id);
        
        if (result == null) return NotFound();
        
        return Ok(result);
        
    }
    
    [HttpGet("name/{name}")]
    [AllowAnonymous]

    public async Task<IActionResult> GetByNameAsync(string name)
    {
        var result = await studentService.GetByNameAsync(name);
        
        if (result == null) return NotFound();
        
        return Ok(result);
        
    }
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateAsync(StudentDto item)
    {
        var students = await studentService.CreateAsync(
            item.FirstName, 
            item.LastName ?? string.Empty,
            item.Age,
            item.Major ?? "Art",
            item.Sex ?? "Female");
        
        return Ok(students);
    }
    
    [HttpPut("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateAsync(int id, StudentDto? item)
    {
        if (item == null || item.Id < 1 || id != item.Id)
        {
            return BadRequest("Invalid data.");
        }
        var students = await studentService.UpdateAsync(
            item.Id,
            item.FirstName, 
            item.LastName ?? string.Empty,
            item.Age,
            item.Major ?? "Art",
            item.Sex ?? "Female");
        
        return Ok(students);
    }
    
    [HttpDelete("{id:int}")]
    [AllowAnonymous]
 
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await  studentService.DeleteAsync(id);
        
        return Ok(result);
    }
    
}