// ============================================
// ARCHIVO: src/LaCazuelaChapina.API/Controllers/VentasController.cs
// ============================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LaCazuelaChapina.Infrastructure.Data;
using LaCazuelaChapina.Domain.Entities;
using LaCazuelaChapina.Domain.Enums;

namespace LaCazuelaChapina.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VentasController : ControllerBase
{
    private readonly CazuelaChapinaContext _context;
    private readonly ILogger<VentasController> _logger;

    public VentasController(
        CazuelaChapinaContext context,
        ILogger<VentasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las ventas con paginación
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetVentas(
        [FromQuery] int pagina = 1,
        [FromQuery] int porPagina = 20,
        [FromQuery] int? sucursalId = null,
        [FromQuery] EstadoVenta? estado = null,
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null)
    {
        try
        {
            var query = _context.Ventas
                .Include(v => v.Sucursal)
                .Include(v => v.Usuario)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Combo)
                .AsQueryable();

            // Filtros
            if (sucursalId.HasValue)
                query = query.Where(v => v.SucursalId == sucursalId.Value);

            if (estado.HasValue)
                query = query.Where(v => v.Estado == estado.Value);

            if (desde.HasValue)
                query = query.Where(v => v.Fecha >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(v => v.Fecha <= hasta.Value);

            var total = await query.CountAsync();
            var ventas = await query
                .OrderByDescending(v => v.Fecha)
                .Skip((pagina - 1) * porPagina)
                .Take(porPagina)
                .ToListAsync();

            return Ok(new
            {
                Success = true,
                Data = ventas,
                Paginacion = new
                {
                    Pagina = pagina,
                    PorPagina = porPagina,
                    Total = total,
                    TotalPaginas = (int)Math.Ceiling(total / (double)porPagina)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ventas");
            return StatusCode(500, new { Success = false, Message = "Error al obtener ventas", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene una venta por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetVenta(int id)
    {
        try
        {
            var venta = await _context.Ventas
                .Include(v => v.Sucursal)
                .Include(v => v.Usuario)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Combo)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
                return NotFound(new { Success = false, Message = $"Venta {id} no encontrada" });

            return Ok(new { Success = true, Data = venta });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener venta {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al obtener venta", Error = ex.Message });
        }
    }

    /// <summary>
    /// Registra una nueva venta
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CrearVenta([FromBody] CrearVentaRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // Validar sucursal y usuario
            var sucursal = await _context.Sucursales.FindAsync(request.SucursalId);
            if (sucursal == null)
                return BadRequest(new { Success = false, Message = "Sucursal no encontrada" });

            var usuario = await _context.Usuarios.FindAsync(request.UsuarioId);
            if (usuario == null)
                return BadRequest(new { Success = false, Message = "Usuario no encontrado" });

            // Generar número de orden
            var numeroOrden = await GenerarNumeroOrdenAsync(request.SucursalId);

            // Crear venta
            var venta = new Venta
            {
                NumeroOrden = numeroOrden,
                SucursalId = request.SucursalId,
                UsuarioId = request.UsuarioId,
                Estado = EstadoVenta.Pendiente,
                Descuento = request.Descuento,
                Sincronizada = request.Sincronizada
            };

            decimal subtotal = 0;

            // Procesar detalles
            foreach (var detalle in request.Detalles)
            {
                var ventaDetalle = new VentaDetalle
                {
                    ProductoId = detalle.ProductoId,
                    ComboId = detalle.ComboId,
                    Cantidad = detalle.Cantidad,
                    ConfiguracionJson = detalle.ConfiguracionJson
                };

                // Calcular precio
                if (detalle.ProductoId.HasValue)
                {
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId.Value);
                    if (producto == null)
                        return BadRequest(new { Success = false, Message = $"Producto {detalle.ProductoId} no encontrado" });
                    
                    ventaDetalle.PrecioUnitario = producto.PrecioBase;
                    
                    // Aquí se podría agregar lógica para costos adicionales según ConfiguracionJson
                }
                else if (detalle.ComboId.HasValue)
                {
                    var combo = await _context.Combos.FindAsync(detalle.ComboId.Value);
                    if (combo == null)
                        return BadRequest(new { Success = false, Message = $"Combo {detalle.ComboId} no encontrado" });
                    
                    ventaDetalle.PrecioUnitario = combo.Precio;
                }

                ventaDetalle.Subtotal = ventaDetalle.PrecioUnitario * detalle.Cantidad;
                subtotal += ventaDetalle.Subtotal;

                venta.Detalles.Add(ventaDetalle);
            }

            venta.Subtotal = subtotal;
            venta.Total = subtotal - request.Descuento;

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            // Marcar como completada si no hay problemas
            venta.Estado = EstadoVenta.Completada;
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            _logger.LogInformation("Venta creada: {NumeroOrden} - Total: Q{Total}", venta.NumeroOrden, venta.Total);

            // Recargar con relaciones
            var ventaCompleta = await _context.Ventas
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Combo)
                .FirstOrDefaultAsync(v => v.Id == venta.Id);

            return CreatedAtAction(nameof(GetVenta), new { id = venta.Id }, new
            {
                Success = true,
                Data = ventaCompleta,
                Message = "Venta registrada exitosamente"
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error al crear venta");
            return StatusCode(500, new { Success = false, Message = "Error al crear venta", Error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza el estado de una venta
    /// </summary>
    [HttpPatch("{id}/estado")]
    public async Task<ActionResult> ActualizarEstado(int id, [FromBody] ActualizarEstadoRequest request)
    {
        try
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
                return NotFound(new { Success = false, Message = "Venta no encontrada" });

            venta.Estado = request.Estado;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Estado de venta {Id} actualizado a {Estado}", id, request.Estado);

            return Ok(new
            {
                Success = true,
                Data = venta,
                Message = "Estado actualizado exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado de venta {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al actualizar estado", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene el resumen de ventas por período
    /// </summary>
    [HttpGet("resumen")]
    public async Task<ActionResult> GetResumenVentas(
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null,
        [FromQuery] int? sucursalId = null)
    {
        try
        {
            var fechaDesde = desde ?? DateTime.UtcNow.Date.AddDays(-30);
            var fechaHasta = hasta ?? DateTime.UtcNow;

            var query = _context.Ventas
                .Where(v => v.Fecha >= fechaDesde && v.Fecha <= fechaHasta && v.Estado == EstadoVenta.Completada);

            if (sucursalId.HasValue)
                query = query.Where(v => v.SucursalId == sucursalId.Value);

            var ventas = await query.ToListAsync();

            var resumen = new
            {
                Periodo = new { Desde = fechaDesde, Hasta = fechaHasta },
                TotalVentas = ventas.Count,
                MontoTotal = ventas.Sum(v => v.Total),
                MontoPromedio = ventas.Any() ? ventas.Average(v => v.Total) : 0,
                VentaPorDia = ventas
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

            return Ok(new { Success = true, Data = resumen });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener resumen de ventas");
            return StatusCode(500, new { Success = false, Message = "Error al obtener resumen", Error = ex.Message });
        }
    }

    /// <summary>
    /// Cancela una venta
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> CancelarVenta(int id)
    {
        try
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
                return NotFound(new { Success = false, Message = "Venta no encontrada" });

            if (venta.Estado == EstadoVenta.Cancelada)
                return BadRequest(new { Success = false, Message = "La venta ya está cancelada" });

            venta.Estado = EstadoVenta.Cancelada;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Venta {Id} cancelada", id);

            return Ok(new { Success = true, Message = "Venta cancelada exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cancelar venta {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al cancelar venta", Error = ex.Message });
        }
    }

    // Método auxiliar para generar número de orden
    private async Task<string> GenerarNumeroOrdenAsync(int sucursalId)
    {
        var fecha = DateTime.UtcNow.ToString("yyyyMMdd");
        var ultimaOrden = await _context.Ventas
            .Where(v => v.SucursalId == sucursalId && v.NumeroOrden.StartsWith($"SUC{sucursalId}-{fecha}"))
            .OrderByDescending(v => v.NumeroOrden)
            .FirstOrDefaultAsync();

        int secuencia = 1;
        if (ultimaOrden != null)
        {
            var partes = ultimaOrden.NumeroOrden.Split('-');
            if (partes.Length == 3 && int.TryParse(partes[2], out int sec))
                secuencia = sec + 1;
        }

        return $"SUC{sucursalId}-{fecha}-{secuencia:D4}";
    }
}

// ============================================
// DTOs para Requests
// ============================================

public class CrearVentaRequest
{
    public int SucursalId { get; set; }
    public int UsuarioId { get; set; }
    public decimal Descuento { get; set; } = 0;
    public bool Sincronizada { get; set; } = true;
    public List<VentaDetalleRequest> Detalles { get; set; } = new();
}

public class VentaDetalleRequest
{
    public int? ProductoId { get; set; }
    public int? ComboId { get; set; }
    public int Cantidad { get; set; }
    public string? ConfiguracionJson { get; set; }
}

public class ActualizarEstadoRequest
{
    public EstadoVenta Estado { get; set; }
}