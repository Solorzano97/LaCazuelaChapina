using LaCazuelaChapina.Domain.Enums;

namespace LaCazuelaChapina.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public int SucursalId { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    public virtual Sucursal Sucursal { get; set; } = null!;
    public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    public virtual ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();
    public virtual ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
    public virtual ICollection<OrdenCompra> OrdenesCompra { get; set; } = new List<OrdenCompra>();
}