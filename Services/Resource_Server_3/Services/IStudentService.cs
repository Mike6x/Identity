using Resource_Server_3.Models;

namespace Resource_Server_3.Services;

public interface IStudentService
{
   Task<IEnumerable<Student>> GetAllAsync();
   
   Task<IEnumerable<Student>> GetListAsync();
   
   Task<IEnumerable<Student>> SearchAsync(string searchTerm, int? minAge, int? maxAge, string sortBy, string sortOrder);
   
   Task<Student?> GetByNameAsync(string name);
   
   Task<Student?> GetByIdAsync(int id);

   Task<Student?> CreateAsync(string firstName, string lastName, int age, string major, string sex);

   public Task<int?> DeleteAsync(int id);
   
   Task<Student?> UpdateAsync(int id, string firstName, string lastName, int age, string major, string sex);

}