using LaCazuelaChapina.Domain.Enums;

namespace LaCazuelaChapina.Domain.Entities;

public class PreferenciaBebida
{
    public int Id { get; set; }
    public int TipoBebidaId { get; set; }
    public FranjaHoraria FranjaHoraria { get; set; }
    public int CantidadVendida { get; set; }
    public DateTime Fecha { get; set; }
    public int SucursalId { get; set; }

    // Navegación
    public virtual TipoBebida TipoBebida { get; set; } = null!;
    public virtual Sucursal Sucursal { get; set; } = null!;
}