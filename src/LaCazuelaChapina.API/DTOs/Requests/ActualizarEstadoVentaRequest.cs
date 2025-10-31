using System.ComponentModel.DataAnnotations;
using LaCazuelaChapina.Domain.Enums;

namespace LaCazuelaChapina.API.DTOs.Requests;

public class ActualizarEstadoVentaRequest
{
    [Required(ErrorMessage = "El estado es requerido")]
    public EstadoVenta Estado { get; set; }
}

