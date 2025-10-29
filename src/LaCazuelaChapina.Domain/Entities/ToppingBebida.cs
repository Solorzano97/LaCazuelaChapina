namespace LaCazuelaChapina.Domain.Entities;

public class ToppingBebida
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal CostoAdicional { get; set; }
    public bool Activo { get; set; } = true;
}