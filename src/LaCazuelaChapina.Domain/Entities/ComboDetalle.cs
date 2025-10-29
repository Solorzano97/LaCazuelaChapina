namespace LaCazuelaChapina.Domain.Entities;

public class ComboDetalle
{
    public int Id { get; set; }
    public int ComboId { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public string? ConfiguracionJson { get; set; }

    // Navegación
    public virtual Combo Combo { get; set; } = null!;
    public virtual Producto Producto { get; set; } = null!;
}