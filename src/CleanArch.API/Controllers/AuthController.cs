using CleanArch.Application.Auth.Commands.Login;
using CleanArch.Application.Auth.Commands.Register;
using CleanArch.Application.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.API.Controllers;

/// <summary>
/// Controller para autenticación y registro de usuarios
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registra un nuevo usuario
    /// </summary>
    /// <param name="dto">Datos de registro</param>
    /// <returns>Usuario creado</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var command = new RegisterCommand(dto.Username, dto.Email, dto.Password, dto.FullName);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetCurrentUser), null, result.Value);
    }

    /// <summary>
    /// Inicia sesión con username y password
    /// </summary>
    /// <param name="dto">Credenciales de login</param>
    /// <returns>Token JWT y datos del usuario</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var command = new LoginCommand(dto.Username, dto.Password);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Unauthorized(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene información del usuario actual autenticado
    /// </summary>
    /// <returns>Datos del usuario</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var fullName = User.FindFirst("FullName")?.Value;
        var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var userDto = new UserDto
        {
            Id = Guid.Parse(userId),
            Username = username ?? "",
            Email = email ?? "",
            FullName = fullName ?? "",
            Roles = roles,
            IsActive = true
        };

        return Ok(userDto);
    }

    /// <summary>
    /// Endpoint de prueba para verificar autenticación
    /// </summary>
    [HttpGet("test-auth")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult TestAuth()
    {
        return Ok(new { message = "You are authenticated!", user = User.Identity?.Name });
    }

    /// <summary>
    /// Endpoint de prueba para verificar rol de Admin
    /// </summary>
    [HttpGet("test-admin")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult TestAdmin()
    {
        return Ok(new { message = "You are an admin!", user = User.Identity?.Name });
    }
}
