using Application.Common.Interfaces;

namespace Application.Identity.Tokens;

public interface ITokenService
{
    TokenResult IssueToken(ICurrentUserContext context, IReadOnlyCollection<string> permissions);
}