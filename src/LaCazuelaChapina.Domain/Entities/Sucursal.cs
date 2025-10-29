namespace LaCazuelaChapina.Domain.Entities;

public class Sucursal
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public bool Activa { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    public virtual ICollection<Vaporera> Vaporeras { get; set; } = new List<Vaporera>();
    public virtual ICollection<ReporteDiario> Reportes { get; set; } = new List<ReporteDiario>();
}