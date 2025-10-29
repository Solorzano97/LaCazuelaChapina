namespace LaCazuelaChapina.Domain.Entities;

public class Proveedor
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Contacto { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public bool Activo { get; set; } = true;

    // Navegación
    public virtual ICollection<OrdenCompra> OrdenesCompra { get; set; } = new List<OrdenCompra>();
}