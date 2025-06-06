// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using Identity.Core.Dtos.Accounts;
// using Identity.Core.Dtos.Authentications;
// using Identity.Core.Entities;
// using Identity.Core.Exceptions;
// using Identity.Core.Interfaces;
// using Identity.Core.Settings;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
// using Microsoft.IdentityModel.Tokens;
// using OpenIddict.Abstractions;
//
// namespace Identity.Infrastructure.Services.Authentication;
//
// public class AuthenticationService(    UserManager<AppUser> userManager,
//     IOpenIddictApplicationManager applicationManager,
//     IOpenIddictScopeManager scopeManager,
//     IOptions<JwtSettings> jwtSettings,
//     ILogger<AuthenticationService> logger)
//     : IAuthenticationService
// {
//     private readonly IOpenIddictApplicationManager _applicationManager = applicationManager;
//     private readonly IOpenIddictScopeManager _scopeManager = scopeManager;
//     private readonly JwtSettings _jwtSettings = jwtSettings.Value;
//
//     public async Task<AuthenticationResult> LoginAsync(string email, string password)
//     {
//         var user = await userManager.FindByEmailAsync(email);
//         if (user == null)
//         {
//             logger.LogWarning("Login attempt failed for non-existent email: {Email}", email);
//             throw new NotFoundException($"User with email {email} not found");
//         }
//         if (!user.IsActive)
//         {
//             throw new UnauthorizedException("user is deactivated");
//         }
//
//         if (!user.EmailConfirmed)
//         {
//             throw new UnauthorizedException("user not yet is confirmed by email");
//         }
//         
//         if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
//             throw new UnauthorizedException($"Account is locked until {user.LockoutEnd.Value.ToLocalTime()}");
//
//         if (!await userManager.CheckPasswordAsync(user, password))
//         {
//             logger.LogWarning("Invalid password attempt for user: {Email}", email);
//             throw new UnauthorizedException("Invalid credentials");
//         }
//         
//         if (string.IsNullOrWhiteSpace(user.UserName)) user.UserName = user.Email;
//         user.LastLoginOn = DateTime.UtcNow;
//         user.IsOnline = true;
//         await userManager.UpdateAsync(user);
//
//         var result = await GenerateAuthenticationResultAsync(user);
//         logger.LogInformation("User logged in successfully: {Email}", email);
//
//         return result;
//
//     }
//
//     public async Task<AuthenticationResult> RegisterAsync(RegisterRequest request)
//     {
//         // Email kontrolü
//         var existingUser = await userManager.FindByEmailAsync(request.Email);
//         if (existingUser != null)
//         {
//             logger.LogWarning("Registration attempt with existing email: {Email}", request.Email);
//             throw new ValidationException("Email is already registered");
//         }
//
//         // Username kontrolü
//         existingUser = await userManager.FindByNameAsync(request.UserName);
//         if (existingUser != null)
//         {
//             logger.LogWarning("Registration attempt with existing username: {Username}", request.UserName);
//             throw new ValidationException("Username is already taken");
//         }
//
//         var user = new AppUser
//         {
//             UserName = request.UserName,
//             Email = request.Email,
//             FirstName = request.FirstName,
//             LastName = request.LastName,
//             IsActive = true,
//             CreatedOn = DateTime.UtcNow
//         };
//
//         var result = await userManager.CreateAsync(user, request.Password);
//         if (!result.Succeeded)
//         {
//             var errors = result.Errors.Select(e => e.Description);
//             logger.LogWarning("User registration failed: {Errors}", string.Join(", ", errors));
//             throw new ValidationException("Registration failed",
//                 result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description }));
//         }
//
//         // Varsayılan rol ataması
//         await userManager.AddToRoleAsync(user, Core.Constants.Roles.User);
//
//         var authResult = await GenerateAuthenticationResultAsync(user);
//         logger.LogInformation("User registered successfully: {Email}", request.Email);
//
//         return authResult;
//     }
//
//     public async Task<bool> LogoutAsync(string userId)
//     {
//         var user = await userManager.FindByIdAsync(userId);
//         if (user == null)
//         {
//             logger.LogWarning("Logout attempt for non-existent user: {UserId}", userId);
//             throw new NotFoundException($"User with id {userId} not found");
//         }
//
//         // Aktif refresh token'ları iptal et
//         // if (user.RefreshTokens?.Any() == true)
//         // {
//         //     foreach (var refreshToken in user.RefreshTokens.Where(t => t.IsActive))
//         //     {
//         //         refreshToken.RevokedDate = DateTime.UtcNow;
//         //         refreshToken.RevokedByIp = "localhost"; // Client IP'si eklenebilir
//         //     }
//         //
//         //     await _userManager.UpdateAsync(user);
//         // }
//
//         logger.LogInformation("User logged out successfully: {UserId}", userId);
//         
//         return true;
//     }
//
//     public Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task<bool> RevokeTokenAsync(string token)
//     {
//         throw new NotImplementedException();
//     }
//
//     // public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
//     // {
//     //
//     //     if (string.IsNullOrEmpty(refreshToken))
//     //         throw new ValidationException("Refresh token is required");
//     //
//     //     var user = await _userManager.Users
//     //         .Include(u => u.RefreshTokens)
//     //         .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken && t.IsActive));
//     //
//     //     if (user == null)
//     //     {
//     //         _logger.LogWarning("Refresh token attempt with invalid token: {Token}", refreshToken);
//     //         throw new UnauthorizedException("Invalid refresh token");
//     //     }
//     //
//     //     var existingToken = user.RefreshTokens.First(t => t.Token == refreshToken);
//     //     if (existingToken.IsExpired)
//     //     {
//     //         _logger.LogWarning("Attempt to use expired refresh token for user: {UserId}", user.Id);
//     //         throw new UnauthorizedException("Refresh token has expired");
//     //     }
//     //
//     //     var result = await GenerateAuthenticationResultAsync(user);
//     //     _logger.LogInformation("Token refreshed successfully for user: {UserId}", user.Id);
//     //
//     //     return result;
//     //     
//     // }
//     //
//     // public async Task<bool> RevokeTokenAsync(string token)
//     // {
//     //     if (string.IsNullOrEmpty(token)) throw new ValidationException("Token is required");
//     //
//     //     var user = await _userManager.Users
//     //         .Include(u => u.RefreshTokens)
//     //         .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
//     //
//     //     if (user == null) return true;
//     //
//     //     var refreshToken = user.RefreshToken;
//     //     refreshToken.RevokedDate = DateTime.UtcNow;
//     //     refreshToken.RevokedByIp = "localhost"; // Client IP'si eklenebilir
//     //
//     //     await _userManager.UpdateAsync(user);
//     //     _logger.LogInformation("Token revoked successfully for user: {UserId}", user.Id);
//     //
//     //     return true;
//     //
//     // }
//
//     public async Task<bool> ValidateTokenAsync(string token)
//     {
//
//             if (string.IsNullOrEmpty(token))
//                 throw new ValidationException("Token is required");
//
//             var tokenHandler = new JwtSecurityTokenHandler();
//             var key = Encoding.ASCII.GetBytes(_jwtSettings.SecurityKey);
//
//             tokenHandler.ValidateToken(token, new TokenValidationParameters
//             {
//                 ValidateIssuerSigningKey = true,
//                 IssuerSigningKey = new SymmetricSecurityKey(key),
//                 ValidateIssuer = true,
//                 ValidateAudience = true,
//                 ValidIssuer = _jwtSettings.Issuer,
//                 ValidAudience = _jwtSettings.Audience,
//                 ClockSkew = TimeSpan.Zero
//             }, out _);
//
//             logger.LogInformation("Token validated successfully");
//             return true; 
//     }
//     
//
//     private async Task<AuthenticationResult> GenerateAuthenticationResultAsync(AppUser user)
//     {
//         try
//         {
//             var claims = new List<Claim>
//             {
//                 new Claim(OpenIddictConstants.Claims.Subject, user.Id.ToString()),
//                 new Claim(OpenIddictConstants.Claims.Email, user.Email??string.Empty),
//                 new Claim(OpenIddictConstants.Claims.Name, user.UserName??string.Empty),
//             };
//
//             if (!string.IsNullOrEmpty(user.FirstName))
//                 claims.Add(new Claim(OpenIddictConstants.Claims.GivenName, user.FirstName));
//
//             if (!string.IsNullOrEmpty(user.LastName))
//                 claims.Add(new Claim(OpenIddictConstants.Claims.FamilyName, user.LastName));
//
//             var roles = await userManager.GetRolesAsync(user);
//             foreach (var role in roles)
//             {
//                 claims.Add(new Claim(OpenIddictConstants.Claims.Role, role));
//             }
//
//             if (string.IsNullOrEmpty(_jwtSettings.SecurityKey))
//                 throw new InvalidOperationException("JWT Security Key is not configured.");
//
//             var tokenHandler = new JwtSecurityTokenHandler();
//             var key = Encoding.ASCII.GetBytes(_jwtSettings.SecurityKey);
//
//             var tokenDescriptor = new SecurityTokenDescriptor
//             {
//                 Subject = new ClaimsIdentity(claims, OpenIddictConstants.Schemes.Bearer),
//                 Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
//                 SigningCredentials = new SigningCredentials(
//                     new SymmetricSecurityKey(key),
//                     SecurityAlgorithms.HmacSha256Signature
//                 ),
//                 Issuer = _jwtSettings.Issuer,
//                 Audience = _jwtSettings.Audience
//             };
//
//             var token = tokenHandler.CreateToken(tokenDescriptor);
//             var refreshToken = await GenerateRefreshTokenAsync(user);
//
//             return new AuthenticationResult
//             {
//                 Succeeded = true,
//                 Token = tokenHandler.WriteToken(token),
//                 RefreshToken = refreshToken,
//                 ExpiresAt = tokenDescriptor.Expires
//             };
//         }
//         catch (Exception ex)
//         {
//             logger.LogError(ex, "Error generating authentication result for user: {UserId}", user.Id);
//             throw new CustomException("Error generating authentication token", "TokenGenerationError");
//         }
//     }
//
//     private async Task<string> GenerateRefreshTokenAsync(AppUser user)
//     {
//         var refreshToken = new RefreshToken
//         {
//             Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
//             ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays),
//             CreatedDate = DateTime.UtcNow,
//             CreatedByIp = "localhost" // Client IP'si eklenebilir
//         };
//
//        // user.RefreshTokens.Add(refreshToken);
//         await userManager.UpdateAsync(user);
//
//         return refreshToken.Token;
//     }
// }