using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Cona40LiveChat.Pages;

public class Login : PageModel
{
    private readonly AuthSettings _authSettings;

    public Login(IOptions<AuthSettings> authSettings)
    {
        _authSettings = authSettings.Value;
    }
    
    public void OnGet()
    {
        
    }

    public async Task<IActionResult> OnPost(string password)
    {
        var salt = Convert.FromBase64String(_authSettings.PasswordSalt);
        var inputHash = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA256,
                iterationCount: 100_000,
                numBytesRequested: 32));

        if (!CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(inputHash),
                Convert.FromBase64String(_authSettings.PasswordHash)))
            return Page();

        var claims = new List<Claim> { new(ClaimTypes.Role, "Admin") };
        var identity = new ClaimsIdentity(claims, "LiveChatAuth");
        var principal = new ClaimsPrincipal(identity);
        
        var authProps = new AuthenticationProperties
        {
            IsPersistent  = true,
            AllowRefresh  = true,
            ExpiresUtc    = DateTimeOffset.UtcNow.AddHours(2)
        };
        
        await HttpContext.SignInAsync("LiveChatAuth", principal, authProps);

        return Redirect("/projection");
    }
}