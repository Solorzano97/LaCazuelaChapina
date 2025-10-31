using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using LaCazuelaChapina.Infrastructure.Data;
using LaCazuelaChapina.Domain.Entities;
using LaCazuelaChapina.Domain.Enums;

namespace LaCazuelaChapina.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly CazuelaChapinaContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        CazuelaChapinaContext context,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Login con Google OAuth
    /// </summary>
    [HttpPost("google")]
    public async Task<ActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        try
        {
            // Validar el token de Google
            var payload = await ValidarTokenGoogle(request.Credential);
            
            if (payload == null)
            {
                return Unauthorized(new { Success = false, Message = "Token de Google inválido" });
            }

            // Buscar o crear usuario
            var usuario = await _context.Usuarios
                .Include(u => u.Sucursal)
                .FirstOrDefaultAsync(u => u.Email == payload.Email);

            if (usuario == null)
            {
                // Crear nuevo usuario si no existe
                // Por defecto, asignar a la primera sucursal y rol vendedor
                var primerasSucursal = await _context.Sucursales.FirstOrDefaultAsync();
                
                if (primerasSucursal == null)
                {
                    return BadRequest(new { 
                        Success = false, 
                        Message = "No hay sucursales disponibles. Contacte al administrador." 
                    });
                }

                usuario = new Usuario
                {
                    Nombre = payload.Name,
                    Email = payload.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()), // Password random para OAuth
                    Rol = RolUsuario.Vendedor,
                    SucursalId = primerasSucursal.Id,
                    Activo = true
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Nuevo usuario creado vía Google OAuth: {Email}", usuario.Email);
            }
            else if (!usuario.Activo)
            {
                return Unauthorized(new { 
                    Success = false, 
                    Message = "Usuario desactivado. Contacte al administrador." 
                });
            }

            // Generar JWT
            var token = GenerarJwtToken(usuario);

            _logger.LogInformation("Usuario autenticado vía Google: {Email}", usuario.Email);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    Token = token,
                    Usuario = new
                    {
                        usuario.Id,
                        usuario.Nombre,
                        usuario.Email,
                        usuario.Rol,
                        usuario.SucursalId,
                        Sucursal = usuario.Sucursal.Nombre
                    }
                },
                Message = "Autenticación exitosa"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en login con Google");
            return StatusCode(500, new { 
                Success = false, 
                Message = "Error al autenticar con Google", 
                Error = ex.Message 
            });
        }
    }

    /// <summary>
    /// Login tradicional con email y password
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Sucursal)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (usuario == null)
            {
                return Unauthorized(new { Success = false, Message = "Credenciales inválidas" });
            }

            if (!usuario.Activo)
            {
                return Unauthorized(new { 
                    Success = false, 
                    Message = "Usuario desactivado. Contacte al administrador." 
                });
            }

            // Verificar password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
            {
                return Unauthorized(new { Success = false, Message = "Credenciales inválidas" });
            }

            // Generar JWT
            var token = GenerarJwtToken(usuario);

            _logger.LogInformation("Usuario autenticado: {Email}", usuario.Email);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    Token = token,
                    Usuario = new
                    {
                        usuario.Id,
                        usuario.Nombre,
                        usuario.Email,
                        usuario.Rol,
                        usuario.SucursalId,
                        Sucursal = usuario.Sucursal.Nombre
                    }
                },
                Message = "Login exitoso"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en login");
            return StatusCode(500, new { 
                Success = false, 
                Message = "Error en login", 
                Error = ex.Message 
            });
        }
    }

    /// <summary>
    /// Registro de nuevo usuario
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            // Validar que el email no esté en uso
            if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { Success = false, Message = "El email ya está registrado" });
            }

            // Validar que la sucursal exista
            var sucursal = await _context.Sucursales.FindAsync(request.SucursalId);
            if (sucursal == null)
            {
                return BadRequest(new { Success = false, Message = "Sucursal no encontrada" });
            }

            // Hash del password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var usuario = new Usuario
            {
                Nombre = request.Nombre,
                Email = request.Email,
                PasswordHash = passwordHash,
                Rol = RolUsuario.Vendedor, // Por defecto vendedor
                SucursalId = request.SucursalId,
                Activo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Nuevo usuario registrado: {Email}", usuario.Email);

            // Generar JWT
            var token = GenerarJwtToken(usuario);

            return Ok(new
            {
                Success = true,
                Data = new
                {
                    Token = token,
                    Usuario = new
                    {
                        usuario.Id,
                        usuario.Nombre,
                        usuario.Email,
                        usuario.Rol,
                        usuario.SucursalId
                    }
                },
                Message = "Usuario registrado exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en registro");
            return StatusCode(500, new { 
                Success = false, 
                Message = "Error al registrar usuario", 
                Error = ex.Message 
            });
        }
    }

    /// <summary>
    /// Verificar token JWT actual
    /// </summary>
    [HttpGet("verify")]
    public async Task<ActionResult> VerifyToken()
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { Success = false, Message = "No se proporcionó token" });
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            
            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (emailClaim == null)
            {
                return Unauthorized(new { Success = false, Message = "Token inválido" });
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Sucursal)
                .FirstOrDefaultAsync(u => u.Email == emailClaim.Value);

            if (usuario == null || !usuario.Activo)
            {
                return Unauthorized(new { Success = false, Message = "Usuario no válido" });
            }

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
                    Sucursal = usuario.Sucursal.Nombre
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verificando token");
            return Unauthorized(new { Success = false, Message = "Token inválido" });
        }
    }

    // ============================================
    // MÉTODOS AUXILIARES
    // ============================================

    private async Task<GoogleJsonWebSignature.Payload?> ValidarTokenGoogle(string credential)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _configuration["Google:ClientId"] }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);
            return payload;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validando token de Google");
            return null;
        }
    }

    private string GenerarJwtToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
            new Claim("SucursalId", usuario.SucursalId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(
            int.Parse(_configuration["Jwt:ExpiryInMinutes"] ?? "1440")
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// ============================================
// DTOs
// ============================================

public class GoogleLoginRequest
{
    public string Credential { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int SucursalId { get; set; }
}