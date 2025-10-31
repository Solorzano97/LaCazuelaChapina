// ============================================
// ARCHIVO: src/LaCazuelaChapina.API/Controllers/UsuariosController.cs
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
public class UsuariosController : ControllerBase
{
    private readonly CazuelaChapinaContext _context;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(
        CazuelaChapinaContext context,
        ILogger<UsuariosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los usuarios
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetUsuarios(
        [FromQuery] int? sucursalId = null,
        [FromQuery] RolUsuario? rol = null,
        [FromQuery] bool? soloActivos = true)
    {
        try
        {
            var query = _context.Usuarios
                .Include(u => u.Sucursal)
                .AsQueryable();

            if (soloActivos == true)
                query = query.Where(u => u.Activo);

            if (sucursalId.HasValue)
                query = query.Where(u => u.SucursalId == sucursalId.Value);

            if (rol.HasValue)
                query = query.Where(u => u.Rol == rol.Value);

            var usuarios = await query
                .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Email,
                    u.Rol,
                    u.SucursalId,
                    Sucursal = u.Sucursal.Nombre,
                    u.Activo,
                    u.CreatedAt
                })
                .OrderBy(u => u.Nombre)
                .ToListAsync();

            return Ok(new { Success = true, Data = usuarios, Count = usuarios.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios");
            return StatusCode(500, new { Success = false, Message = "Error al obtener usuarios", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene un usuario por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetUsuario(int id)
    {
        try
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Sucursal)
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Email,
                    u.Rol,
                    u.SucursalId,
                    Sucursal = u.Sucursal.Nombre,
                    u.Activo,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound(new { Success = false, Message = "Usuario no encontrado" });

            return Ok(new { Success = true, Data = usuario });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al obtener usuario", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene las notificaciones de un usuario
    /// </summary>
    [HttpGet("{id}/notificaciones")]
    public async Task<ActionResult> GetNotificaciones(
        int id,
        [FromQuery] bool? soloNoLeidas = true,
        [FromQuery] int limite = 50)
    {
        try
        {
            var query = _context.Notificaciones
                .Where(n => n.UsuarioId == id);

            if (soloNoLeidas == true)
                query = query.Where(n => !n.Leida);

            var notificaciones = await query
                .OrderByDescending(n => n.Fecha)
                .Take(limite)
                .ToListAsync();

            return Ok(new
            {
                Success = true,
                Data = notificaciones,
                Total = notificaciones.Count,
                NoLeidas = notificaciones.Count(n => !n.Leida)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener notificaciones del usuario {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al obtener notificaciones", Error = ex.Message });
        }
    }

    /// <summary>
    /// Marca una notificación como leída
    /// </summary>
    [HttpPatch("notificaciones/{notificacionId}/marcar-leida")]
    public async Task<ActionResult> MarcarNotificacionLeida(int notificacionId)
    {
        try
        {
            var notificacion = await _context.Notificaciones.FindAsync(notificacionId);
            if (notificacion == null)
                return NotFound(new { Success = false, Message = "Notificación no encontrada" });

            notificacion.Leida = true;
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Notificación marcada como leída" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al marcar notificación {Id} como leída", notificacionId);
            return StatusCode(500, new { Success = false, Message = "Error al actualizar", Error = ex.Message });
        }
    }

    /// <summary>
    /// Marca todas las notificaciones de un usuario como leídas
    /// </summary>
    [HttpPatch("{id}/notificaciones/marcar-todas-leidas")]
    public async Task<ActionResult> MarcarTodasLeidas(int id)
    {
        try
        {
            var notificaciones = await _context.Notificaciones
                .Where(n => n.UsuarioId == id && !n.Leida)
                .ToListAsync();

            foreach (var notificacion in notificaciones)
            {
                notificacion.Leida = true;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = $"{notificaciones.Count} notificaciones marcadas como leídas"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al marcar todas las notificaciones como leídas para usuario {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al actualizar", Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene estadísticas de desempeño de un usuario
    /// </summary>
    [HttpGet("{id}/estadisticas")]
    public async Task<ActionResult> GetEstadisticasUsuario(
        int id,
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null)
    {
        try
        {
            var fechaDesde = desde ?? DateTime.UtcNow.AddMonths(-1);
            var fechaHasta = hasta ?? DateTime.UtcNow;

            var ventas = await _context.Ventas
                .Where(v => v.UsuarioId == id)
                .Where(v => v.Fecha >= fechaDesde && v.Fecha <= fechaHasta)
                .Where(v => v.Estado == EstadoVenta.Completada)
                .ToListAsync();

            var estadisticas = new
            {
                Periodo = new { Desde = fechaDesde, Hasta = fechaHasta },
                TotalVentas = ventas.Count,
                MontoTotal = ventas.Sum(v => v.Total),
                MontoPromedio = ventas.Any() ? ventas.Average(v => v.Total) : 0,
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
            _logger.LogError(ex, "Error al obtener estadísticas del usuario {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al obtener estadísticas", Error = ex.Message });
        }
    }

    /// <summary>
    /// Crea un nuevo usuario
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CrearUsuario([FromBody] CrearUsuarioRequest request)
    {
        try
        {
            // Validar que el email no esté en uso
            if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
                return BadRequest(new { Success = false, Message = "El email ya está en uso" });

            // Validar que la sucursal exista
            var sucursal = await _context.Sucursales.FindAsync(request.SucursalId);
            if (sucursal == null)
                return BadRequest(new { Success = false, Message = "Sucursal no encontrada" });

            // Hash del password (en producción usar BCrypt)
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var usuario = new Usuario
            {
                Nombre = request.Nombre,
                Email = request.Email,
                PasswordHash = passwordHash,
                Rol = request.Rol,
                SucursalId = request.SucursalId,
                Activo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuario creado: {Email} - Rol: {Rol}", usuario.Email, usuario.Rol);

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, new
            {
                Success = true,
                Data = new
                {
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Email,
                    usuario.Rol,
                    usuario.SucursalId
                },
                Message = "Usuario creado exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            return StatusCode(500, new { Success = false, Message = "Error al crear usuario", Error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza un usuario
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> ActualizarUsuario(int id, [FromBody] ActualizarUsuarioRequest request)
    {
        try
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { Success = false, Message = "Usuario no encontrado" });

            // Validar email único si se está cambiando
            if (usuario.Email != request.Email && await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
                return BadRequest(new { Success = false, Message = "El email ya está en uso" });

            usuario.Nombre = request.Nombre;
            usuario.Email = request.Email;
            usuario.Rol = request.Rol;
            usuario.SucursalId = request.SucursalId;
            usuario.Activo = request.Activo;

            // Actualizar password si se proporciona
            if (!string.IsNullOrEmpty(request.NuevoPassword))
            {
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NuevoPassword);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuario actualizado: {Email} (ID: {Id})", usuario.Email, id);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Email,
                    usuario.Rol,
                    usuario.SucursalId,
                    usuario.Activo
                },
                Message = "Usuario actualizado exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al actualizar usuario", Error = ex.Message });
        }
    }

    /// <summary>
    /// Activa o desactiva un usuario
    /// </summary>
    [HttpPatch("{id}/estado")]
    public async Task<ActionResult> CambiarEstadoUsuario(int id, [FromBody] CambiarEstadoUsuarioRequest request)
    {
        try
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { Success = false, Message = "Usuario no encontrado" });

            usuario.Activo = request.Activo;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Estado de usuario {Id} cambiado a {Activo}", id, request.Activo);

            return Ok(new
            {
                Success = true,
                Message = $"Usuario {(request.Activo ? "activado" : "desactivado")} exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado del usuario {Id}", id);
            return StatusCode(500, new { Success = false, Message = "Error al cambiar estado", Error = ex.Message });
        }
    }

}

// ============================================
// DTOs
// ============================================

public class CrearUsuarioRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public int SucursalId { get; set; }
}

public class ActualizarUsuarioRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? NuevoPassword { get; set; }
    public RolUsuario Rol { get; set; }
    public int SucursalId { get; set; }
    public bool Activo { get; set; }
}

public class CambiarEstadoUsuarioRequest
{
    public bool Activo { get; set; }
}