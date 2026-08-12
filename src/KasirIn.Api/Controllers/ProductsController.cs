namespace KasirIn.Api.Controllers;

using KasirIn.Application.Common.Interfaces;
using KasirIn.Application.Products.Commands.CreateProduct;
using KasirIn.Application.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IFileStorageService _fileStorageService;

    public ProductsController(ISender sender, IFileStorageService fileStorageService)
    {
        _sender = sender;
        _fileStorageService = fileStorageService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts(
        [FromQuery] Guid tenantId,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery(tenantId, categoryId, searchTerm);
        var products = await _sender.Send(query, cancellationToken);
        return Ok(products);
    }

    [HttpPost]
    public async Task<ActionResult<object>> CreateProduct(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var id = await _sender.Send(command, cancellationToken);
        return Ok(new { id });
    }

    [HttpPost("upload-image")]
    public async Task<ActionResult<object>> UploadImage(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File cannot be empty.");
        }

        using var stream = file.OpenReadStream();
        var url = await _fileStorageService.UploadImageAsync(stream, file.FileName, cancellationToken);
        return Ok(new { url });
    }
}
