namespace Application.Identity.Tokens;

public sealed record TokenResult(
    string AccessToken,
    DateTime ExpiresAtUtc);