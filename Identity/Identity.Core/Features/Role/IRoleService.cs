using BuildingBlocks.Paging;
using Identity.Core.Features.Claim;
using Identity.Core.Features.Claim.Add;
using Identity.Core.Features.Claim.Change;
using Identity.Core.Features.Claim.Remove;
using Identity.Core.Features.Claim.Update;
using Identity.Core.Features.Role.CreateOrUpdateRole;
using Identity.Core.Features.Role.SearchRoles;
using Identity.Core.Features.Role.UpdatePermissions;

namespace Identity.Core.Features.Role;

public interface IRoleService
{
    Task<List<RoleDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<RoleDto?> GetAsync(string roleId);
    Task<RoleDto?> GetByNameAsync(string name);
    Task<PagedList<RoleDto>> SearchAsync(SearchRolesRequest request, CancellationToken cancellationToken);
    
    Task<RoleDto> CreateAsync(CreateRoleCommand request);
    Task<RoleDto> UpdateAsync(UpdateRoleCommand request);
    Task<RoleDto> CreateOrUpdateAsync(CreateOrUpdateRoleCommand request);

    Task DeleteAsync(string roleId);

    #region Claim Role
    
    Task<List<ClaimViewModel>> GetRoleClaimsAsync(string roleId, CancellationToken cancellationToken);
    Task<bool> AddClaimToRoleAsync(string roleId, AddClaimCommand request, CancellationToken cancellationToken);
    Task<bool> RemoveClaimOfRoleAsync(string roleId, RemoveClaimCommand request, CancellationToken cancellationToken);
    Task<bool> ChangeClaimOfRoleAsync(string roleId,ChangeClaimCommand request, CancellationToken cancellationToken);
    Task<string> UpdateClaimsToRoleAsync(string roleId, AssignClaimsCommand request,CancellationToken cancellationToken);

    Task<string> AssignClaimsToRoleAsync(string roleId, AssignClaimsCommand request,
        CancellationToken cancellationToken);
    #endregion
    
    #region Permission Role

    // Task<List<string>> GetRolePermissionsAsync(string roleId, CancellationToken cancellationToken);
    Task<string> UpdatePermissionsToRoleAsync(UpdatePermissionsCommand request);

    #endregion

}
