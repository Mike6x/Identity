using System.Collections.ObjectModel;
using Ardalis.Specification.EntityFrameworkCore;
using BuildingBlocks.DataIO;
using BuildingBlocks.Identity.Users.Dtos;
using BuildingBlocks.Mail;
using BuildingBlocks.Specifications;
using BuildingBlocks.Storage.File;
using BuildingBlocks.Storage.File.Features;
using Identity.Core.Entities;
using Identity.Core.Features.User.ExportUsers;
using Identity.Shared.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.User;

public partial class UserService
{
    public async Task<byte[]> ExportAsync(ExportUsersRequest request, CancellationToken cancellationToken)
    {
        var spec = new EntitiesByBaseFilterSpec<AppUser>(request);

        var list = await Mapster.Extensions
            .ProjectToType<UserExportDto>(userManager.Users
                .WithSpecification(spec))
            .ToListAsync(cancellationToken);

        return dataExport.ListToByteArray(list);
    }

    public async Task<ImportResponse> ImportAsync(FileUploadCommand uploadFile, bool isUpdate, string origin, CancellationToken cancellationToken)
    {
        var items = await dataImport.ToListAsync<AppUser>(uploadFile, FileType.Excel);

        ImportResponse response = new()
        {
            TotalRecords = items.Count,
            Message = ""
        };

        if (response.TotalRecords <= 0)
        {
            response.Message = "File is empty or Invalid format";
            return response;
        }

        int count = 0;
        try
        {
            if (isUpdate)
            {
                foreach (var item in items)
                {
                    var user = await userManager.FindByIdAsync(item.Id.ToString());
                    if (user != null)
                    {
                        user.FirstName = item.FirstName;
                        user.LastName = item.LastName;
                        user.UserName = item.UserName;
                        user.PhoneNumber = item.PhoneNumber;
                        user.IsActive = item.IsActive;
                        user.EmailConfirmed = item.IsActive && item.EmailConfirmed;

                        _ = await userManager.UpdateAsync(user);
                        count++;
                    }
                }

                response.Message = $"Updated {count} Users successfully";
            }
            else
            {
                foreach (var item in items)
                {
                    var result = await userManager.CreateAsync(item, item.UserName!);
                    if (result.Succeeded)
                    {
                        count++;
                        // add basic role
                        _ = await userManager.AddToRoleAsync(item, AppRoles.Basic);

                        // send confirmation mail
                        if (!string.IsNullOrEmpty(item.Email))
                        {
                            string emailVerificationUri = await GetEmailVerificationUriAsync(item, origin);
                            var mailRequest = new MailRequest(
                                new Collection<string> { item.Email },
                                "Confirm Registration",
                                emailVerificationUri);
                            _ = jobService.Enqueue("email", () => mailService.SendAsync(mailRequest, CancellationToken.None));
                        }
                    }
                }

                response.Message = $"Imported {count} Users successfully";
            }
        }
        catch (Exception)
        {
            response.Message = $"Internal error with {count} items!";
            return response;
        }

        return response;
    }
}
