// ============================================
// ARCHIVO: src/LaCazuelaChapina.API/Controllers/CombosController.cs
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
public class CombosController : ControllerBase
{
    private readonly CazuelaChapinaContext _context;
    private readonly ILogger<CombosController> _logger;

    public CombosController(
        CazuelaChapinaContext context,
        ILogger<CombosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los combos activos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetCombos(
        [FromQuery] TipoCombo? tipo = null,
        [FromQuery] bool? soloActivos = true)
    {
        try
        {
            var query = _context.Combos
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Producto)
                .AsQueryable();

            if (soloActivos == true)
                query = query.Where(c => c.Activo);

            if (tipo.HasValue)
                query = query.Where(c => c.Tipo == tipo.Value);

            // Filtrar combos vigentes
            var ahora = DateTime.UtcNow;
            query = query.Where(c => 
                (c.VigenciaInicio == null || c.VigenciaInicio <= ahora) &&
                (c.VigenciaFin == null || c.VigenciaFin >= ahora));

            var combos = await query
                .OrderBy(c => c.Tipo)
                .ThenBy(c => c.Nombre)
                .ToListAsync();

            return Ok(new
            {
                Success = true,
                Data = combos,
                Count = combos.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener combos");
            return StatusCode(500, new { Success = false, Message = "Error al obtener combos", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene un combo por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetCombo(int id)
    {
        try
        {
            var combo = await _context.Combos
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (combo == null)
                return NotFound(new { Success = false, Message = "Combo no encontrado" });

            return Ok(new { Success = true, Data = combo });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener combo {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al obtener combo", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene el combo estacional actual (editable sin redeploy)
    /// </summary>
    [HttpGet("estacional")]
    public async Task<ActionResult> GetComboEstacional()
    {
        try
        {
            var ahora = DateTime.UtcNow;
            
            var comboEstacional = await _context.Combos
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Producto)
                .Where(c => c.Tipo == TipoCombo.Estacional && c.Activo)
                .Where(c => 
                    (c.VigenciaInicio == null || c.VigenciaInicio <= ahora) &&
                    (c.VigenciaFin == null || c.VigenciaFin >= ahora))
                .FirstOrDefaultAsync();

            if (comboEstacional == null)
            {
                return Ok(new
                {
                    Success = false,
                    Message = "No hay combo estacional activo en este momento",
                    Sugerencia = "Los combos estacionales varían según la época: Fiambre (noviembre), Quema del Diablo (diciembre), Cuaresma (Semana Santa)"
                });
            }

            return Ok(new
            {
                Success = true,
                Data = comboEstacional,
                Message = "Combo estacional actual"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener combo estacional");
            return StatusCode(500, new { Success = false, Message = "Error al obtener combo estacional", Error = ex.Message });
        }
    }

    /// <summary>
    /// Crea un nuevo combo
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CrearCombo([FromBody] CrearComboRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // Validar que el código sea único
            if (await _context.Combos.AnyAsync(c => c.Codigo == request.Codigo))
                return BadRequest(new { Success = false, Message = "Ya existe un combo con ese código" });

            var combo = new Combo
            {
                Nombre = request.Nombre,
                Codigo = request.Codigo,
                Tipo = request.Tipo,
                Precio = request.Precio,
                Descripcion = request.Descripcion,
                Editable = request.Editable,
                VigenciaInicio = request.VigenciaInicio,
                VigenciaFin = request.VigenciaFin,
                Activo = true
            };

            // Agregar detalles
            foreach (var detalle in request.Detalles)
            {
                var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                if (producto == null)
                    return BadRequest(new { Success = false, Message = $"Producto {detalle.ProductoId} no encontrado" });

                combo.Detalles.Add(new ComboDetalle
                {
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad,
                    ConfiguracionJson = detalle.ConfiguracionJson
                });
            }

            _context.Combos.Add(combo);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Combo creado: {Nombre} ({Codigo})", combo.Nombre, combo.Codigo);

            // Recargar con relaciones
            var comboCompleto = await _context.Combos
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(c => c.Id == combo.Id);

            return CreatedAtAction(nameof(GetCombo), new { id = combo.Id }, new
            {
                Success = true,
                Data = comboCompleto,
                Message = "Combo creado exitosamente"
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error al crear combo");
            return StatusCode(500, new { Success = false, Message = "Error al crear combo", Error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza un combo existente (ideal para combos estacionales)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> ActualizarCombo(int id, [FromBody] ActualizarComboRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var combo = await _context.Combos
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (combo == null)
                return NotFound(new { Success = false, Message = "Combo no encontrado" });

            // Solo permitir editar si es editable o es admin (por simplicidad permitimos siempre)
            if (!combo.Editable && combo.Tipo != TipoCombo.Estacional)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Este combo no es editable. Solo los combos estacionales pueden modificarse sin redeploy."
                });
            }

            // Actualizar propiedades básicas
            combo.Nombre = request.Nombre;
            combo.Precio = request.Precio;
            combo.Descripcion = request.Descripcion;
            combo.VigenciaInicio = request.VigenciaInicio;
            combo.VigenciaFin = request.VigenciaFin;
            combo.Activo = request.Activo;
            combo.UpdatedAt = DateTime.UtcNow;

            // Actualizar detalles si se proporcionan
            if (request.Detalles != null && request.Detalles.Any())
            {
                // Eliminar detalles anteriores
                _context.CombosDetalle.RemoveRange(combo.Detalles);

                // Agregar nuevos detalles
                foreach (var detalle in request.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto == null)
                        return BadRequest(new { Success = false, Message = $"Producto {detalle.ProductoId} no encontrado" });

                    combo.Detalles.Add(new ComboDetalle
                    {
                        ProductoId = detalle.ProductoId,
                        Cantidad = detalle.Cantidad,
                        ConfiguracionJson = detalle.ConfiguracionJson
                    });
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Combo actualizado: {Nombre} (ID: {Id})", combo.Nombre, id);

            // Recargar con relaciones
            var comboActualizado = await _context.Combos
                .Include(c => c.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(c => c.Id == id);

            return Ok(new
            {
                Success = true,
                Data = comboActualizado,
                Message = "Combo actualizado exitosamente sin necesidad de redeploy"
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error al actualizar combo {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al actualizar combo", Error = ex.Message });
        }
    }

    /// <summary>
    /// Activa o desactiva un combo
    /// </summary>
    [HttpPatch("{id}/estado")]
    public async Task<ActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoComboRequest request)
    {
        try
        {
            var combo = await _context.Combos.FindAsync(id);
            if (combo == null)
                return NotFound(new { Success = false, Message = "Combo no encontrado" });

            combo.Activo = request.Activo;
            combo.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Estado de combo {Id} cambiado a {Activo}", id, request.Activo);

            return Ok(new
            {
                Success = true,
                Data = combo,
                Message = $"Combo {(request.Activo ? "activado" : "desactivado")} exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado del combo {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al cambiar estado", Error = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un combo (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> EliminarCombo(int id)
    {
        try
        {
            var combo = await _context.Combos.FindAsync(id);
            if (combo == null)
                return NotFound(new { Success = false, Message = "Combo no encontrado" });

            combo.Activo = false;
            combo.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Combo {Id} desactivado (soft delete)", id);

            return Ok(new { Success = true, Message = "Combo eliminado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar combo {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al eliminar combo", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene los combos más vendidos
    /// </summary>
    [HttpGet("mas-vendidos")]
    public async Task<ActionResult> GetCombosMasVendidos([FromQuery] int top = 5)
    {
        try
        {
            var combosMasVendidos = await _context.VentasDetalle
                .Include(vd => vd.Combo)
                .Where(vd => vd.ComboId != null)
                .GroupBy(vd => new { vd.ComboId, vd.Combo!.Nombre })
                .Select(g => new
                {
                    ComboId = g.Key.ComboId,
                    Nombre = g.Key.Nombre,
                    VecesVendido = g.Count(),
                    UnidadesVendidas = g.Sum(vd => vd.Cantidad),
                    TotalVentas = g.Sum(vd => vd.Subtotal)
                })
                .OrderByDescending(x => x.VecesVendido)
                .Take(top)
                .ToListAsync();

            return Ok(new
            {
                Success = true,
                Data = combosMasVendidos
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener combos más vendidos");
            return StatusCode(500, new { Success = false, Message = "Error al obtener estadísticas", Error = ex.Message });
        }
    }
}

// ============================================
// DTOs para Requests
// ============================================

public class CrearComboRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public TipoCombo Tipo { get; set; }
    public decimal Precio { get; set; }
    public string? Descripcion { get; set; }
    public bool Editable { get; set; }
    public DateTime? VigenciaInicio { get; set; }
    public DateTime? VigenciaFin { get; set; }
    public List<ComboDetalleRequest> Detalles { get; set; } = new();
}

public class ActualizarComboRequest
{
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime? VigenciaInicio { get; set; }
    public DateTime? VigenciaFin { get; set; }
    public List<ComboDetalleRequest>? Detalles { get; set; }
}

public class ComboDetalleRequest
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public string? ConfiguracionJson { get; set; }
}

public class CambiarEstadoComboRequest
{
    public bool Activo { get; set; }
}