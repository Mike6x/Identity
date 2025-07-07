using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using BuildingBlocks.Exceptions;
using Identity.Core.Entities;
using Identity.Core.Features.Authenticator;
using Identity.Core.Models;
using Identity.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Services.Authenticator;

public class AuthenticatorService(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager): IAuthenticatorService
{
    /// <summary>
    /// Gets whether authenticator is enabled for the user account
    /// </summary>
    /// <returns></returns>
    public async Task<bool> IsAuthenticatorEnabledAsync(HttpContext context)
    {
        var currentUser = context.User;
        
        var user = await userManager.GetUserAsync(currentUser) 
                   ?? throw new NotFoundException($"Unable to load user with ID '{userManager.GetUserId(currentUser)}'.");

        var isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user);
        
        return isTwoFactorEnabled;
    }
    
    /// <summary>
    /// Get the details required for setting up authenticator for the user
    /// </summary>
    /// <returns></returns>
    public async Task<EnableAuthenticatorModel> RetrieveAuthenticatorConfigAsync(HttpContext context)
    {
        var currentUser = context.User;
        
        var user = await userManager.GetUserAsync(currentUser) 
                   ?? throw new NotFoundException($"Unable to load user with ID '{userManager.GetUserId(currentUser)}'.");
        
        var sharedKeyAndQrCode = await LoadSharedKeyAndQrCodeUriAsync(user);
          
        return new EnableAuthenticatorModel
        {
            SharedKey = sharedKeyAndQrCode.Item1,
            AuthenticatorUri = sharedKeyAndQrCode.Item2
        };
    }
    
    /// <summary>
    /// Generate new recovery codes for the authenticator
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<string>?> GenerateRecoveryCodesAsync(HttpContext context)
    {
        var currentUser = context.User;
        
        var user = await userManager.GetUserAsync(currentUser) 
                   ?? throw new NotFoundException($"Unable to load user with ID '{userManager.GetUserId(currentUser)}'.");

        var isTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user);
        if (!isTwoFactorEnabled)
            throw new BadRequestException("2FA is not enabled for account." );                
        
        var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
    
        return recoveryCodes;
    }

    /// <summary>
    /// Get the number of remaining recovery codes 
    /// </summary>
    /// <returns></returns>
    public async Task<int> CountActiveRecoveryCodesAsync(HttpContext context)
    {
        var currentUser = context.User;
        
        var user = await userManager.GetUserAsync(currentUser) 
                   ?? throw new NotFoundException($"Unable to load user with ID '{userManager.GetUserId(currentUser)}'.");
        
        var recoveryCodesCount = await userManager.CountRecoveryCodesAsync(user);
        
        return recoveryCodesCount;
    }
    
    /// <summary>
    /// Enable the authenticator by verifying the provided Code from the authenticator app
    /// </summary>
    /// <returns></returns>
    public async Task<IResult> EnableAuthenticatorAsync(string code, HttpContext context)
    {
        return await SetUser2FaStatus(code,  true, false, context);
    }

    /// <summary>
    /// Disable authenticator for the user account
    /// </summary>
    /// <returns></returns>
    public async Task<IResult> DisableAuthenticatorAsync(string code, HttpContext context)
    {
        return await SetUser2FaStatus(code, false, false, context);
    }

    /// <summary>
    /// Disable 2FA and reset authenticator keys. User will need to set up an authenticator again.
    /// </summary>
   /// <returns></returns>
    public async Task<IResult> ResetAuthenticatorAsync(string code, HttpContext context)
    {
        return await SetUser2FaStatus(code, false, true, context);
    
    }
    
    private async Task<IResult> SetUser2FaStatus(string code, bool is2FaEnabled,  bool isReset, HttpContext context)
    {
        if (string.IsNullOrEmpty(code)) 
            throw new BadRequestException("Verification code can not be null or empty." );
        
        var currentUser = context.User;
        
        var user = await userManager.GetUserAsync(currentUser) 
                   ?? throw new NotFoundException($"Unable to load user with ID '{userManager.GetUserId(currentUser)}'.");
        
        // Strip spaces and hyphens
        var verificationCode = code.Replace(" ", string.Empty).Replace("-", string.Empty);

        var is2FaTokenValid = await userManager.VerifyTwoFactorTokenAsync(user,
            userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!is2FaTokenValid)  
            throw new BadRequestException("Verification code is invalid." );
        
        var settingResult = await userManager.SetTwoFactorEnabledAsync(user, is2FaEnabled);
        if (!settingResult.Succeeded)
        {
            throw new BadRequestException(string.Join(Environment.NewLine, settingResult.GetErrors()));
        }

        if (isReset)
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            await userManager.GetUserIdAsync(user);         
            await signInManager.RefreshSignInAsync(user);
        }
        
        return Results.Ok(); 
    }
    
    private async Task<(string,string)> LoadSharedKeyAndQrCodeUriAsync(AppUser user)
    {
        // Load the authenticator key & QR code URI to display on the form
        var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
        }
          
        var email = await userManager.GetEmailAsync(user);
        (string, string) sharedKeyAndQrCode =  (FormatKey(unformattedKey), GenerateQrCodeUri(email, unformattedKey));
        return sharedKeyAndQrCode;
    }
    
    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
    
    private  readonly UrlEncoder _urlEncoder = UrlEncoder.Default;
    private string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        int currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }
        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition));
        }

        return result.ToString().ToLowerInvariant();
    }

    private string GenerateQrCodeUri(string email, string unformattedKey)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            AuthenticatorUriFormat,
            _urlEncoder.Encode("Pixel-Identity"),
            _urlEncoder.Encode(email),
            unformattedKey);
    }
    
}