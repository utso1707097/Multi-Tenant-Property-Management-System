using Domus.Infrastructure.Auth;

namespace Domus.Application.Common.Interfaces;

public interface ITokenService
{
  (string Token, DateTimeOffset ExpiresAt) CreateAccessToken(
      ApplicationUser user,
      IEnumerable<string> roles);

  string CreateRefreshToken();
}