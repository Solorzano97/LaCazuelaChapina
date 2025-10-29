// ============================================
// ARCHIVO: src/LaCazuelaChapina.API/Controllers/IAController.cs
// ============================================

using Microsoft.AspNetCore.Mvc;
using LaCazuelaChapina.Infrastructure.Services;
using LaCazuelaChapina.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace LaCazuelaChapina.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class IAController : ControllerBase
{
    private readonly IOpenRouterService _openRouterService;
    private readonly CazuelaChapinaContext _context;
    private readonly ILogger<IAController> _logger;

    public IAController(
        IOpenRouterService openRouterService,
        CazuelaChapinaContext context,
        ILogger<IAController> logger)
    {
        _openRouterService = openRouterService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Consulta general a la IA sobre productos y servicios
    /// </summary>
    [HttpPost("consulta")]
    public async Task<ActionResult> ConsultaGeneral([FromBody] ConsultaRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Pregunta))
            {
                return BadRequest(new { Success = false, Message = "La pregunta es requerida" });
            }

            var respuesta = await _openRouterService.GenerarRespuestaAsync(request.Pregunta, request.Contexto);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    Pregunta = request.Pregunta,
                    Respuesta = respuesta,
                    Timestamp = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en consulta general a IA");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error al procesar la consulta",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Sugiere un combo personalizado basado en preferencias del cliente
    /// </summary>
    [HttpPost("sugerir-combo")]
    public async Task<ActionResult> SugerirCombo([FromBody] PreferenciasComboRequest request)
    {
        try
        {
            var preferencias = new StringBuilder();
            
            if (!string.IsNullOrEmpty(request.TipoTamalPreferido))
                preferencias.AppendLine($"- Prefiere tamales: {request.TipoTamalPreferido}");
            
            if (!string.IsNullOrEmpty(request.NivelPicante))
                preferencias.AppendLine($"- Nivel de picante: {request.NivelPicante}");
            
            if (!string.IsNullOrEmpty(request.TipoBebidaPreferida))
                preferencias.AppendLine($"- Prefiere bebidas: {request.TipoBebidaPreferida}");
            
            if (request.Presupuesto > 0)
                preferencias.AppendLine($"- Presupuesto: Q{request.Presupuesto}");
            
            if (request.CantidadPersonas > 0)
                preferencias.AppendLine($"- Para {request.CantidadPersonas} personas");
            
            if (!string.IsNullOrEmpty(request.Ocasion))
                preferencias.AppendLine($"- Ocasión: {request.Ocasion}");

            var sugerencia = await _openRouterService.SugerirComboPersonalizadoAsync(preferencias.ToString());

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    Preferencias = request,
                    Sugerencia = sugerencia,
                    Timestamp = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al sugerir combo personalizado");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error al generar sugerencia de combo",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Analiza las ventas con IA y proporciona insights
    /// </summary>
    [HttpGet("analizar-ventas")]
    public async Task<ActionResult> AnalizarVentas(
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null)
    {
        try
        {
            var fechaDesde = desde ?? DateTime.UtcNow.AddDays(-30);
            var fechaHasta = hasta ?? DateTime.UtcNow;

            // Obtener datos de ventas
            var ventas = await _context.Ventas
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .Where(v => v.Fecha >= fechaDesde && v.Fecha <= fechaHasta && v.Estado == Domain.Enums.EstadoVenta.Completada)
                .ToListAsync();

            if (!ventas.Any())
            {
                return Ok(new
                {
                    Success = false,
                    Message = "No hay datos de ventas en el período especificado"
                });
            }

            // Preparar resumen de datos
            var datosVentas = new StringBuilder();
            datosVentas.AppendLine($"Período: {fechaDesde:dd/MM/yyyy} - {fechaHasta:dd/MM/yyyy}");
            datosVentas.AppendLine($"Total de ventas: {ventas.Count}");
            datosVentas.AppendLine($"Monto total: Q{ventas.Sum(v => v.Total):N2}");
            datosVentas.AppendLine($"Promedio por venta: Q{ventas.Average(v => v.Total):N2}");
            
            var productosMasVendidos = ventas
                .SelectMany(v => v.Detalles)
                .Where(d => d.Producto != null)
                .GroupBy(d => d.Producto!.Nombre)
                .Select(g => new { Producto = g.Key, Cantidad = g.Sum(d => d.Cantidad) })
                .OrderByDescending(x => x.Cantidad)
                .Take(5);

            datosVentas.AppendLine("\nProductos más vendidos:");
            foreach (var item in productosMasVendidos)
            {
                datosVentas.AppendLine($"- {item.Producto}: {item.Cantidad} unidades");
            }

            var analisis = await _openRouterService.AnalizarVentasAsync(datosVentas.ToString());

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    Periodo = new { Desde = fechaDesde, Hasta = fechaHasta },
                    Resumen = datosVentas.ToString(),
                    Analisis = analisis,
                    Timestamp = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al analizar ventas con IA");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error al analizar ventas",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Recomienda productos basándose en el historial del cliente
    /// </summary>
    [HttpGet("recomendar-productos/{clienteId}")]
    public async Task<ActionResult> RecomendarProductos(int clienteId)
    {
        try
        {
            // Obtener historial de compras del cliente
            var historial = await _context.Ventas
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .Where(v => v.UsuarioId == clienteId && v.Estado == Domain.Enums.EstadoVenta.Completada)
                .OrderByDescending(v => v.Fecha)
                .Take(10)
                .ToListAsync();

            if (!historial.Any())
            {
                return Ok(new
                {
                    Success = false,
                    Message = "No hay historial de compras para este cliente"
                });
            }

            var historialTexto = new StringBuilder();
            historialTexto.AppendLine($"Cliente ID: {clienteId}");
            historialTexto.AppendLine($"Total de compras: {historial.Count}");
            
            var productosComprados = historial
                .SelectMany(v => v.Detalles)
                .Where(d => d.Producto != null)
                .GroupBy(d => d.Producto!.Nombre)
                .Select(g => new { Producto = g.Key, Veces = g.Count() })
                .OrderByDescending(x => x.Veces);

            historialTexto.AppendLine("\nProductos comprados anteriormente:");
            foreach (var item in productosComprados)
            {
                historialTexto.AppendLine($"- {item.Producto} ({item.Veces} veces)");
            }

            var recomendaciones = await _openRouterService.RecomendarProductosAsync(historialTexto.ToString());

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    ClienteId = clienteId,
                    Historial = historialTexto.ToString(),
                    Recomendaciones = recomendaciones,
                    Timestamp = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al recomendar productos para cliente {ClienteId}", clienteId);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error al generar recomendaciones",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Optimiza el inventario usando IA
    /// </summary>
    [HttpGet("optimizar-inventario")]
    public async Task<ActionResult> OptimizarInventario()
    {
        try
        {
            // Obtener estado del inventario
            var materiasEnPuntoCritico = await _context.MateriasPrimas
                .Where(m => m.PuntoCritico)
                .ToListAsync();

            var materiasStockBajo = await _context.MateriasPrimas
                .Where(m => m.StockActual <= m.StockMinimo * 1.5m && !m.PuntoCritico)
                .ToListAsync();

            var datosInventario = new StringBuilder();
            
            datosInventario.AppendLine("=== ESTADO DE INVENTARIO ===\n");
            
            if (materiasEnPuntoCritico.Any())
            {
                datosInventario.AppendLine("⚠️ MATERIAS PRIMAS EN PUNTO CRÍTICO:");
                foreach (var materia in materiasEnPuntoCritico)
                {
                    datosInventario.AppendLine($"- {materia.Nombre}: {materia.StockActual} {materia.UnidadMedida} (Mínimo: {materia.StockMinimo})");
                }
                datosInventario.AppendLine();
            }

            if (materiasStockBajo.Any())
            {
                datosInventario.AppendLine("⚡ MATERIAS PRIMAS CON STOCK BAJO:");
                foreach (var materia in materiasStockBajo)
                {
                    datosInventario.AppendLine($"- {materia.Nombre}: {materia.StockActual} {materia.UnidadMedida} (Mínimo: {materia.StockMinimo})");
                }
                datosInventario.AppendLine();
            }

            // Obtener movimientos recientes
            var movimientosRecientes = await _context.MovimientosInventario
                .Include(m => m.MateriaPrima)
                .Where(m => m.Fecha >= DateTime.UtcNow.AddDays(-7))
                .GroupBy(m => m.MateriaPrima.Nombre)
                .Select(g => new
                {
                    Materia = g.Key,
                    Salidas = g.Where(m => m.Tipo == Domain.Enums.TipoMovimiento.Salida).Sum(m => m.Cantidad),
                    Mermas = g.Where(m => m.Tipo == Domain.Enums.TipoMovimiento.Merma).Sum(m => m.Cantidad)
                })
                .ToListAsync();

            if (movimientosRecientes.Any())
            {
                datosInventario.AppendLine("📊 CONSUMO ÚLTIMOS 7 DÍAS:");
                foreach (var mov in movimientosRecientes.OrderByDescending(m => m.Salidas))
                {
                    datosInventario.AppendLine($"- {mov.Materia}: {mov.Salidas} salidas, {mov.Mermas} mermas");
                }
            }

            var optimizacion = await _openRouterService.OptimizarInventarioAsync(datosInventario.ToString());

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    EstadoInventario = datosInventario.ToString(),
                    Optimizacion = optimizacion,
                    MaterialesCriticos = materiasEnPuntoCritico.Count,
                    MaterialesStockBajo = materiasStockBajo.Count,
                    Timestamp = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al optimizar inventario con IA");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error al optimizar inventario",
                Error = ex.Message
            });
        }
    }
}

// ============================================
// DTOs para Requests
// ============================================

public class ConsultaRequest
{
    public string Pregunta { get; set; } = string.Empty;
    public string? Contexto { get; set; }
}

public class PreferenciasComboRequest
{
    public string? TipoTamalPreferido { get; set; }
    public string? NivelPicante { get; set; }
    public string? TipoBebidaPreferida { get; set; }
    public decimal Presupuesto { get; set; }
    public int CantidadPersonas { get; set; }
    public string? Ocasion { get; set; }
}