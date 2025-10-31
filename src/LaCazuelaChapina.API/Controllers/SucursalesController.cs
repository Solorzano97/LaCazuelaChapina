// ============================================
// ARCHIVO: src/LaCazuelaChapina.API/Controllers/SucursalesController.cs
// ============================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LaCazuelaChapina.Infrastructure.Data;
using LaCazuelaChapina.Domain.Entities;

namespace LaCazuelaChapina.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SucursalesController : ControllerBase
{
    private readonly CazuelaChapinaContext _context;
    private readonly ILogger<SucursalesController> _logger;

    public SucursalesController(
        CazuelaChapinaContext context,
        ILogger<SucursalesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las sucursales
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetSucursales([FromQuery] bool? soloActivas = true)
    {
        try
        {
            var query = _context.Sucursales.AsQueryable();

            if (soloActivas == true)
                query = query.Where(s => s.Activa);

            var sucursales = await query
                .OrderBy(s => s.Nombre)
                .ToListAsync();

            return Ok(new { Success = true, Data = sucursales, Count = sucursales.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener sucursales");
            return StatusCode(500, new { Success = false, Message = "Error al obtener sucursales", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene una sucursal por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetSucursal(int id)
    {
        try
        {
            var sucursal = await _context.Sucursales
                .Include(s => s.Usuarios)
                .Include(s => s.Vaporeras)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sucursal == null)
                return NotFound(new { Success = false, Message = "Sucursal no encontrada" });

            return Ok(new { Success = true, Data = sucursal });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener sucursal {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al obtener sucursal", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene estadísticas de una sucursal
    /// </summary>
    [HttpGet("{id}/estadisticas")]
    public async Task<ActionResult> GetEstadisticasSucursal(
        int id,
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null)
    {
        try
        {
            var fechaDesde = desde ?? DateTime.UtcNow.AddDays(-30);
            var fechaHasta = hasta ?? DateTime.UtcNow;

            var ventas = await _context.Ventas
                .Where(v => v.SucursalId == id)
                .Where(v => v.Fecha >= fechaDesde && v.Fecha <= fechaHasta)
                .Where(v => v.Estado == Domain.Enums.EstadoVenta.Completada)
                .ToListAsync();

            var usuarios = await _context.Usuarios
                .Where(u => u.SucursalId == id && u.Activo)
                .CountAsync();

            var vaporeras = await _context.Vaporeras
                .Where(v => v.SucursalId == id)
                .CountAsync();

            var estadisticas = new
            {
                Periodo = new { Desde = fechaDesde, Hasta = fechaHasta },
                TotalVentas = ventas.Count,
                MontoTotal = ventas.Sum(v => v.Total),
                MontoPromedio = ventas.Any() ? ventas.Average(v => v.Total) : 0,
                EmpleadosActivos = usuarios,
                VaporerasDisponibles = vaporeras,
                VentasPorDia = ventas
                    .GroupBy(v => v.Fecha.Date)
                    .Select(g => new
                    {
                        Fecha = g.Key,
                        Cantidad = g.Count(),
                        Monto = g.Sum(v => v.Total)
                    })
                    .OrderBy(x => x.Fecha)
                    .ToList()
            };

            return Ok(new { Success = true, Data = estadisticas });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas de sucursal {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al obtener estadísticas", Error = ex.Message });
        }
    }

    /// <summary>
    /// Crea una nueva sucursal
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CrearSucursal([FromBody] CrearSucursalRequest request)
    {
        try
        {
            var sucursal = new Sucursal
            {
                Nombre = request.Nombre,
                Direccion = request.Direccion,
                Telefono = request.Telefono,
                Activa = true
            };

            _context.Sucursales.Add(sucursal);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Sucursal creada: {Nombre} (ID: {Id})", sucursal.Nombre, sucursal.Id);

            return CreatedAtAction(nameof(GetSucursal), new { id = sucursal.Id }, new
            {
                Success = true,
                Data = sucursal,
                Message = "Sucursal creada exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear sucursal");
            return StatusCode(500, new { Success = false, Message = "Error al crear sucursal", Error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza una sucursal
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> ActualizarSucursal(int id, [FromBody] ActualizarSucursalRequest request)
    {
        try
        {
            var sucursal = await _context.Sucursales.FindAsync(id);
            if (sucursal == null)
                return NotFound(new { Success = false, Message = "Sucursal no encontrada" });

            sucursal.Nombre = request.Nombre;
            sucursal.Direccion = request.Direccion;
            sucursal.Telefono = request.Telefono;
            sucursal.Activa = request.Activa;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Sucursal actualizada: {Nombre} (ID: {Id})", sucursal.Nombre, id);

            return Ok(new
            {
                Success = true,
                Data = sucursal,
                Message = "Sucursal actualizada exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar sucursal {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al actualizar sucursal", Error = ex.Message });
        }
    }

    /// <summary>
    /// Activa o desactiva una sucursal
    /// </summary>
    [HttpPatch("{id}/estado")]
    public async Task<ActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoRequest request)
    {
        try
        {
            var sucursal = await _context.Sucursales.FindAsync(id);
            if (sucursal == null)
                return NotFound(new { Success = false, Message = "Sucursal no encontrada" });

            sucursal.Activa = request.Activa;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Estado de sucursal {Id} cambiado a {Activa}", id, request.Activa);

            return Ok(new
            {
                Success = true,
                Data = sucursal,
                Message = $"Sucursal {(request.Activa ? "activada" : "desactivada")} exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado de sucursal {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al cambiar estado", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene el ranking de sucursales por ventas
    /// </summary>
    [HttpGet("ranking")]
    public async Task<ActionResult> GetRankingSucursales(
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null)
    {
        try
        {
            var fechaDesde = desde ?? DateTime.UtcNow.AddMonths(-1);
            var fechaHasta = hasta ?? DateTime.UtcNow;

            var ranking = await _context.Ventas
                .Include(v => v.Sucursal)
                .Where(v => v.Estado == Domain.Enums.EstadoVenta.Completada)
                .Where(v => v.Fecha >= fechaDesde && v.Fecha <= fechaHasta)
                .GroupBy(v => new { v.SucursalId, v.Sucursal.Nombre })
                .Select(g => new
                {
                    SucursalId = g.Key.SucursalId,
                    Nombre = g.Key.Nombre,
                    TotalVentas = g.Sum(v => v.Total),
                    NumeroOrdenes = g.Count(),
                    TicketPromedio = g.Average(v => v.Total)
                })
                .OrderByDescending(x => x.TotalVentas)
                .ToListAsync();

            return Ok(new
            {
                Success = true,
                Data = ranking,
                Periodo = new { Desde = fechaDesde, Hasta = fechaHasta }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ranking de sucursales");
            return StatusCode(500, new { Success = false, Message = "Error al obtener ranking", Error = ex.Message });
        }
    }
}

// ============================================
// DTOs
// ============================================

public class CrearSucursalRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string? Telefono { get; set; }
}

public class ActualizarSucursalRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public bool Activa { get; set; }
}

public class CambiarEstadoRequest
{
    public bool Activa { get; set; }
}