using Identity.Core.Features.Role;
using Identity.Core.Features.Role.CreateOrUpdateRole;
using Identity.Core.Features.Role.SearchRoles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Identity.Infrastructure.Endpoints.Role;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateOrUpdateRoleEndpoint();
        app.MapCreateRoleEndpoint();
        app.MapGetRoleByIdEndpoint();
        app.MapGetRoleByNameEndpoint();
        app.MapGetRolesEndpoint();
        app.MapSearchRolesEndpoint();
        app.MapUpdateRoleEndpoint();
        app.MapDeleteRoleEndpoint();
        
        return app;
    }
}

public static class GetRolesEndpoint
{
    public static RouteHandlerBuilder MapGetRolesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/", async (IRoleService service, CancellationToken cancellationToken) =>
                await service.GetAllAsync(cancellationToken))
            .WithName(nameof(GetRolesEndpoint))
            .WithSummary("Get a list of all roles")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Retrieve a list of all roles available in the system.");
    }
}

public static class GetRoleByIdEndpoint
{
    public static RouteHandlerBuilder MapGetRoleByIdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("{roleId}", async (string roleId, IRoleService service) 
                => await service.GetByIdAsync(roleId))
            .WithName(nameof(GetRoleByIdEndpoint))
            .WithSummary("Get role details with claims and permissions")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Retrieve the details of a role by its Id.");
    }
}

public static class GetRoleByNameEndpoint
{
    public static RouteHandlerBuilder MapGetRoleByNameEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/name/{roleName}", async (string roleName, IRoleService service) 
                => await service.GetByNameAsync(roleName))
            .WithName(nameof(GetRoleByNameEndpoint))
            .WithSummary("Get role details with claims and permissions")
            // .RequirePermission("Permissions.Handlers.View")
            .WithDescription("Retrieve the details of a role by its name.");
    }
}

public static class SearchRolesEndpoint
{
    internal static RouteHandlerBuilder MapSearchRolesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/search", (SearchRolesRequest request, IRoleService service, CancellationToken cancellationToken) =>
            {
                return service.SearchAsync(request, cancellationToken);
            })
            .WithName(nameof(SearchRolesEndpoint))
            .WithSummary("get a list of roles with paging support")
            // .RequirePermission("Permissions.Roles.Search")
            .WithDescription("get a list of roles with paging support");
    }
}

public static class DeleteRoleEndpoint
{
    public static RouteHandlerBuilder MapDeleteRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/{roleId:guid}", async (string roleId, IRoleService service) =>
            {
                await service.DeleteAsync(roleId);
            })
            .WithName(nameof(DeleteRoleEndpoint))
            .WithSummary("Remove a role by ID")
            // .RequirePermission("Permissions.Handlers.Remove")
            .WithDescription("Remove a role from the system by its ID.");
    }
}

public static class CreateOrUpdateRoleEndpoint
{
    public static RouteHandlerBuilder MapCreateOrUpdateRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/CreateAndUpdate", async (CreateOrUpdateRoleCommand request, IRoleService service) =>
            {
                return await service.CreateOrUpdateAsync(request);
            })
            .WithName(nameof(CreateOrUpdateRoleEndpoint))
            .WithSummary("Create or update a role")
            // .RequirePermission("Permissions.Handlers.Create")
            .WithDescription("Create a new role or update an existing role.");
    }
}

public static class CreateRoleEndpoint
{
    public static RouteHandlerBuilder MapCreateRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/", async (CreateRoleCommand request, IRoleService roleService) =>
            {
                return await roleService.CreateAsync(request);
            })
            .WithName(nameof(CreateRoleEndpoint))
            .WithSummary("Create a role")
            // .RequirePermission("Permissions.Handlers.Create")
            .WithDescription("Create a new role .");
    }
}

public static class UpdateRoleEndpoint
{
    public static RouteHandlerBuilder MapUpdateRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPut("/", async (UpdateRoleCommand request, IRoleService roleService) =>
            {
                return await roleService.UpdateAsync(request);
            })
            .WithName(nameof(UpdateRoleEndpoint))
            .WithSummary("Update a role")
            // .RequirePermission("Permissions.Handlers.Create")
            .WithDescription("Update an existing role.");
    }
}





