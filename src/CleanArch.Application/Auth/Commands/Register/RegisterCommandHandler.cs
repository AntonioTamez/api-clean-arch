using CleanArch.Application.Auth.DTOs;
using CleanArch.Application.Common.Interfaces;
using CleanArch.Application.Common.Models;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces;
using MediatR;

namespace CleanArch.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Validar que el username no exista
        if (await _userRepository.UsernameExistsAsync(request.Username, cancellationToken))
            return Result<UserDto>.Failure(new Error("Register.UsernameExists", "Username already exists"));

        // Validar que el email no exista
        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
            return Result<UserDto>.Failure(new Error("Register.EmailExists", "Email already exists"));

        // Hash del password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Crear el usuario
        var user = User.Create(request.Username, request.Email, passwordHash, request.FullName);
        
        // Asignar rol por defecto
        user.AddRole("User");

        // Guardar en BD
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Roles = user.Roles.ToList(),
            IsActive = user.IsActive
        };

        return Result<UserDto>.Success(userDto);
    }
}
