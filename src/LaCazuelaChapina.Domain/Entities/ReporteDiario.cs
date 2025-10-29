namespace LaCazuelaChapina.Domain.Entities;

public class ReporteDiario
{
    public int Id { get; set; }
    public int SucursalId { get; set; }
    public DateTime Fecha { get; set; }
    public decimal VentasTotal { get; set; }
    public int TamalesVendidos { get; set; }
    public int BebidasVendidas { get; set; }
    public decimal UtilidadBruta { get; set; }
    public string? TamalMasVendido { get; set; }
    public string? BebidaMasVendida { get; set; }
    public decimal DesperdicioTotal { get; set; }
    public DateTime GeneradoAt { get; set; } = DateTime.UtcNow;

    // Navegación
    public virtual Sucursal Sucursal { get; set; } = null!;
}