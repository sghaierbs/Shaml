using Microsoft.AspNetCore.Http;

namespace ElsaStudio.Authentication;

public interface IShamlTokenAccessor
{
    string? GetToken();
}

public sealed class ShamlTokenAccessor : IShamlTokenAccessor
{
    private const string CookieName = "ShamlElsaToken";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public ShamlTokenAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetToken()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null)
            return null;

        return httpContext.Request.Cookies.TryGetValue(
            CookieName,
            out var token)
            ? token
            : null;
    }
}