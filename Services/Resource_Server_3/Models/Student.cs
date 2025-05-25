using Newtonsoft.Json;

namespace Resource_Server_3.Models;

public class Student
{
    public required int Id { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateTime { get; set; }
    public string? Sex { get; set; }
    public int Age { get; set; }
    public string? Major { get; set; }
}