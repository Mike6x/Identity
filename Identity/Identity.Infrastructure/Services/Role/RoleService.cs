using Ardalis.Specification.EntityFrameworkCore;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Identity.Users.Dtos;
using BuildingBlocks.Paging;
using BuildingBlocks.Specifications;
using Identity.Core.Entities;
using Identity.Core.Features.Claim;
using Identity.Core.Features.Role;
using Identity.Core.Features.Role.CreateOrUpdateRole;
using Identity.Core.Features.Role.SearchRoles;
using Identity.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.Role;

public sealed partial class RoleService(
    RoleManager<AppRole> roleManager,
    UserManager<AppUser> userManager
    ) : IRoleService
{
    public async Task<List<RoleSummaryDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await roleManager.Roles.AsNoTracking()
            .Select(role => role.ToSummaryDto())
            .ToListAsync(cancellationToken);
    }
    public async Task<PagedList<RoleSummaryDto>> SearchAsync(SearchRolesRequest request, CancellationToken cancellationToken)
    {
        var spec = new EntitiesByPaginationFilterSpec<AppRole>(request);

        var roles = await roleManager.Roles
            .WithSpecification(spec)
            .ProjectToType<RoleSummaryDto>()
            .ToListAsync(cancellationToken);

        var count = await roleManager.Roles.CountAsync(cancellationToken);

        return new PagedList<RoleSummaryDto>(roles, request.PageNumber, request.PageSize, count);
    }
    public async Task<RoleDto?> GetByIdAsync(string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId) 
                   ?? throw new NotFoundException($"Role with Id: {roleId} could not be located");

        var roleDto = role.ToDto();
        
        var claims = await roleManager.GetClaimsAsync(role);
        foreach (var claim in claims)
        {
            roleDto.Claims.Add(ClaimViewModel.FromClaim(claim));
            if(claim.Type == AppClaims.Permission) roleDto.Permissions.Add(claim.Value);
        }
        
        return roleDto;
    }

    public async Task<RoleDto?> GetByNameAsync(string name)
    {
        var role = await roleManager.FindByNameAsync(name) 
                   ?? throw new NotFoundException($"Role with Id: {name} not found");
        
        var roleDto = role.ToDto();
       
        var claims = await roleManager.GetClaimsAsync(role);
        foreach (var claim in claims)
        {
            roleDto.Claims.Add(ClaimViewModel.FromClaim(claim));
            if(claim.Type == AppClaims.Permission) roleDto.Permissions?.Add(claim.Value);
        }
        
        return roleDto;
    
    }

    public async Task<RoleDto> CreateOrUpdateAsync(CreateOrUpdateRoleCommand request)
    {
        var role = await roleManager.FindByIdAsync(request.Id);

        if (role != null)
        {
            role.Name = request.Name;
            role.Description = request.Description ?? string.Empty;
            await roleManager.UpdateAsync(role);
        }
        else
        {
            role = new AppRole(request.Name, request.Description);
            await roleManager.CreateAsync(role);
        }

        return new RoleDto { Id = role.Id, Name = role.Name ?? string.Empty, Description = role.Description };
    }
    public async Task<RoleDto> CreateAsync(CreateRoleCommand request)
    {
        var role = await roleManager.FindByNameAsync(request.Name);

        if (role != null) throw new ConflictException($"Role with Name: {request.Name} already existed.");

        role = new AppRole(request.Name, request.Description);
        
        var result = await roleManager.CreateAsync(role);
        
        if (!result.Succeeded) throw new InternalServerException("CreateRoleAsync failed. ");
        
        role = await roleManager.FindByNameAsync(request.Name) ??  throw new InternalServerException("Internal error. ");
        foreach (var userClaim in request.Claims)
        {
            await roleManager.AddClaimAsync(role, userClaim.ToClaim());
        }
        
        return  new RoleDto
        {
            Id = role.Id, 
            Name = role.Name?? string.Empty, 
            Description = role.Description,
            Claims = request.Claims
        };
    }
    public async Task<RoleDto> UpdateAsync(UpdateRoleCommand request)
    {
        var role = await roleManager.FindByIdAsync(request.Id)
            ?? throw new NotFoundException($"Role with Id: {request.Id} could not be located");
        
        var exists = await roleManager.FindByNameAsync(request.Name);
        
        if(exists != null) throw new ConflictException($"Role: {request.Name} already existed.");
        
        role.Name = request.Name;
        role.Description = request.Description ?? string.Empty;
        var result = await roleManager.UpdateAsync(role);
       
        if (!result.Succeeded) throw new InternalServerException("CreateRoleAsync failed. ");

        return role.ToDto();
    }
    
    public async Task<bool> DeleteAsync(string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId) ?? throw new NotFoundException($"Role with Id: {roleId} not found");
        
        var users = await userManager.GetUsersInRoleAsync(role.Name ?? string.Empty);
        
        if (users.Any()) throw new ConflictException($"Role: {role.Name} is in use so can not be deleted.");

        var result =  await roleManager.DeleteAsync(role);
        
        return result.Succeeded;
    }

}
