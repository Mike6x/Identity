/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */
using Identity.Core.Features.Scope;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Identity.Infrastructure.Services.Scope;

public static class ScopeMapping
{
    public static ScopeDto ToDto(this OpenIddictEntityFrameworkCoreScope source)
    {
        var resources = string.IsNullOrWhiteSpace(source.Resources)
            ? []
            : JsonConvert.DeserializeObject<IEnumerable<string>>(source.Resources)!.ToHashSet();
        
        return new ScopeDto
        (
            Id: source.Id ?? string.Empty,
            Name: source.Name ?? string.Empty,
            DisplayName: source.DisplayName ?? string.Empty,
            Description: source.Description ?? string.Empty,
            Resources: resources
        );
    }

    public static OpenIddictScopeDescriptor ToModel(this ScopeDto source)
    {
        var destination = new OpenIddictScopeDescriptor
        {
            Name = source.Name,
            DisplayName = source.DisplayName,
            Description = source.Description,

        };

        if (source.Resources is { Count: > 0 })
            foreach (var resource in source.Resources)
            {
                destination.Resources.Add(resource);
            }

        return destination;
    }
}
