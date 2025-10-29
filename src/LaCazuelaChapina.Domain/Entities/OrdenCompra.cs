using LaCazuelaChapina.Domain.Enums;

namespace LaCazuelaChapina.Domain.Entities;

public class OrdenCompra
{
    public int Id { get; set; }
    public int ProveedorId { get; set; }
    public int UsuarioId { get; set; }
    public decimal Total { get; set; }
    public EstadoOrden Estado { get; set; }
    public DateTime FechaOrden { get; set; } = DateTime.UtcNow;
    public DateTime? FechaEntrega { get; set; }

    // Navegación
    public virtual Proveedor Proveedor { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual ICollection<OrdenCompraDetalle> Detalles { get; set; } = new List<OrdenCompraDetalle>();
}