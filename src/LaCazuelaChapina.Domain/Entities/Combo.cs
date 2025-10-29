using LaCazuelaChapina.Domain.Enums;

namespace LaCazuelaChapina.Domain.Entities;

public class Combo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public TipoCombo Tipo { get; set; }
    public decimal Precio { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    public bool Editable { get; set; } = false;
    public DateTime? VigenciaInicio { get; set; }
    public DateTime? VigenciaFin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    public virtual ICollection<ComboDetalle> Detalles { get; set; } = new List<ComboDetalle>();
    public virtual ICollection<VentaDetalle> VentasDetalle { get; set; } = new List<VentaDetalle>();
}