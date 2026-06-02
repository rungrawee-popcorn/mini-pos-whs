using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniPOSWHS.Data;
using MiniPOSWHS.Models;

namespace MiniPOSWHS.Controllers.API;

[Route("api/[controller]")]
[ApiController]
public class ProductApiController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductApiController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/products
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products
            .OrderByDescending(x => x.ProductId)
            .ToListAsync();

        return Ok(products);
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.ProductId == id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }
}