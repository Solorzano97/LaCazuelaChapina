using System.ComponentModel.DataAnnotations;
using LaCazuelaChapina.Domain.Enums;

namespace LaCazuelaChapina.API.DTOs.Requests;

public class CrearUsuarioRequest
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    [StringLength(255, ErrorMessage = "El email no puede exceder 255 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "El rol es requerido")]
    public RolUsuario Rol { get; set; }

    [Required(ErrorMessage = "La sucursal es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de sucursal debe ser válido")]
    public int SucursalId { get; set; }
}

