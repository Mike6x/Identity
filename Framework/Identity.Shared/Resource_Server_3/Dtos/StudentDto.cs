using System.ComponentModel.DataAnnotations;

namespace Identity.Shared.Resource_Server_3.Dtos;

public class StudentDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter your name.")]
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    
    public DateTime DateTime { get; set; } = DateTime.Now;
    public string? Sex { get; set; } = "Male";
    public int Age { get; set; }
    public string? Major { get; set; }
}