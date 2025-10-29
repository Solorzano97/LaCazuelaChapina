// ============================================
// ARCHIVO: src/LaCazuelaChapina.API/Controllers/InventarioController.cs
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
public class InventarioController : ControllerBase
{
    private readonly CazuelaChapinaContext _context;
    private readonly ILogger<InventarioController> _logger;

    public InventarioController(
        CazuelaChapinaContext context,
        ILogger<InventarioController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las materias primas
    /// </summary>
    [HttpGet("materias-primas")]
    public async Task<ActionResult> GetMateriasPrimas(
        [FromQuery] CategoriaMateriaPrima? categoria = null,
        [FromQuery] bool? soloCriticos = null)
    {
        try
        {
            var query = _context.MateriasPrimas.AsQueryable();

            if (categoria.HasValue)
                query = query.Where(m => m.Categoria == categoria.Value);

            if (soloCriticos == true)
                query = query.Where(m => m.PuntoCritico);

            var materias = await query
                .OrderBy(m => m.Categoria)
                .ThenBy(m => m.Nombre)
                .ToListAsync();

            // Calcular estadísticas
            var estadisticas = new
            {
                Total = materias.Count,
                EnPuntoCritico = materias.Count(m => m.PuntoCritico),
                StockBajo = materias.Count(m => !m.PuntoCritico && m.StockActual <= m.StockMinimo * 1.5m),
                ValorTotal = materias.Sum(m => m.StockActual * m.CostoPromedio)
            };

            return Ok(new
            {
                Success = true,
                Data = materias,
                Estadisticas = estadisticas
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener materias primas");
            return StatusCode(500, new { Success = false, Message = "Error al obtener inventario", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene una materia prima por ID
    /// </summary>
    [HttpGet("materias-primas/{id}")]
    public async Task<ActionResult> GetMateriaPrima(int id)
    {
        try
        {
            var materia = await _context.MateriasPrimas
                .Include(m => m.Movimientos.OrderByDescending(mov => mov.Fecha).Take(10))
                .Include(m => m.Recetas)
                    .ThenInclude(r => r.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (materia == null)
                return NotFound(new { Success = false, Message = "Materia prima no encontrada" });

            return Ok(new { Success = true, Data = materia });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener materia prima {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al obtener materia prima", Error = ex.Message });
        }
    }

    /// <summary>
    /// Registra un movimiento de inventario
    /// </summary>
    [HttpPost("movimientos")]
    public async Task<ActionResult> RegistrarMovimiento([FromBody] MovimientoRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var materia = await _context.MateriasPrimas.FindAsync(request.MateriaPrimaId);
            if (materia == null)
                return BadRequest(new { Success = false, Message = "Materia prima no encontrada" });

            var usuario = await _context.Usuarios.FindAsync(request.UsuarioId);
            if (usuario == null)
                return BadRequest(new { Success = false, Message = "Usuario no encontrado" });

            // Validar stock para salidas y mermas
            if ((request.Tipo == TipoMovimiento.Salida || request.Tipo == TipoMovimiento.Merma) 
                && materia.StockActual < request.Cantidad)
            {
                return BadRequest(new { Success = false, Message = "Stock insuficiente" });
            }

            // Crear movimiento
            var movimiento = new MovimientoInventario
            {
                MateriaPrimaId = request.MateriaPrimaId,
                Tipo = request.Tipo,
                Cantidad = request.Cantidad,
                CostoUnitario = request.CostoUnitario,
                CostoTotal = request.Cantidad * request.CostoUnitario,
                Motivo = request.Motivo,
                UsuarioId = request.UsuarioId
            };

            _context.MovimientosInventario.Add(movimiento);

            // Actualizar stock
            if (request.Tipo == TipoMovimiento.Entrada)
            {
                // Actualizar costo promedio ponderado
                var costoTotal = (materia.StockActual * materia.CostoPromedio) + (request.Cantidad * request.CostoUnitario);
                var cantidadTotal = materia.StockActual + request.Cantidad;
                materia.CostoPromedio = cantidadTotal > 0 ? costoTotal / cantidadTotal : request.CostoUnitario;
                materia.StockActual += request.Cantidad;
            }
            else
            {
                materia.StockActual -= request.Cantidad;
            }

            // Actualizar punto crítico
            materia.PuntoCritico = materia.StockActual <= materia.StockMinimo;
            materia.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Si es punto crítico, crear notificación
            if (materia.PuntoCritico)
            {
                await CrearNotificacionStockCriticoAsync(materia);
            }

            await transaction.CommitAsync();

            _logger.LogInformation("Movimiento registrado: {Tipo} - {Materia} - {Cantidad}", 
                request.Tipo, materia.Nombre, request.Cantidad);

            return Ok(new
            {
                Success = true,
                Data = movimiento,
                StockActual = materia.StockActual,
                PuntoCritico = materia.PuntoCritico,
                Message = "Movimiento registrado exitosamente"
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error al registrar movimiento");
            return StatusCode(500, new { Success = false, Message = "Error al registrar movimiento", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene el historial de movimientos
    /// </summary>
    [HttpGet("movimientos")]
    public async Task<ActionResult> GetMovimientos(
        [FromQuery] int? materiaPrimaId = null,
        [FromQuery] TipoMovimiento? tipo = null,
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null,
        [FromQuery] int pagina = 1,
        [FromQuery] int porPagina = 50)
    {
        try
        {
            var query = _context.MovimientosInventario
                .Include(m => m.MateriaPrima)
                .Include(m => m.Usuario)
                .AsQueryable();

            if (materiaPrimaId.HasValue)
                query = query.Where(m => m.MateriaPrimaId == materiaPrimaId.Value);

            if (tipo.HasValue)
                query = query.Where(m => m.Tipo == tipo.Value);

            if (desde.HasValue)
                query = query.Where(m => m.Fecha >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(m => m.Fecha <= hasta.Value);

            var total = await query.CountAsync();
            var movimientos = await query
                .OrderByDescending(m => m.Fecha)
                .Skip((pagina - 1) * porPagina)
                .Take(porPagina)
                .ToListAsync();

            return Ok(new
            {
                Success = true,
                Data = movimientos,
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
            _logger.LogError(ex, "Error al obtener movimientos");
            return StatusCode(500, new { Success = false, Message = "Error al obtener movimientos", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene el resumen de inventario por categoría
    /// </summary>
    [HttpGet("resumen")]
    public async Task<ActionResult> GetResumen()
    {
        try
        {
            var materias = await _context.MateriasPrimas.ToListAsync();

            var resumen = materias
                .GroupBy(m => m.Categoria)
                .Select(g => new
                {
                    Categoria = g.Key.ToString(),
                    TotalItems = g.Count(),
                    EnPuntoCritico = g.Count(m => m.PuntoCritico),
                    ValorTotal = g.Sum(m => m.StockActual * m.CostoPromedio),
                    Items = g.Select(m => new
                    {
                        m.Id,
                        m.Nombre,
                        m.StockActual,
                        m.UnidadMedida,
                        m.PuntoCritico
                    })
                })
                .ToList();

            return Ok(new { Success = true, Data = resumen });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener resumen de inventario");
            return StatusCode(500, new { Success = false, Message = "Error al obtener resumen", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene las materias primas en punto crítico
    /// </summary>
    [HttpGet("alertas")]
    public async Task<ActionResult> GetAlertas()
    {
        try
        {
            var criticos = await _context.MateriasPrimas
                .Where(m => m.PuntoCritico)
                .OrderBy(m => m.StockActual)
                .ToListAsync();

            var stockBajo = await _context.MateriasPrimas
                .Where(m => !m.PuntoCritico && m.StockActual <= m.StockMinimo * 1.5m)
                .OrderBy(m => m.StockActual)
                .ToListAsync();

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    Criticos = criticos,
                    StockBajo = stockBajo,
                    TotalAlertas = criticos.Count + stockBajo.Count
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener alertas");
            return StatusCode(500, new { Success = false, Message = "Error al obtener alertas", Error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza el stock mínimo de una materia prima
    /// </summary>
    [HttpPatch("materias-primas/{id}/stock-minimo")]
    public async Task<ActionResult> ActualizarStockMinimo(int id, [FromBody] ActualizarStockMinimoRequest request)
    {
        try
        {
            var materia = await _context.MateriasPrimas.FindAsync(id);
            if (materia == null)
                return NotFound(new { Success = false, Message = "Materia prima no encontrada" });

            materia.StockMinimo = request.StockMinimo;
            materia.PuntoCritico = materia.StockActual <= materia.StockMinimo;
            materia.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Data = materia,
                Message = "Stock mínimo actualizado"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar stock mínimo");
            return StatusCode(500, new { Success = false, Message = "Error al actualizar", Error = ex.Message });
        }
    }

    // Método auxiliar para crear notificaciones
    private async Task CrearNotificacionStockCriticoAsync(MateriaPrima materia)
    {
        var gerentes = await _context.Usuarios
            .Where(u => u.Rol == RolUsuario.Gerente || u.Rol == RolUsuario.Admin)
            .Where(u => u.Activo)
            .ToListAsync();

        foreach (var gerente in gerentes)
        {
            var notificacion = new Notificacion
            {
                UsuarioId = gerente.Id,
                Tipo = TipoNotificacion.Inventario,
                Titulo = "⚠️ Stock Crítico",
                Mensaje = $"La materia prima \"{materia.Nombre}\" ha alcanzado el punto crítico. Stock actual: {materia.StockActual} {materia.UnidadMedida}"
            };

            _context.Notificaciones.Add(notificacion);
        }

        await _context.SaveChangesAsync();
    }
}

// ============================================
// DTOs para Requests
// ============================================

public class MovimientoRequest
{
    public int MateriaPrimaId { get; set; }
    public TipoMovimiento Tipo { get; set; }
    public decimal Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
    public string? Motivo { get; set; }
    public int UsuarioId { get; set; }
}

public class ActualizarStockMinimoRequest
{
    public decimal StockMinimo { get; set; }
}