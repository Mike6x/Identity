using System.Text.Json;
using Resource_Server_3.Models;

namespace Resource_Server_3.Services;

public class JsonStudentDataService(IWebHostEnvironment environment) : IStudentService
{
        
        private List<Student>? _mockData = [];
        private readonly string _jsonFilePath = Path.Combine(environment.ContentRootPath, "Data", "Students.api.Data.json");
        private static readonly SemaphoreSlim FileReadLock = new(1, 1);


        private void LoadInMemoryStudents()
        {
            _mockData?.AddRange(new List<Student>
            {
                new Student { Id = 1101, FirstName = "Alice", Age = 20, Major = "Computer Science" },
                new Student { Id = 1102, FirstName = "Bob", Age = 22, Major = "Mathematics" },
                new Student { Id = 1103, FirstName = "Charlie", Age = 21, Major = "Physics" },
                new Student { Id = 1104, FirstName = "David", Age = 19, Major = "Engineering" },
                new Student { Id = 1105, FirstName = "Eva", Age = 23, Major = "Biology" },
                new Student { Id = 1106, FirstName = "Frank", Age = 18, Major = "Computer Science" },
                new Student { Id = 1107, FirstName = "Grace", Age = 24, Major = "Chemistry" },
                new Student { Id = 1108, FirstName = "Henry", Age = 20, Major = "Mathematics" },
                new Student { Id = 1109, FirstName = "Ivy", Age = 21, Major = "Physics" },
                new Student { Id = 1110, FirstName = "Jack", Age = 22, Major = "Engineering" },
                new Student { Id = 1111, FirstName = "Kate", Age = 20, Major = "Biology" },
                new Student { Id = 1112, FirstName = "Leo", Age = 19, Major = "Computer Science" },
                new Student { Id = 1113, FirstName = "Mia", Age = 22, Major = "Mathematics" },
                new Student { Id = 1114, FirstName = "Noah", Age = 23, Major = "Physics" },
                new Student { Id = 1115, FirstName = "Olivia", Age = 18, Major = "Engineering" },
                new Student { Id = 1116, FirstName = "Paul", Age = 21, Major = "Chemistry" }
            });
        }

        private void LoadStudentsFromJson()
        {
            // Use IWebHostEnvironment to get the path to wwwroot
            // string jsonFilePath = Path.Combine(_environment.WebRootPath, "StudentsData.json");
            Console.WriteLine($"Looking for JSON file at: {_jsonFilePath}");

            if (File.Exists(_jsonFilePath))
            {
                Console.WriteLine("JSON file found. Loading data...");
                string jsonData = File.ReadAllText(_jsonFilePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Handles different casing in JSON properties
                };
                var jsonStudents = JsonSerializer.Deserialize<List<Student>>(jsonData, options);
                if (jsonStudents != null)
                {
                    // Combine students from JSON with in-memory students
                    _mockData?.AddRange(jsonStudents);
                    Console.WriteLine($"{jsonStudents.Count} students loaded from JSON.");
                }
                else
                {
                    Console.WriteLine("No students found in JSON.");
                }
            }
            else
            {
                Console.WriteLine("JSON file not found.");
            }
        }
        
        private async Task EnsureDataLoadedAsync()
        {
            if (_mockData != null && _mockData.Count != 0) return; // Data already loaded

            await FileReadLock.WaitAsync(); // Acquire lock
            
            if (_mockData != null && _mockData.Count != 0) return; // Double-check after acquiring lock

            _mockData = [];
            
            // try
            // {
            //     LoadStudentsFromJson();
            // }
            // catch (JsonException jsonEx)
            // {
            //     Console.WriteLine(" JSON file errors.");
            //     
            // }
            // catch (Exception ex)
            // {
            //
            //     Console.WriteLine(" JSON file errors.");
            // }
            // finally
            // {
            //     FileReadLock.Release(); // Release lock
            // }
            
            LoadInMemoryStudents();
        }
        

        public async Task<IEnumerable<Student>> GetListAsync()
        {
            await EnsureDataLoadedAsync();
            return _mockData ?? Enumerable.Empty<Student>(); ;
        }
        
        public async Task<IEnumerable<Student>> SearchAsync(string searchTerm, int? minAge, int? maxAge, string sortBy, string sortOrder)
        {
            await EnsureDataLoadedAsync();
            
            
            var filteredStudents = _mockData?.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                filteredStudents = filteredStudents?.Where(s => s.Major != null && (s.FirstName.ToLower().Contains(searchTerm.ToLower()) || s.Major.ToLower().Contains(searchTerm.ToLower())));
            }

            if (minAge.HasValue)
            {
                filteredStudents = filteredStudents?.Where(s => s.Age >= minAge.Value);
            }

            if (maxAge.HasValue)
            {
                filteredStudents = filteredStudents?.Where(s => s.Age <= maxAge.Value);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy == "name")
                {
                    filteredStudents = sortOrder == "desc" ? filteredStudents?.OrderByDescending(s => s.FirstName) : filteredStudents?.OrderBy(s => s.FirstName);
                }
                else if (sortBy == "age")
                {
                    filteredStudents = sortOrder == "desc" ? filteredStudents?.OrderByDescending(s => s.Age) : filteredStudents?.OrderBy(s => s.Age);
                }
            }

            return filteredStudents != null ? filteredStudents.ToList() : [];
        }
        
        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            await EnsureDataLoadedAsync();
            return _mockData ?? Enumerable.Empty<Student>();
        }
        
        public async Task<Student?> GetByNameAsync(string name)
        {
            await EnsureDataLoadedAsync();
            
            var result = _mockData?.FirstOrDefault(c => c.FirstName.Equals(name, StringComparison.OrdinalIgnoreCase));
            
            return result;
        }
        
        public async Task<Student?> GetByIdAsync(int id)
        {
            await EnsureDataLoadedAsync();
            var result = _mockData?.FirstOrDefault(c => c.Id.Equals(id));
            
            return result;
        }
        
        public async Task<int?> DeleteAsync(int id)
        {
            await EnsureDataLoadedAsync();
            
            var result = _mockData?.FirstOrDefault(c => c.Id.Equals(id));
            
            if (result == null) return null;
            
            _mockData?.Remove(result);
            
            return result.Id;
        }

        public async Task<Student?> CreateAsync(string firstName, string lastName, int age, string major, string sex)
        {
            await EnsureDataLoadedAsync();
            
            var result = _mockData?.FirstOrDefault(c => c.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase));
            if (result != null) return null;
            
            var newId = _mockData?.Max(s => s.Id) + 1;

            var newStudent = new Student
            {
                Id = newId ?? 0,
                FirstName = firstName,
                LastName = lastName,
                Age = age,
                Sex = sex,
                Major = major,
                DateTime = DateTime.Now,
            };

            _mockData?.Add(newStudent);
            
            return newStudent;
        }
        
        public async Task<Student?> UpdateAsync(int id, string firstName, string lastName, int age, string major, string sex)
        {
            await EnsureDataLoadedAsync();
            
            var result = _mockData?.FirstOrDefault(c => c.Id.Equals(id));
            if (result == null) return null;
            
            var newStudent = new Student
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Age = age,
                Sex = sex,
                Major = major,
                DateTime = DateTime.Now,
            };
            
            _mockData?.Remove(result);
            
            _mockData?.Add(newStudent);
            
            return newStudent;
        }
        
    }