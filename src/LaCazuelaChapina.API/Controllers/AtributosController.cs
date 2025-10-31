// ============================================
// ARCHIVO: src/LaCazuelaChapina.API/Controllers/AtributosController.cs
// ============================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LaCazuelaChapina.Infrastructure.Data;
using LaCazuelaChapina.Domain.Entities;

namespace LaCazuelaChapina.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AtributosController : ControllerBase
{
    private readonly CazuelaChapinaContext _context;
    private readonly ILogger<AtributosController> _logger;

    public AtributosController(
        CazuelaChapinaContext context,
        ILogger<AtributosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ============================================
    // ATRIBUTOS DE TAMALES
    // ============================================

    /// <summary>
    /// Obtiene todos los tipos de masa para tamales
    /// </summary>
    [HttpGet("masas-tamal")]
    public async Task<ActionResult> GetMasasTamal()
    {
        try
        {
            var masas = await _context.MasasTamal
                .Where(m => m.Activo)
                .OrderBy(m => m.CostoAdicional)
                .ToListAsync();

            return Ok(new { Success = true, Data = masas });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener masas de tamal");
            return StatusCode(500, new { Success = false, Message = "Error al obtener datos", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene todos los rellenos para tamales
    /// </summary>
    [HttpGet("rellenos-tamal")]
    public async Task<ActionResult> GetRellenosTamal()
    {
        try
        {
            var rellenos = await _context.RellenosTamal
                .Where(r => r.Activo)
                .OrderBy(r => r.CostoAdicional)
                .ToListAsync();

            return Ok(new { Success = true, Data = rellenos });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener rellenos de tamal");
            return StatusCode(500, new { Success = false, Message = "Error al obtener datos", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene todos los tipos de envoltura para tamales
    /// </summary>
    [HttpGet("envolturas-tamal")]
    public async Task<ActionResult> GetEnvolturasTamal()
    {
        try
        {
            var envolturas = await _context.EnvolturasTamal
                .Where(e => e.Activo)
                .OrderBy(e => e.CostoAdicional)
                .ToListAsync();

            return Ok(new { Success = true, Data = envolturas });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener envolturas de tamal");
            return StatusCode(500, new { Success = false, Message = "Error al obtener datos", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene todos los niveles de picante para tamales
    /// </summary>
    [HttpGet("picantes-tamal")]
    public async Task<ActionResult> GetPicantesTamal()
    {
        try
        {
            var picantes = await _context.PicantesTamal
                .Where(p => p.Activo)
                .OrderBy(p => p.CostoAdicional)
                .ToListAsync();

            return Ok(new { Success = true, Data = picantes });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener niveles de picante");
            return StatusCode(500, new { Success = false, Message = "Error al obtener datos", Error = ex.Message });
        }
    }

    // ============================================
    // ATRIBUTOS DE BEBIDAS
    // ============================================

    /// <summary>
    /// Obtiene todos los tipos de bebida
    /// </summary>
    [HttpGet("tipos-bebida")]
    public async Task<ActionResult> GetTiposBebida()
    {
        try
        {
            var tipos = await _context.TiposBebida
                .Where(t => t.Activo)
                .OrderBy(t => t.CostoAdicional)
                .ToListAsync();

            return Ok(new { Success = true, Data = tipos });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipos de bebida");
            return StatusCode(500, new { Success = false, Message = "Error al obtener datos", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene todos los endulzantes para bebidas
    /// </summary>
    [HttpGet("endulzantes-bebida")]
    public async Task<ActionResult> GetEndulzantesBebida()
    {
        try
        {
            var endulzantes = await _context.EndulzantesBebida
                .Where(e => e.Activo)
                .OrderBy(e => e.CostoAdicional)
                .ToListAsync();

            return Ok(new { Success = true, Data = endulzantes });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener endulzantes");
            return StatusCode(500, new { Success = false, Message = "Error al obtener datos", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene todos los toppings para bebidas
    /// </summary>
    [HttpGet("toppings-bebida")]
    public async Task<ActionResult> GetToppingsBebida()
    {
        try
        {
            var toppings = await _context.ToppingsBebida
                .Where(t => t.Activo)
                .OrderBy(t => t.CostoAdicional)
                .ToListAsync();

            return Ok(new { Success = true, Data = toppings });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener toppings");
            return StatusCode(500, new { Success = false, Message = "Error al obtener datos", Error = ex.Message });
        }
    }

    // ============================================
    // OBTENER TODOS LOS ATRIBUTOS DE UNA VEZ
    // ============================================

    /// <summary>
    /// Obtiene todos los atributos disponibles (tamales y bebidas)
    /// </summary>
    [HttpGet("todos")]
    public async Task<ActionResult> GetTodosAtributos()
    {
        try
        {
            var atributos = new
            {
                Tamales = new
                {
                    Masas = await _context.MasasTamal.Where(m => m.Activo).ToListAsync(),
                    Rellenos = await _context.RellenosTamal.Where(r => r.Activo).ToListAsync(),
                    Envolturas = await _context.EnvolturasTamal.Where(e => e.Activo).ToListAsync(),
                    Picantes = await _context.PicantesTamal.Where(p => p.Activo).ToListAsync()
                },
                Bebidas = new
                {
                    Tipos = await _context.TiposBebida.Where(t => t.Activo).ToListAsync(),
                    Endulzantes = await _context.EndulzantesBebida.Where(e => e.Activo).ToListAsync(),
                    Toppings = await _context.ToppingsBebida.Where(t => t.Activo).ToListAsync()
                }
            };

            return Ok(new { Success = true, Data = atributos });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los atributos");
            return StatusCode(500, new { Success = false, Message = "Error al obtener datos", Error = ex.Message });
        }
    }

    // ============================================
    // CRUD DE ATRIBUTOS (ADMIN)
    // ============================================

    /// <summary>
    /// Crea una nueva masa de tamal
    /// </summary>
    [HttpPost("masas-tamal")]
    public async Task<ActionResult> CrearMasaTamal([FromBody] CrearAtributoRequest request)
    {
        try
        {
            var masa = new MasaTamal
            {
                Nombre = request.Nombre,
                CostoAdicional = request.CostoAdicional,
                Activo = true
            };

            _context.MasasTamal.Add(masa);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMasasTamal), null, new
            {
                Success = true,
                Data = masa,
                Message = "Masa creada exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear masa de tamal");
            return StatusCode(500, new { Success = false, Message = "Error al crear", Error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza una masa de tamal
    /// </summary>
    [HttpPut("masas-tamal/{id}")]
    public async Task<ActionResult> ActualizarMasaTamal(int id, [FromBody] ActualizarAtributoRequest request)
    {
        try
        {
            var masa = await _context.MasasTamal.FindAsync(id);
            if (masa == null)
                return NotFound(new { Success = false, Message = "Masa no encontrada" });

            masa.Nombre = request.Nombre;
            masa.CostoAdicional = request.CostoAdicional;
            masa.Activo = request.Activo;

            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Data = masa, Message = "Masa actualizada exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar masa de tamal");
            return StatusCode(500, new { Success = false, Message = "Error al actualizar", Error = ex.Message });
        }
    }

    // Endpoints similares para los demás atributos (Rellenos, Envolturas, etc.)
    // Por brevedad, se pueden implementar de la misma manera
}

// ============================================
// DTOs
// ============================================

public class CrearAtributoRequest
{
    public string Nombre { get; set; } = string.Empty;
    public decimal CostoAdicional { get; set; }
}

public class ActualizarAtributoRequest
{
    public string Nombre { get; set; } = string.Empty;
    public decimal CostoAdicional { get; set; }
    public bool Activo { get; set; }
}