// ============================================
// ARCHIVO: src/LaCazuelaChapina.API/Controllers/DashboardController.cs
// ============================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LaCazuelaChapina.Infrastructure.Data;
using LaCazuelaChapina.Domain.Enums;

namespace LaCazuelaChapina.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly CazuelaChapinaContext _context;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        CazuelaChapinaContext context,
        ILogger<DashboardController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los KPIs principales del dashboard
    /// </summary>
    [HttpGet("kpis")]
    public async Task<ActionResult> GetKPIs(
        [FromQuery] int? sucursalId = null,
        [FromQuery] DateTime? fecha = null)
    {
        try
        {
            var fechaConsulta = fecha ?? DateTime.UtcNow.Date;
            var inicioMes = new DateTime(fechaConsulta.Year, fechaConsulta.Month, 1);

            var query = _context.Ventas
                .Where(v => v.Estado == EstadoVenta.Completada);

            if (sucursalId.HasValue)
                query = query.Where(v => v.SucursalId == sucursalId.Value);

            // Ventas del día
            var ventasDia = await query
                .Where(v => v.Fecha.Date == fechaConsulta)
                .SumAsync(v => v.Total);

            // Ventas del mes
            var ventasMes = await query
                .Where(v => v.Fecha >= inicioMes && v.Fecha.Date <= fechaConsulta)
                .SumAsync(v => v.Total);

            // Ventas del mes anterior (para comparación)
            var inicioMesAnterior = inicioMes.AddMonths(-1);
            var ventasMesAnterior = await query
                .Where(v => v.Fecha >= inicioMesAnterior && v.Fecha < inicioMes)
                .SumAsync(v => v.Total);

            var crecimientoMensual = ventasMesAnterior > 0 
                ? ((ventasMes - ventasMesAnterior) / ventasMesAnterior) * 100 
                : 0;

            // Total de órdenes hoy
            var ordenesHoy = await query
                .Where(v => v.Fecha.Date == fechaConsulta)
                .CountAsync();

            // Ticket promedio
            var ticketPromedio = ordenesHoy > 0 ? ventasDia / ordenesHoy : 0;

            var kpis = new
            {
                Fecha = fechaConsulta,
                VentasDia = new
                {
                    Total = ventasDia,
                    NumeroOrdenes = ordenesHoy,
                    TicketPromedio = ticketPromedio
                },
                VentasMes = new
                {
                    Total = ventasMes,
                    CrecimientoVsMesAnterior = Math.Round(crecimientoMensual, 2)
                }
            };

            return Ok(new { Success = true, Data = kpis });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener KPIs");
            return StatusCode(500, new { Success = false, Message = "Error al obtener KPIs", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene los tamales más vendidos
    /// </summary>
    [HttpGet("tamales-mas-vendidos")]
    public async Task<ActionResult> GetTamalesMasVendidos(
        [FromQuery] int? sucursalId = null,
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null,
        [FromQuery] int top = 10)
    {
        try
        {
            var fechaDesde = desde ?? DateTime.UtcNow.AddDays(-30);
            var fechaHasta = hasta ?? DateTime.UtcNow;

            var query = _context.VentasDetalle
                .Include(vd => vd.Venta)
                .Include(vd => vd.Producto)
                .Where(vd => vd.Producto != null)
                .Where(vd => vd.Producto!.Tipo == TipoProducto.Tamal)
                .Where(vd => vd.Venta.Estado == EstadoVenta.Completada)
                .Where(vd => vd.Venta.Fecha >= fechaDesde && vd.Venta.Fecha <= fechaHasta);

            if (sucursalId.HasValue)
                query = query.Where(vd => vd.Venta.SucursalId == sucursalId.Value);

            var tamalesMasVendidos = await query
                .GroupBy(vd => new { vd.ProductoId, vd.Producto!.Nombre })
                .Select(g => new
                {
                    ProductoId = g.Key.ProductoId,
                    Nombre = g.Key.Nombre,
                    CantidadVendida = g.Sum(vd => vd.Cantidad),
                    TotalVentas = g.Sum(vd => vd.Subtotal),
                    NumeroOrdenes = g.Count()
                })
                .OrderByDescending(x => x.CantidadVendida)
                .Take(top)
                .ToListAsync();

            return Ok(new
            {
                Success = true,
                Data = tamalesMasVendidos,
                Periodo = new { Desde = fechaDesde, Hasta = fechaHasta }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tamales más vendidos");
            return StatusCode(500, new { Success = false, Message = "Error al obtener datos", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene las bebidas preferidas por franja horaria
    /// </summary>
    [HttpGet("bebidas-por-horario")]
    public async Task<ActionResult> GetBebidasPorHorario(
        [FromQuery] int? sucursalId = null,
        [FromQuery] DateTime? fecha = null)
    {
        try
        {
            var fechaConsulta = fecha ?? DateTime.UtcNow.Date;

            var query = _context.VentasDetalle
                .Include(vd => vd.Venta)
                .Include(vd => vd.Producto)
                .Where(vd => vd.Producto != null)
                .Where(vd => vd.Producto!.Tipo == TipoProducto.Bebida)
                .Where(vd => vd.Venta.Estado == EstadoVenta.Completada)
                .Where(vd => vd.Venta.Fecha.Date == fechaConsulta);

            if (sucursalId.HasValue)
                query = query.Where(vd => vd.Venta.SucursalId == sucursalId.Value);

            var ventas = await query.ToListAsync();

            // Agrupar por franja horaria
            var bebidasPorHorario = ventas
                .GroupBy(vd => ObtenerFranjaHoraria(vd.Venta.Fecha.Hour))
                .Select(g => new
                {
                    FranjaHoraria = g.Key,
                    Bebidas = g.GroupBy(vd => vd.Producto!.Nombre)
                        .Select(bg => new
                        {
                            Nombre = bg.Key,
                            Cantidad = bg.Sum(vd => vd.Cantidad)
                        })
                        .OrderByDescending(b => b.Cantidad)
                        .Take(5)
                        .ToList(),
                    TotalBebidas = g.Sum(vd => vd.Cantidad)
                })
                .OrderBy(x => OrdenFranjaHoraria(x.FranjaHoraria))
                .ToList();

            return Ok(new
            {
                Success = true,
                Data = bebidasPorHorario,
                Fecha = fechaConsulta
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener bebidas por horario");
            return StatusCode(500, new { Success = false, Message = "Error al obtener datos", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene la proporción de tamales por nivel de picante
    /// </summary>
    [HttpGet("proporcion-picante")]
    public async Task<ActionResult> GetProporcionPicante(
        [FromQuery] int? sucursalId = null,
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null)
    {
        try
        {
            var fechaDesde = desde ?? DateTime.UtcNow.AddDays(-30);
            var fechaHasta = hasta ?? DateTime.UtcNow;

            // Obtener estadísticas de picante si existen
            var estadisticasQuery = _context.EstadisticasPicante
                .Include(e => e.PicanteTamal)
                .Where(e => e.Fecha >= fechaDesde.Date && e.Fecha <= fechaHasta.Date);

            if (sucursalId.HasValue)
                estadisticasQuery = estadisticasQuery.Where(e => e.SucursalId == sucursalId.Value);

            var estadisticas = await estadisticasQuery.ToListAsync();

            if (estadisticas.Any())
            {
                var proporcion = estadisticas
                    .GroupBy(e => e.PicanteTamal.Nombre)
                    .Select(g => new
                    {
                        NivelPicante = g.Key,
                        Cantidad = g.Sum(e => e.CantidadVendida),
                        Porcentaje = 0m // Se calculará después
                    })
                    .ToList();

                var total = proporcion.Sum(p => p.Cantidad);
                var proporcionConPorcentaje = proporcion.Select(p => new
                {
                    p.NivelPicante,
                    p.Cantidad,
                    Porcentaje = total > 0 ? Math.Round((decimal)p.Cantidad / total * 100, 2) : 0
                }).ToList();

                return Ok(new
                {
                    Success = true,
                    Data = proporcionConPorcentaje,
                    Total = total,
                    Periodo = new { Desde = fechaDesde, Hasta = fechaHasta }
                });
            }

            // Si no hay estadísticas, devolver mensaje
            return Ok(new
            {
                Success = true,
                Data = new List<object>(),
                Message = "No hay datos de picante en el período especificado"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener proporción de picante");
            return StatusCode(500, new { Success = false, Message = "Error al obtener datos", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene las utilidades por línea de producto
    /// </summary>
    [HttpGet("utilidades-por-linea")]
    public async Task<ActionResult> GetUtilidadesPorLinea(
        [FromQuery] int? sucursalId = null,
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null)
    {
        try
        {
            var fechaDesde = desde ?? DateTime.UtcNow.AddMonths(-1);
            var fechaHasta = hasta ?? DateTime.UtcNow;

            var query = _context.VentasDetalle
                .Include(vd => vd.Venta)
                .Include(vd => vd.Producto)
                .Where(vd => vd.Producto != null)
                .Where(vd => vd.Venta.Estado == EstadoVenta.Completada)
                .Where(vd => vd.Venta.Fecha >= fechaDesde && vd.Venta.Fecha <= fechaHasta);

            if (sucursalId.HasValue)
                query = query.Where(vd => vd.Venta.SucursalId == sucursalId.Value);

            var ventas = await query.ToListAsync();

            var utilidadesPorLinea = ventas
                .GroupBy(vd => vd.Producto!.Tipo)
                .Select(g => new
                {
                    Linea = g.Key.ToString(),
                    VentaTotal = g.Sum(vd => vd.Subtotal),
                    CantidadVendida = g.Sum(vd => vd.Cantidad),
                    // Asumiendo un margen del 45% (puedes calcularlo con costos reales)
                    UtilidadEstimada = g.Sum(vd => vd.Subtotal) * 0.45m,
                    MargenPorcentaje = 45m
                })
                .OrderByDescending(x => x.VentaTotal)
                .ToList();

            return Ok(new
            {
                Success = true,
                Data = utilidadesPorLinea,
                Periodo = new { Desde = fechaDesde, Hasta = fechaHasta },
                Nota = "Las utilidades son estimadas con margen del 45%. Para cálculos precisos se requiere el costo de cada producto."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener utilidades por línea");
            return StatusCode(500, new { Success = false, Message = "Error al obtener datos", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene el desperdicio de materias primas
    /// </summary>
    [HttpGet("desperdicio-materias-primas")]
    public async Task<ActionResult> GetDesperdicioMateriasPrimas(
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null)
    {
        try
        {
            var fechaDesde = desde ?? DateTime.UtcNow.AddDays(-30);
            var fechaHasta = hasta ?? DateTime.UtcNow;

            var mermas = await _context.MovimientosInventario
                .Include(m => m.MateriaPrima)
                .Where(m => m.Tipo == TipoMovimiento.Merma)
                .Where(m => m.Fecha >= fechaDesde && m.Fecha <= fechaHasta)
                .GroupBy(m => new { m.MateriaPrimaId, m.MateriaPrima.Nombre, m.MateriaPrima.Categoria })
                .Select(g => new
                {
                    MateriaPrima = g.Key.Nombre,
                    Categoria = g.Key.Categoria.ToString(),
                    CantidadDesperdiciada = g.Sum(m => m.Cantidad),
                    CostoTotal = g.Sum(m => m.CostoTotal),
                    NumeroIncidentes = g.Count()
                })
                .OrderByDescending(x => x.CostoTotal)
                .ToListAsync();

            var totalDesperdicio = mermas.Sum(m => m.CostoTotal);

            return Ok(new
            {
                Success = true,
                Data = mermas,
                TotalDesperdicio = totalDesperdicio,
                Periodo = new { Desde = fechaDesde, Hasta = fechaHasta }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener desperdicio de materias primas");
            return StatusCode(500, new { Success = false, Message = "Error al obtener datos", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene un dashboard completo con todos los KPIs
    /// </summary>
    [HttpGet("completo")]
    public async Task<ActionResult> GetDashboardCompleto(
        [FromQuery] int? sucursalId = null,
        [FromQuery] DateTime? fecha = null)
    {
        try
        {
            var fechaConsulta = fecha ?? DateTime.UtcNow.Date;

            // Reutilizar los métodos existentes
            var kpisResult = await GetKPIs(sucursalId, fechaConsulta);
            var tamalesResult = await GetTamalesMasVendidos(sucursalId, fechaConsulta.AddDays(-30), fechaConsulta, 5);
            var bebidasResult = await GetBebidasPorHorario(sucursalId, fechaConsulta);
            var picanteResult = await GetProporcionPicante(sucursalId, fechaConsulta.AddDays(-30), fechaConsulta);
            var utilidadesResult = await GetUtilidadesPorLinea(sucursalId, fechaConsulta.AddMonths(-1), fechaConsulta);
            var desperdicioResult = await GetDesperdicioMateriasPrimas(fechaConsulta.AddDays(-30), fechaConsulta);

            // Extraer datos de los ActionResults
            var dashboard = new
            {
                Fecha = fechaConsulta,
                SucursalId = sucursalId,
                KPIs = GetDataFromActionResult(kpisResult),
                TamalesMasVendidos = GetDataFromActionResult(tamalesResult),
                BebidasPorHorario = GetDataFromActionResult(bebidasResult),
                ProporcionPicante = GetDataFromActionResult(picanteResult),
                UtilidadesPorLinea = GetDataFromActionResult(utilidadesResult),
                DesperdicioMateriasPrimas = GetDataFromActionResult(desperdicioResult)
            };

            return Ok(new
            {
                Success = true,
                Data = dashboard,
                Message = "Dashboard completo generado exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar dashboard completo");
            return StatusCode(500, new { Success = false, Message = "Error al generar dashboard", Error = ex.Message });
        }
    }

    // Métodos auxiliares
    private static string ObtenerFranjaHoraria(int hora)
    {
        return hora switch
        {
            >= 0 and < 6 => "Madrugada",
            >= 6 and < 12 => "Mañana",
            >= 12 and < 18 => "Tarde",
            _ => "Noche"
        };
    }

    private static int OrdenFranjaHoraria(string franja)
    {
        return franja switch
        {
            "Madrugada" => 0,
            "Mañana" => 1,
            "Tarde" => 2,
            "Noche" => 3,
            _ => 4
        };
    }

    private static object? GetDataFromActionResult(ActionResult result)
    {
        if (result is OkObjectResult okResult && okResult.Value != null)
        {
            var value = okResult.Value;
            var dataProperty = value.GetType().GetProperty("Data");
            return dataProperty?.GetValue(value);
        }
        return null;
    }
}