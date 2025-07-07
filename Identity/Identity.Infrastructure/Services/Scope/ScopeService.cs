/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */
using Ardalis.Specification.EntityFrameworkCore;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Paging;
using BuildingBlocks.Specifications;
using Identity.Core.Features.Scope;
using Identity.Core.Features.Scope.Create;
using Identity.Core.Features.Scope.Search;
using Identity.Core.Features.Scope.Update;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Identity.Infrastructure.Services.Scope;

public class ScopeService(
    
    IOpenIddictScopeManager scopeManager, 
    IOpenIddictApplicationManager applicationManager) : IScopeService
{
    public async Task<IResult> CreateAsync (CreateScopeCommand request, CancellationToken cancellationToken)
    {
        if (await scopeManager.FindByNameAsync(request.Name, cancellationToken) is not null)
            throw new ConflictException($"Scope: {request.Name} have existed");
  
        var openIdScopeDescriptor = new ScopeDto(
           Id:  Guid.NewGuid().ToString(), 
           Name:   request.Name, 
           DisplayName: request.DisplayName, 
           Description:  request.Description, 
           Resources:request.Resources).ToModel();
                
        var result = await scopeManager.CreateAsync(openIdScopeDescriptor, cancellationToken)
            as OpenIddictEntityFrameworkCoreScope;

        return result == null ? Results.InternalServerError() : Results.Ok("Item created");
    }
    
    public async Task<ScopeDto> GetByIdAsync(string scopeId, CancellationToken cancellationToken)
    {
        var existing = await scopeManager.FindByIdAsync(scopeId, cancellationToken) 
                           as OpenIddictEntityFrameworkCoreScope
                       ?? throw new NotFoundException($"Scope with id : {scopeId}  doesn't exist");
                
        var scopeDescriptor = existing.ToDto();
             
        return scopeDescriptor;
    }
    
    public async Task<List<ScopeSummaryDto>> GetAllAsync (CancellationToken cancellationToken)
    {
        var scopeDescriptors = new List<ScopeSummaryDto>();

        Func<IQueryable<object>, IQueryable<OpenIddictEntityFrameworkCoreScope>> query 
            = sources => sources.Where(s => true)
                .Select(s => s as OpenIddictEntityFrameworkCoreScope)
                .OrderBy(s => s.Name)!;
            
        await foreach (var scope in scopeManager.ListAsync(query, cancellationToken))
        {
            var descriptor = scope.ToSummaryDto();
            scopeDescriptors.Add(descriptor);
        }
            
        return scopeDescriptors;
    }
    
    public  async Task<PagedList<ScopeSummaryDto>> SearchAsync(SearchScopesRequest request, CancellationToken cancellationToken)
    {
        var spec = new EntitiesByPaginationFilterSpec<OpenIddictEntityFrameworkCoreScope>(request);
            
        var scopeDescriptors = new List<ScopeSummaryDto>();

        Func<IQueryable<object>, IQueryable<OpenIddictEntityFrameworkCoreScope>> query;
            
        query = apps => apps.Where(app => true)
                                            .Select(s => s as OpenIddictEntityFrameworkCoreScope)
                                            .WithSpecification(spec)
                                            .OrderBy(s => s.Name);
            
        await foreach (var app in scopeManager.ListAsync(query, cancellationToken))
        {
            var descriptor = app.ToSummaryDto();
            scopeDescriptors.Add(descriptor);
        }

        var count = (int)await scopeManager.CountAsync(cancellationToken);
            
        return new PagedList<ScopeSummaryDto>(scopeDescriptors, request.PageNumber, request.PageSize, count);

    }
    
    public async Task<IResult> DeleteAsync(string scopeId, CancellationToken cancellationToken)
    {
        var existing = await scopeManager.FindByIdAsync(scopeId, cancellationToken) 
                       ?? throw new NotFoundException($"Scope with id : {scopeId}  doesn't exist");
                
        var descriptorFromExisting = new OpenIddictScopeDescriptor();
        await scopeManager.PopulateAsync(descriptorFromExisting, existing, cancellationToken);

        if (string.IsNullOrEmpty(descriptorFromExisting.Name)) throw new ConflictException("Invalid scope name ");

        var count = await applicationManager.CountAsync((Func<IQueryable<object>, IQueryable<OpenIddictEntityFrameworkCoreApplication>>)Query, CancellationToken.None);
        if (count > 0)
        {
            throw new ConflictException( $"Scope is in use by {count} applications so that can not be deleted" );
        }
                                
        await scopeManager.DeleteAsync(existing, cancellationToken);
                
        return Results.Ok();

        IQueryable<OpenIddictEntityFrameworkCoreApplication> Query(IQueryable<object> sources) =>
            sources.Where(s => (s as OpenIddictEntityFrameworkCoreApplication)!.Permissions.Contains(descriptorFromExisting.Name))
                .Select(s => s as OpenIddictEntityFrameworkCoreApplication);
    }
    
    public async Task<IResult> UpdateAsync(UpdateScopeCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Id)) return Results.BadRequest();
                
        var existing = await scopeManager.FindByIdAsync(request.Id, cancellationToken) 
                       ?? throw new NotFoundException($"Failed to find Scope with Id: {request.Id}");
                
        var descriptorFromExisting = new OpenIddictScopeDescriptor();
        await scopeManager.PopulateAsync(descriptorFromExisting, existing, cancellationToken);
                
        if (request.Name != descriptorFromExisting.Name 
            && await scopeManager.FindByNameAsync(request.Name, cancellationToken) is not null)
            throw new ConflictException($"Scope: {request.Name} have existed");

        var openIdScopeDescriptor = new ScopeDto(
            Id:  request.Id, 
            Name:   request.Name, 
            DisplayName: request.DisplayName, 
            Description:  request.Description, 
            Resources:request.Resources).ToModel();
                
        await scopeManager.UpdateAsync(existing, openIdScopeDescriptor, cancellationToken);
                
        return Results.Ok();
    }
}
