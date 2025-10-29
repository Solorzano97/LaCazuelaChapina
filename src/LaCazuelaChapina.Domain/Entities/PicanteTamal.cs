namespace LaCazuelaChapina.Domain.Entities;

public class PicanteTamal
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal CostoAdicional { get; set; }
    public bool Activo { get; set; } = true;

    // Navegación
    public virtual ICollection<EstadisticaPicante> Estadisticas { get; set; } = new List<EstadisticaPicante>();
}