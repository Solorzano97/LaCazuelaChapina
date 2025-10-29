// ============================================
// ARCHIVO: src/LaCazuelaChapina.Infrastructure/Services/OpenRouterService.cs
// ============================================

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LaCazuelaChapina.Infrastructure.Services;

public interface IOpenRouterService
{
    Task<string> GenerarRespuestaAsync(string prompt, string? contexto = null);
    Task<string> SugerirComboPersonalizadoAsync(string preferenciasCliente);
    Task<string> AnalizarVentasAsync(string datosVentas);
    Task<string> RecomendarProductosAsync(string historialCliente);
    Task<string> OptimizarInventarioAsync(string datosInventario);
}

public class OpenRouterService : IOpenRouterService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenRouterService> _logger;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly string _defaultModel;
    private readonly int _maxTokens;
    private readonly double _temperature;

    public OpenRouterService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<OpenRouterService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        // Leer configuración
        _apiKey = configuration["OpenRouter:ApiKey"] ?? throw new ArgumentException("OpenRouter ApiKey no configurada");
        _baseUrl = configuration["OpenRouter:BaseUrl"] ?? "https://openrouter.ai/api/v1";
        _defaultModel = configuration["OpenRouter:DefaultModel"] ?? "meta-llama/llama-3.2-3b-instruct:free";
        _maxTokens = int.Parse(configuration["OpenRouter:MaxTokens"] ?? "1000");
        _temperature = double.Parse(configuration["OpenRouter:Temperature"] ?? "0.7");

        // Configurar HttpClient
        _httpClient.BaseAddress = new Uri(_baseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://lacazuelachapina.com");
        _httpClient.DefaultRequestHeaders.Add("X-Title", "La Cazuela Chapina");
    }

    public async Task<string> GenerarRespuestaAsync(string prompt, string? contexto = null)
    {
        try
        {
            var systemPrompt = @"Eres un asistente especializado en La Cazuela Chapina, un negocio guatemalteco de tamales y bebidas tradicionales. 
Conoces todos los productos: tamales con diferentes masas (maíz amarillo, blanco, arroz), rellenos (recado rojo de cerdo, negro de pollo, chipilín vegetariano, chuchito), 
envolturas (hoja de plátano, tusa de maíz) y niveles de picante. También bebidas como atol de elote, atole shuco, pinol y cacao batido.
Hablas de forma amigable, profesional y con conocimiento de la cultura guatemalteca.";

            if (!string.IsNullOrEmpty(contexto))
            {
                systemPrompt += $"\n\nContexto adicional: {contexto}";
            }

            var requestBody = new
            {
                model = _defaultModel,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = prompt }
                },
                max_tokens = _maxTokens,
                temperature = _temperature
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Enviando request a OpenRouter: {Model}", _defaultModel);
            _logger.LogInformation("API Key configurada: {HasKey}", !string.IsNullOrEmpty(_apiKey));

            var response = await _httpClient.PostAsync("/chat/completions", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Status Code: {StatusCode}", response.StatusCode);
            _logger.LogInformation("Response (primeros 500 chars): {Response}", 
                responseContent.Length > 500 ? responseContent.Substring(0, 500) : responseContent);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error en OpenRouter: {StatusCode} - {Response}", response.StatusCode, responseContent);
                
                // Devolver mensaje más descriptivo
                return $"Error al conectar con OpenRouter (Status: {response.StatusCode}). " +
                       $"Por favor verifica tu API Key en appsettings.json. " +
                       $"Detalles: {(responseContent.Length > 200 ? responseContent.Substring(0, 200) : responseContent)}";
            }

            // Intentar parsear JSON
            JsonDocument result;
            try
            {
                result = JsonDocument.Parse(responseContent);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error al parsear respuesta JSON. Response: {Response}", responseContent);
                return $"Error: La respuesta de OpenRouter no es JSON válido. Esto generalmente indica un problema con la API Key. Response: {responseContent.Substring(0, Math.Min(500, responseContent.Length))}";
            }

            var messageContent = result.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            _logger.LogInformation("Respuesta exitosa de OpenRouter");
            return messageContent ?? "No se pudo generar una respuesta.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar respuesta con OpenRouter. Message: {Message}", ex.Message);
            throw new Exception($"Error en OpenRouter: {ex.Message}", ex);
        }
    }

    public async Task<string> SugerirComboPersonalizadoAsync(string preferenciasCliente)
    {
        var prompt = $@"Un cliente tiene las siguientes preferencias: {preferenciasCliente}

Basándote en nuestro menú de La Cazuela Chapina, sugiere un combo personalizado que incluya:
1. Cantidad y tipo de tamales (especifica masa, relleno, envoltura, picante)
2. Bebidas recomendadas (tipo, endulzante, topping)
3. Precio estimado del combo
4. Breve explicación de por qué esta combinación es ideal para el cliente

Sé específico con los ingredientes disponibles y mantén un tono amigable y guatemalteco.";

        return await GenerarRespuestaAsync(prompt);
    }

    public async Task<string> AnalizarVentasAsync(string datosVentas)
    {
        var prompt = $@"Analiza los siguientes datos de ventas de La Cazuela Chapina:

{datosVentas}

Proporciona:
1. Principales tendencias identificadas
2. Productos más y menos vendidos
3. Recomendaciones para mejorar ventas
4. Oportunidades de nuevos combos
5. Sugerencias de gestión de inventario

Sé conciso pero informativo.";

        return await GenerarRespuestaAsync(prompt);
    }

    public async Task<string> RecomendarProductosAsync(string historialCliente)
    {
        var prompt = $@"Basándote en el historial de compras del cliente:

{historialCliente}

Recomienda 3 productos o combos que podrían interesarle, explicando por qué cada uno sería una buena opción.
Considera variedad, complementariedad y preferencias mostradas.";

        return await GenerarRespuestaAsync(prompt);
    }

    public async Task<string> OptimizarInventarioAsync(string datosInventario)
    {
        var prompt = $@"Revisa el siguiente estado de inventario de La Cazuela Chapina:

{datosInventario}

Proporciona:
1. Materias primas que necesitan reposición urgente
2. Predicción de demanda para los próximos días
3. Sugerencias para reducir desperdicio
4. Optimización de órdenes de compra
5. Alertas sobre productos en punto crítico";

        var contexto = "Eres un experto en gestión de inventarios para negocios de alimentos. Conoces las particularidades de mantener ingredientes frescos y minimizar mermas.";
        
        return await GenerarRespuestaAsync(prompt, contexto);
    }
}