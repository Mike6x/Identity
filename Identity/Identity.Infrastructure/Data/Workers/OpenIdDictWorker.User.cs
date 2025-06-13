using Identity.Core.Entities;
using Identity.Core.Settings;
using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Data.Workers;

public partial class OpenIdDictWorker
{
    private async Task SeedUsersAsync(IServiceScope scope)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        // if (userManager.Users.Any()) return;

        var loadingList = configuration.GetSection("OpenIdDict:Users").Get<IEnumerable<UserConfig>>() ?? new List<UserConfig>();
        var seedingList = new UserCollection().GetAll().ToList();
        seedingList.AddRange(loadingList);

        foreach (var userConfig in seedingList )
        {
            var existingUser = await userManager.FindByNameAsync(userConfig.Username);
            if(existingUser != null) continue;
            
            var user = new AppUser
            {
                UserName = userConfig.Username,
                FirstName = userConfig.Username,
                Email = userConfig.Email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true,
                CreatedOn = DateTime.UtcNow
            };
            if (string.IsNullOrWhiteSpace(userConfig.Password))
            {
                userConfig.Password = Guid.NewGuid().ToString();
                //add 3 random upper case letters
                userConfig.Password += new string(Enumerable.Range(0, 3).Select(_ => (char)Random.Shared.Next('A', 'Z')).ToArray());
                Console.WriteLine($"Creating user {userConfig.Email} with password '{userConfig.Password}'");
            }
            var result = await userManager.CreateAsync(user, userConfig.Password);
            if (result.Succeeded)
            {
                if (!AppRoles.IsDefault(userConfig.Role)) userConfig.Role = AppRoles.Basic;
                
                await userManager.AddToRoleAsync(user, userConfig.Role);
            }
            Console.WriteLine($"Creating user {userConfig.Email}");
        }
    }
}