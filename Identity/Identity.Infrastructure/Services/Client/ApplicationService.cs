using Ardalis.Specification.EntityFrameworkCore;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Paging;
using BuildingBlocks.Specifications;
using Identity.Core.Features.Client;
using Identity.Core.Features.Client.Create;
using Identity.Core.Features.Client.Search;
using Identity.Core.Features.Client.Update;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Identity.Infrastructure.Services.Client;

public class ClientService(
    IOpenIddictApplicationManager applicationManager,
    IOptions<CorsOptions> corsOptions) : IClientService
{
    
    public async Task<IResult> CreateAsync(CreateClientCommand request, CancellationToken cancellationToken)
    {
     
        if (await applicationManager.FindByClientIdAsync(request.ClientId, cancellationToken) is not null)
            throw new ConflictException($"Application: {request.ClientId} have existed");
                
        var openIdApplicationDescriptor = request.ToModel();
                
        if(request.RedirectUris != null && request.RedirectUris.Count != 0)
        {
            OriginHelper.AllowOriginsAsync(request.RedirectUris, corsOptions);
        }

        var result = await applicationManager.CreateAsync(openIdApplicationDescriptor, cancellationToken)
            as OpenIddictEntityFrameworkCoreApplication;

        return result == null ? Results.InternalServerError() : Results.Ok($"Item created: {result.Id}");

    }
    
    public async Task<ApplicationDto> GetByNameAsync(string clientId, CancellationToken cancellationToken)
    {
        var existing = await applicationManager.FindByClientIdAsync(clientId, cancellationToken) 
                           as OpenIddictEntityFrameworkCoreApplication
                       ?? throw new NotFoundException($"Failed to find application: {clientId}");
                
        var applicationDescriptor = existing.ToDto();
        return applicationDescriptor;
    }
    
    public async Task<ApplicationDto> GetAsync(string id, CancellationToken cancellationToken)
    {
        var existing = await applicationManager.FindByIdAsync(id, cancellationToken) 
                           as OpenIddictEntityFrameworkCoreApplication
                       ?? throw new NotFoundException($"Failed to find application: {id}");
                
        var applicationDescriptor = existing.ToDto();
        return applicationDescriptor;
    }
    
    public async Task<List<ApplicationDto>>GetAllAsync(CancellationToken cancellationToken)
    {
        List<ApplicationDto> applicationDescriptors = [];

        Func<IQueryable<object>, IQueryable<OpenIddictEntityFrameworkCoreApplication>> query;
                
        query = sources => sources.Where(app => true)
            .Select(s => s as OpenIddictEntityFrameworkCoreApplication)
            .OrderBy(s => s.ClientId);
                
        await foreach (var app in applicationManager.ListAsync(query, cancellationToken))
        {
            var applicationDescriptor = app.ToDto();
            applicationDescriptors.Add(applicationDescriptor);
        }
        
        return applicationDescriptors;
    }
    
    public async Task<PagedList<ApplicationDto>> SearchAsync (SearchClientsRequest request, CancellationToken cancellationToken)
    {
        var spec = new EntitiesByPaginationFilterSpec<OpenIddictEntityFrameworkCoreApplication>(request);
                
        List<ApplicationDto> applicationDescriptors = [];

        Func<IQueryable<object>, IQueryable<OpenIddictEntityFrameworkCoreApplication>> query;
                
        query = apps => apps.Where(app => true)
            .Select(s => s as OpenIddictEntityFrameworkCoreApplication)
            .WithSpecification(spec)
            .OrderBy(s => s.ClientId);
                
        await foreach (var app in applicationManager.ListAsync(query, cancellationToken))
        {
            var applicationDescriptor = app.ToDto();
            //applicationDescriptor.ClientSecret = string.Empty
            applicationDescriptors.Add(applicationDescriptor);
        }

        var count = (int)await applicationManager.CountAsync(cancellationToken);
                
        return new PagedList<ApplicationDto>(applicationDescriptors, request.PageNumber, request.PageSize, count);

    }
    
    public async Task<IResult> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var existing = await applicationManager.FindByIdAsync(id, cancellationToken) 
                           as OpenIddictEntityFrameworkCoreApplication
                       ?? throw new NotFoundException($"Failed to find application: {id}");
                
        await applicationManager.DeleteAsync(existing, cancellationToken);
                
        var descriptor = existing.ToDto();
        if (descriptor.RedirectUris != null && descriptor.RedirectUris.Count != 0)
        {
            OriginHelper.RemoveOriginsAsync(descriptor.RedirectUris, corsOptions);
        }
                
        return Results.Ok();
    }
    
    public async Task<IResult> UpdateAsync (UpdateClientCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.ClientId)) return Results.BadRequest();
        
        var existing = await applicationManager.FindByIdAsync(request.Id, cancellationToken) 
                       ?? throw new NotFoundException($"Failed to find application with Id: {request.Id}");
        
        var descriptorFromExisting = new OpenIddictApplicationDescriptor();
        await applicationManager.PopulateAsync(descriptorFromExisting, existing, cancellationToken);
        
        if (request.ClientId != descriptorFromExisting.ClientId 
            && await applicationManager.FindByClientIdAsync(request.ClientId, cancellationToken) is not null)
            throw new ConflictException($"Application: {request.ClientId} have existed");

        var openIdApplicationDescriptor = request.ToModel();
        
        //No new secret to update. Populate existing on descriptor before updating
        if (request.IsConfidentialClient)
        {                     
            if(string.IsNullOrEmpty(request.ClientSecret))
            {
                openIdApplicationDescriptor.ClientSecret = descriptorFromExisting.ClientSecret;
            }
            if(string.IsNullOrEmpty(request.JsonWebKeySet))
            {
                openIdApplicationDescriptor.JsonWebKeySet = descriptorFromExisting.JsonWebKeySet;
            }
        }  
        
        if(!openIdApplicationDescriptor.RedirectUris.SequenceEqual(descriptorFromExisting.RedirectUris))
        {
            OriginHelper.RemoveOriginsAsync(descriptorFromExisting.RedirectUris, corsOptions);
            if (request.RedirectUris != null) OriginHelper.AllowOriginsAsync(request.RedirectUris, corsOptions);
        }
        
        await applicationManager.UpdateAsync(existing, openIdApplicationDescriptor, cancellationToken);
        
        return Results.Ok();

    }
    
    //https://github.com/legimenes/articles-OpenIddictAuthorizationServer/blob/main/src/AuthorizationServer/Endpoints/ApplicationEndpoint.cs
    public async Task<IResult> CallbackAsync(HttpContext httpContext, [FromServices] IHttpClientFactory httpClientFactory)
    {
        IEnumerable<KeyValuePair<string, StringValues>> parameters = httpContext.Request.HasFormContentType ?
            httpContext.Request.Form : httpContext.Request.Query;

        Dictionary<string, string> formData = new()
        {
            { "grant_type", "authorization_code" },
            { "code_verifier", "AVA~cbYg_UDgPYrJNJX.kMotv0x.z8nY~C23XzWq4DxEUu0cw9rWk6SwlgHgihmBoPN4.WKV0H1ui6TTL3vCWC0jyv7fYlAef3Z-y-7rgC6~0m9bb06x8FEO24LJArH4" },
            { "client_id", "test_client" },
            { "client_secret", "test_secret" },
            { "redirect_uri", "https://localhost:4001/callback" }
        };
        var codeParameter = parameters.First(p => p.Key == "code");

        formData.Add(codeParameter.Key, codeParameter.Value);

        var httpClient = httpClientFactory.CreateClient("TokenApiClient");
        FormUrlEncodedContent content = new(formData);
        var response = await httpClient.PostAsync("connect/token", content);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        dynamic? jsonObject = JsonConvert.DeserializeObject<dynamic>(responseContent);

        return Results.Json(jsonObject);
    }
}
