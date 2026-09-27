namespace Application.Identity.IssueUserToken;

public sealed record IssueUserTokenResult(string AccessToken, DateTime ExpiresAtUtc);