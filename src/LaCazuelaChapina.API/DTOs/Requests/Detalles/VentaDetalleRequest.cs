using System.ComponentModel.DataAnnotations;

namespace LaCazuelaChapina.API.DTOs.Requests.Detalles;

public class VentaDetalleRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "El ID de producto debe ser válido")]
    public int? ProductoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El ID de combo debe ser válido")]
    public int? ComboId { get; set; }

    [Required(ErrorMessage = "La cantidad es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Cantidad { get; set; }

    [MaxLength(5000, ErrorMessage = "La configuración JSON no puede exceder 5000 caracteres")]
    public string? ConfiguracionJson { get; set; }
}

