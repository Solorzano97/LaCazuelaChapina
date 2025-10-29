using LaCazuelaChapina.Domain.Enums;

namespace LaCazuelaChapina.Domain.Entities;

public class Notificacion
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public TipoNotificacion Tipo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public bool Leida { get; set; } = false;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    // Navegación
    public virtual Usuario Usuario { get; set; } = null!;
}