using CleanArch.Domain.Entities;

namespace CleanArch.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}
