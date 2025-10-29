using LaCazuelaChapina.Domain.Enums;

namespace LaCazuelaChapina.Domain.Entities;

public class LoteCoccion
{
    public int Id { get; set; }
    public int VaporeraId { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public DateTime Inicio { get; set; } = DateTime.UtcNow;
    public DateTime FinEstimado { get; set; }
    public DateTime? FinReal { get; set; }
    public EstadoLote Estado { get; set; }

    // Navegación
    public virtual Vaporera Vaporera { get; set; } = null!;
    public virtual Producto Producto { get; set; } = null!;
}