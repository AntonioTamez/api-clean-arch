using CleanArch.Application.Products.Commands.CreateProduct;
using CleanArch.Application.Products.Queries.GetProduct;
using CleanArch.Application.Products.Queries.GetProducts;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.API.Controllers;

/// <summary>
/// Controlador de productos
/// </summary>
public class ProductsController : BaseApiController
{
    /// <summary>
    /// Obtiene todos los productos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = false)
    {
        var query = new GetProductsQuery { OnlyActive = onlyActive };
        var result = await Mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene un producto por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetProductQuery(id);
        var result = await Mediator.Send(query);

        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Crea un nuevo producto
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var result = await Mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }
}
