namespace LaCazuelaChapina.Domain.Entities;

public class RecetaProducto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public int MateriaPrimaId { get; set; }
    public decimal CantidadNecesaria { get; set; }
    public string Unidad { get; set; } = string.Empty;

    // Navegación
    public virtual Producto Producto { get; set; } = null!;
    public virtual MateriaPrima MateriaPrima { get; set; } = null!;
}