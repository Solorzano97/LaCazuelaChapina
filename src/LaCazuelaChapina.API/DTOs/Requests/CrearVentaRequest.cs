using System.ComponentModel.DataAnnotations;
using LaCazuelaChapina.API.DTOs.Requests.Detalles;

namespace LaCazuelaChapina.API.DTOs.Requests;

public class CrearVentaRequest
{
    [Required(ErrorMessage = "La sucursal es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de sucursal debe ser válido")]
    public int SucursalId { get; set; }

    [Required(ErrorMessage = "El usuario es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de usuario debe ser válido")]
    public int UsuarioId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El descuento no puede ser negativo")]
    public decimal Descuento { get; set; } = 0;

    public bool Sincronizada { get; set; } = true;

    [Required(ErrorMessage = "Los detalles de la venta son requeridos")]
    [MinLength(1, ErrorMessage = "Debe haber al menos un detalle en la venta")]
    public List<VentaDetalleRequest> Detalles { get; set; } = new();
}

