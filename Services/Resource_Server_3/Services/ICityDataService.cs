using Resource_Server_3.Models;

namespace Resource_Server_3.Services;

public interface ICityDataService
{

    Task<CityData?> GetByNameAsync(string name);

    Task<IEnumerable<CityData>> GetAllAsync();
    
}