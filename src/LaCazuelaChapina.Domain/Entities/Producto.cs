using LaCazuelaChapina.Domain.Enums;

namespace LaCazuelaChapina.Domain.Entities;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TipoProducto Tipo { get; set; }
    public decimal PrecioBase { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    public virtual ICollection<VentaDetalle> VentasDetalle { get; set; } = new List<VentaDetalle>();
    public virtual ICollection<ComboDetalle> CombosDetalle { get; set; } = new List<ComboDetalle>();
    public virtual ICollection<RecetaProducto> Recetas { get; set; } = new List<RecetaProducto>();
    public virtual ICollection<LoteCoccion> Lotes { get; set; } = new List<LoteCoccion>();
}