namespace Resource_Server_1.Models;

public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}