using LaCazuelaChapina.Domain.Enums;

namespace LaCazuelaChapina.Domain.Entities;

public class Venta
{
    public int Id { get; set; }
    public string NumeroOrden { get; set; } = string.Empty;
    public int SucursalId { get; set; }
    public int UsuarioId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    public EstadoVenta Estado { get; set; }
    public bool Sincronizada { get; set; } = true;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    public virtual Sucursal Sucursal { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual ICollection<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();
}