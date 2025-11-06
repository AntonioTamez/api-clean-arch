using CleanArch.Application.Auth.DTOs;
using CleanArch.Application.Common.Models;
using MediatR;

namespace CleanArch.Application.Auth.Commands.Login;

public record LoginCommand(string Username, string Password) : IRequest<Result<LoginResponseDto>>;
