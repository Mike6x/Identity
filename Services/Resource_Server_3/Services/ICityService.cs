using Identity.Shared.Resource_Server_3.Dtos;
using Resource_Server_3.Models;

namespace Resource_Server_3.Services;

public interface ICityService
{
    Task<IEnumerable<CityData>> GetAllAsync();
   
    Task<IEnumerable<CityData>> GetListAsync();
    
    Task<CityData?> GetByCodeAsync(string code);
   
    Task<CityData?> GetByIdAsync(int id);

    Task<CityData?> CreateAsync(CityInfoDto cityInfoDto);

    public Task<int?> DeleteAsync(int id);
   
    Task<CityData?> UpdateAsync(CityInfoDto cityInfoDto);
    
}