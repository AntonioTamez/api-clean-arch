using CleanArch.Application.Auth.DTOs;
using CleanArch.Application.Common.Interfaces;
using CleanArch.Application.Common.Models;
using CleanArch.Domain.Interfaces;
using MediatR;

namespace CleanArch.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Buscar usuario
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        
        if (user == null)
            return Result<LoginResponseDto>.Failure(new Error("Login.InvalidCredentials", "Invalid username or password"));

        // Verificar password
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Result<LoginResponseDto>.Failure(new Error("Login.InvalidCredentials", "Invalid username or password"));

        // Verificar si está activo
        if (!user.IsActive)
            return Result<LoginResponseDto>.Failure(new Error("Login.UserInactive", "User account is inactive"));

        // Registrar login
        user.RecordLogin();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generar tokens
        var token = _jwtTokenService.GenerateToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var response = new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Roles = user.Roles.ToList(),
                IsActive = user.IsActive
            }
        };

        return Result<LoginResponseDto>.Success(response);
    }
}
