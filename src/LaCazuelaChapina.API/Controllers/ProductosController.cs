// ============================================
// ARCHIVO: src/LaCazuelaChapina.API/Controllers/ProductosController.cs
// ============================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LaCazuelaChapina.Infrastructure.Data;
using LaCazuelaChapina.Domain.Entities;

namespace LaCazuelaChapina.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductosController : ControllerBase
{
    private readonly CazuelaChapinaContext _context;
    private readonly ILogger<ProductosController> _logger;

    public ProductosController(
        CazuelaChapinaContext context,
        ILogger<ProductosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los productos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
    {
        try
        {
            var productos = await _context.Productos
                .Where(p => p.Activo)
                .ToListAsync();

            return Ok(new
            {
                Success = true,
                Data = productos,
                Count = productos.Count,
                Message = "Productos obtenidos exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error al obtener productos",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Obtiene un producto por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Producto>> GetProducto(int id)
    {
        try
        {
            var producto = await _context.Productos
                .Include(p => p.Recetas)
                    .ThenInclude(r => r.MateriaPrima)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = $"Producto con ID {id} no encontrado"
                });
            }

            return Ok(new
            {
                Success = true,
                Data = producto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener producto {Id}", id);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error al obtener producto",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Crea un nuevo producto
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Producto>> CreateProducto([FromBody] Producto producto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Datos inválidos",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Producto creado: {Nombre} (ID: {Id})", producto.Nombre, producto.Id);

            return CreatedAtAction(
                nameof(GetProducto),
                new { id = producto.Id },
                new
                {
                    Success = true,
                    Data = producto,
                    Message = "Producto creado exitosamente"
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear producto");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error al crear producto",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Actualiza un producto existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProducto(int id, [FromBody] Producto producto)
    {
        try
        {
            if (id != producto.Id)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "El ID del producto no coincide"
                });
            }

            var productoExistente = await _context.Productos.FindAsync(id);
            if (productoExistente == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = $"Producto con ID {id} no encontrado"
                });
            }

            // Actualizar propiedades
            productoExistente.Nombre = producto.Nombre;
            productoExistente.Tipo = producto.Tipo;
            productoExistente.PrecioBase = producto.PrecioBase;
            productoExistente.Activo = producto.Activo;
            productoExistente.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Producto actualizado: {Nombre} (ID: {Id})", producto.Nombre, id);

            return Ok(new
            {
                Success = true,
                Data = productoExistente,
                Message = "Producto actualizado exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar producto {Id}", id);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error al actualizar producto",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Elimina (desactiva) un producto
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProducto(int id)
    {
        try
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = $"Producto con ID {id} no encontrado"
                });
            }

            // Soft delete - solo desactivar
            producto.Activo = false;
            producto.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Producto desactivado: {Nombre} (ID: {Id})", producto.Nombre, id);

            return Ok(new
            {
                Success = true,
                Message = "Producto desactivado exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar producto {Id}", id);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error al eliminar producto",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Obtiene productos por tipo (Tamal o Bebida)
    /// </summary>
    [HttpGet("tipo/{tipo}")]
    public async Task<ActionResult<IEnumerable<Producto>>> GetProductosPorTipo(string tipo)
    {
        try
        {
            if (!Enum.TryParse<Domain.Enums.TipoProducto>(tipo, true, out var tipoProducto))
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Tipo de producto inválido. Use 'Tamal' o 'Bebida'"
                });
            }

            var productos = await _context.Productos
                .Where(p => p.Tipo == tipoProducto && p.Activo)
                .ToListAsync();

            return Ok(new
            {
                Success = true,
                Data = productos,
                Count = productos.Count,
                Tipo = tipo
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos por tipo {Tipo}", tipo);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error al obtener productos",
                Error = ex.Message
            });
        }
    }
}