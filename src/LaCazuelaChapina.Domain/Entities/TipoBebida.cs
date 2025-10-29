namespace LaCazuelaChapina.Domain.Entities;

public class TipoBebida
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal CostoAdicional { get; set; }
    public bool Activo { get; set; } = true;

    // Navegación
    public virtual ICollection<PreferenciaBebida> Preferencias { get; set; } = new List<PreferenciaBebida>();
}