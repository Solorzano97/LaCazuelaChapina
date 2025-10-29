using LaCazuelaChapina.Domain.Enums;

namespace LaCazuelaChapina.Domain.Entities;

public class Vaporera
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public int SucursalId { get; set; }
    public EstadoVaporera Estado { get; set; }
    public decimal? TemperaturaActual { get; set; }
    public DateTime? UltimoLote { get; set; }

    // Navegación
    public virtual Sucursal Sucursal { get; set; } = null!;
    public virtual ICollection<LoteCoccion> Lotes { get; set; } = new List<LoteCoccion>();
}