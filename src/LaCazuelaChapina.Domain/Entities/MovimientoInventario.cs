using LaCazuelaChapina.Domain.Enums;

namespace LaCazuelaChapina.Domain.Entities;

public class MovimientoInventario
{
    public int Id { get; set; }
    public int MateriaPrimaId { get; set; }
    public TipoMovimiento Tipo { get; set; }
    public decimal Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal CostoTotal { get; set; }
    public string? Motivo { get; set; }
    public int UsuarioId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    // Navegación
    public virtual MateriaPrima MateriaPrima { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
}