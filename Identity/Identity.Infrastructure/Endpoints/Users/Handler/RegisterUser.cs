// using Identity.Core.Entities;
// using Identity.Shared.Models;
// using Microsoft.AspNetCore.Identity;
//
// namespace Identity.AuthServer.EndPoints.Users.Handler;
//
// public static class RegisterUser
// {
//     public static async Task<IResult> Handler(RegisterInputModel model, UserManager<AppUser> userManager)
//     {
//                
//         var user = await userManager.FindByNameAsync(model.Email);
//         
//         if (user != null) return Results.BadRequest("User already exists.");
//
//         user = new AppUser { UserName = model.Email, Email = model.Email };
//         
//         var result = await userManager.CreateAsync(user, model.Password);
//         
//         return result.Succeeded ? Results.Ok($"User with email{ model.Email} created.") : Results.BadRequest();
//     }
//     
// }