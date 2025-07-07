using BuildingBlocks.Exceptions;
using BuildingBlocks.Identity.Users.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.User
{
    public partial class UserService
    {
        #region User Query Section
        
        public Task<int> GetCountAsync(CancellationToken cancellationToken)
        {
            return userManager.Users.AsNoTracking().CountAsync(cancellationToken);
        }
        
        public async Task<bool> ExistsWithEmailAsync(string email, Guid? exceptId = null)
        {
            EnsureValidTenant();
            return await userManager.FindByEmailAsync(email.Normalize()) is { } user && user.Id!= exceptId;
        }
        
        public async Task<bool> ExistsWithNameAsync(string name)
        {
            EnsureValidTenant();
            return await userManager.FindByNameAsync(name) is not null;
        }

        public async Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber, Guid? exceptId = null)
        {
            EnsureValidTenant();
            return await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber) is { } user && user.Id != exceptId;
        }
        
        
        public async Task<UserDto> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            var user = await userManager.Users
                    .AsNoTracking()
                    .Where(u => u.UserName == name)
                    .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new NotFoundException($"User with username: {name} not found!");
                       
            return await GetUserDetailsAsync(user, cancellationToken);
        }

        public async Task<UserDto> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var user = await userManager.Users
                    .AsNoTracking()
                    .Where(u => u.Email == email)
                    .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new NotFoundException($"User with email address: {email} not found!");

            return await GetUserDetailsAsync(user, cancellationToken);
        }

        public async Task<UserDto> GetByPhoneAsync(string phone, CancellationToken cancellationToken)
        {
            var user = await userManager.Users
                .AsNoTracking()
                .Where(u => u.PhoneNumber == phone)
                    .FirstOrDefaultAsync(cancellationToken);

            if(user == null) throw new NotFoundException($"User with phone number: {phone} not found!");

            return await GetUserDetailsAsync(user, cancellationToken);
        }

        #endregion
    }
}
