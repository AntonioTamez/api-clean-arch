using ApiCleanArch.Application.Users;
using ApiCleanArch.Application.Users.Create;
using ApiCleanArch.Application.Users.Delete;
using ApiCleanArch.Application.Users.Get;
using ApiCleanArch.Application.Users.List;
using ApiCleanArch.Application.Users.Update;
using Microsoft.AspNetCore.Mvc;

namespace ApiCleanArch.Api.Users;

[ApiController]
[Route("api/v1/users")]
public sealed class UsersController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<UserDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromServices] ListUsersHandler handler, CancellationToken cancellationToken) =>
        Ok(await handler.Handle(new ListUsersQuery(), cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, [FromServices] GetUserHandler handler, CancellationToken cancellationToken) =>
        Ok(await handler.Handle(new GetUserQuery(id), cancellationToken));

    [HttpPost]
    [ProducesResponseType<UserDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        CreateUserRequest request,
        [FromServices] CreateUserHandler handler,
        CancellationToken cancellationToken)
    {
        var created = await handler.Handle(new CreateUserCommand(request.Name, request.Email), cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateUserRequest request,
        [FromServices] UpdateUserHandler handler,
        CancellationToken cancellationToken) =>
        Ok(await handler.Handle(new UpdateUserCommand(id, request.Name, request.Email), cancellationToken));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, [FromServices] DeleteUserHandler handler, CancellationToken cancellationToken)
    {
        await handler.Handle(new DeleteUserCommand(id), cancellationToken);
        return NoContent();
    }
}
