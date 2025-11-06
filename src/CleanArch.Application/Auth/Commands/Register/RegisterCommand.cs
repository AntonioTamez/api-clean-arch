using CleanArch.Application.Auth.DTOs;
using CleanArch.Application.Common.Models;
using MediatR;

namespace CleanArch.Application.Auth.Commands.Register;

public record RegisterCommand(string Username, string Email, string Password, string FullName) 
    : IRequest<Result<UserDto>>;
