namespace LaCazuelaChapina.API.Common;

/// <summary>
/// Respuesta estandarizada de la API
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public object? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> Ok(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message ?? "Operación exitosa",
        Timestamp = DateTime.UtcNow
    };

    public static ApiResponse<T> Error(string message, object? errors = null) => new()
    {
        Success = false,
        Message = message,
        Errors = errors,
        Timestamp = DateTime.UtcNow
    };

    public static ApiResponse<T> Created(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message ?? "Recurso creado exitosamente",
        Timestamp = DateTime.UtcNow
    };
}

public class PaginatedResponse<T>
{
    public bool Success { get; set; } = true;
    public T? Data { get; set; }
    public PaginationInfo? Pagination { get; set; }
    public string? Message { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static PaginatedResponse<T> Create(
        T data,
        int pagina,
        int porPagina,
        int total,
        string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message,
        Pagination = new PaginationInfo
        {
            Pagina = pagina,
            PorPagina = porPagina,
            Total = total,
            TotalPaginas = (int)Math.Ceiling(total / (double)porPagina)
        },
        Timestamp = DateTime.UtcNow
    };
}

public class PaginationInfo
{
    public int Pagina { get; set; }
    public int PorPagina { get; set; }
    public int Total { get; set; }
    public int TotalPaginas { get; set; }
}

