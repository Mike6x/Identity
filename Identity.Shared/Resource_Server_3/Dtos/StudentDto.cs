namespace Identity.Shared.Resource_Server_3.Dtos;

public class StudentDto
{
    public required int Id { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    
    public DateTime DateTime { get; set; }
    public string? Sex { get; set; }
    public int Age { get; set; }
    public string? Major { get; set; }
}