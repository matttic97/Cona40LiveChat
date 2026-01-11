using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cona40LiveChat.Pages;

[Authorize(Roles = "Admin")]
public class Projection : PageModel
{
    public DateTimeOffset AuthExpiresUtc { get; private set; }
    
    public void OnGet()
    {
        var authResult = HttpContext.Features.Get<IAuthenticateResultFeature>()?.AuthenticateResult;

        AuthExpiresUtc = authResult?.Properties?.ExpiresUtc ?? DateTimeOffset.UtcNow;
    }
}