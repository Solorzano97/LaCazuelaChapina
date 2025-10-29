namespace LaCazuelaChapina.Domain.Entities;

public class VentaDetalle
{
    public int Id { get; set; }
    public int VentaId { get; set; }
    public int? ProductoId { get; set; }
    public int? ComboId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public string? ConfiguracionJson { get; set; }

    // Navegación
    public virtual Venta Venta { get; set; } = null!;
    public virtual Producto? Producto { get; set; }
    public virtual Combo? Combo { get; set; }
}