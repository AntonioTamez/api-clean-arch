using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.API.Controllers;

/// <summary>
/// Controlador base para todos los controladores de la API
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
