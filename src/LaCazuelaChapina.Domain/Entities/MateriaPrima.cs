using LaCazuelaChapina.Domain.Enums;

namespace LaCazuelaChapina.Domain.Entities;

public class MateriaPrima
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public CategoriaMateriaPrima Categoria { get; set; }
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal StockActual { get; set; }
    public decimal StockMinimo { get; set; }
    public decimal CostoPromedio { get; set; }
    public bool PuntoCritico { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    public virtual ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();
    public virtual ICollection<RecetaProducto> Recetas { get; set; } = new List<RecetaProducto>();
    public virtual ICollection<OrdenCompraDetalle> OrdenesDetalle { get; set; } = new List<OrdenCompraDetalle>();
}
