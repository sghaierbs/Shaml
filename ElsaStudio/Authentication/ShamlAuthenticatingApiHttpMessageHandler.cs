using System.Net.Http.Headers;

namespace ElsaStudio.Authentication;

public sealed class ShamlAuthenticatingApiHttpMessageHandler
    : DelegatingHandler
{
    private readonly IShamlTokenAccessor _tokenAccessor;

    public ShamlAuthenticatingApiHttpMessageHandler(
        IShamlTokenAccessor tokenAccessor)
    {
        _tokenAccessor = tokenAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = _tokenAccessor.GetToken();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}