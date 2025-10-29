namespace LaCazuelaChapina.Domain.Entities;

public class OrdenCompraDetalle
{
    public int Id { get; set; }
    public int OrdenCompraId { get; set; }
    public int MateriaPrimaId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }

    // Navegación
    public virtual OrdenCompra OrdenCompra { get; set; } = null!;
    public virtual MateriaPrima MateriaPrima { get; set; } = null!;
}