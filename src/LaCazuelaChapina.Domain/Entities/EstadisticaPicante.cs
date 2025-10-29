namespace LaCazuelaChapina.Domain.Entities;

public class EstadisticaPicante
{
    public int Id { get; set; }
    public int PicanteTamalId { get; set; }
    public int CantidadVendida { get; set; }
    public DateTime Fecha { get; set; }
    public int SucursalId { get; set; }

    // Navegación
    public virtual PicanteTamal PicanteTamal { get; set; } = null!;
    public virtual Sucursal Sucursal { get; set; } = null!;
}