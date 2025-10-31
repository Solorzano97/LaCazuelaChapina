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
        var apiKeyRaw = configuration["OpenRouter:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKeyRaw))
        {
            throw new ArgumentException("OpenRouter ApiKey no está configurada en appsettings.json");
        }
        
        // Limpiar la API Key (eliminar espacios)
        _apiKey = apiKeyRaw.Trim();
        
        // Validar formato básico de la API Key
        if (!_apiKey.StartsWith("sk-or-v1-", StringComparison.OrdinalIgnoreCase) && 
            !_apiKey.StartsWith("sk-", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("La API Key no tiene el formato esperado de OpenRouter. Debería comenzar con 'sk-or-v1-' o 'sk-'");
        }
        
        _baseUrl = configuration["OpenRouter:BaseUrl"] ?? "https://openrouter.ai/api/v1";
        _defaultModel = configuration["OpenRouter:DefaultModel"] ?? "meta-llama/llama-3.2-3b-instruct:free";
        _maxTokens = int.Parse(configuration["OpenRouter:MaxTokens"] ?? "1000");
        _temperature = double.Parse(configuration["OpenRouter:Temperature"] ?? "0.7");
        
        _logger.LogInformation("OpenRouter configurado. API Key presente: {HasKey}, Modelo: {Model}", 
            !string.IsNullOrEmpty(_apiKey), _defaultModel);

        // Configurar HttpClient
        // NO usar BaseAddress para evitar problemas con rutas relativas
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.Timeout = TimeSpan.FromSeconds(60);
        
        _logger.LogInformation("OpenRouterService configurado. BaseUrl: {BaseUrl}, Model: {Model}", _baseUrl, _defaultModel);
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

            // Asegurar que la URL esté bien formada
            var baseUrl = _baseUrl.TrimEnd('/');
            var requestUrl = $"{baseUrl}/chat/completions";
            
            _logger.LogInformation("Enviando request a OpenRouter");
            _logger.LogInformation("URL completa: {Url}", requestUrl);
            _logger.LogInformation("Model: {Model}", _defaultModel);
            _logger.LogInformation("API Key presente: {HasKey}", !string.IsNullOrEmpty(_apiKey));
            _logger.LogInformation("API Key length: {Length}", _apiKey?.Length ?? 0);
            _logger.LogInformation("API Key (primeros 15 chars): {KeyPrefix}", _apiKey?.Substring(0, Math.Min(15, _apiKey?.Length ?? 0)));

            // Validar que la API Key no esté vacía antes de enviar
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new InvalidOperationException("La API Key de OpenRouter está vacía. Por favor, configura una API Key válida en appsettings.json");
            }

            // Crear request manualmente para tener control total
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Content = content;
            
            // Configurar Authorization header - asegurar que se envíe correctamente
            if (!string.IsNullOrWhiteSpace(_apiKey))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey.Trim());
                _logger.LogInformation("Header Authorization configurado: Bearer {KeyPrefix}...", 
                    _apiKey.Trim().Substring(0, Math.Min(15, _apiKey.Trim().Length)));
            }
            
            request.Headers.Add("HTTP-Referer", "https://lacazuelachapina.com");
            request.Headers.Add("X-Title", "La Cazuela Chapina");
            
            // Agregar Content-Type explícitamente
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            
            // Log de todos los headers que se están enviando (sin la API Key completa por seguridad)
            _logger.LogInformation("Headers enviados: Authorization=Bearer ***, HTTP-Referer={Referer}, X-Title={Title}", 
                "https://lacazuelachapina.com", "La Cazuela Chapina");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Status Code: {StatusCode}", response.StatusCode);
            _logger.LogInformation("Response Headers: {Headers}", string.Join(", ", response.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}")));
            
            // Si la respuesta es HTML, es un error
            if (responseContent.TrimStart().StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase) || 
                responseContent.TrimStart().StartsWith("<html", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError("OpenRouter devolvió HTML en lugar de JSON. Posible error de autenticación o URL incorrecta.");
                _logger.LogError("Response completa (primeros 1000 chars): {Response}", 
                    responseContent.Length > 1000 ? responseContent.Substring(0, 1000) : responseContent);
                
                return $"Error: OpenRouter devolvió una página HTML en lugar de JSON. " +
                       $"Esto generalmente indica que la URL está incorrecta, la API Key es inválida, o hay un problema de autenticación. " +
                       $"Status Code: {response.StatusCode}. " +
                       $"Verifica tu API Key en appsettings.json y que la URL sea correcta.";
            }
            
            _logger.LogInformation("Response (primeros 500 chars): {Response}", 
                responseContent.Length > 500 ? responseContent.Substring(0, 500) : responseContent);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error en OpenRouter: {StatusCode} - {Response}", response.StatusCode, responseContent);
                
                // Intentar parsear el error para dar un mensaje más específico
                string errorMessage = "Error desconocido";
                try
                {
                    var errorDoc = JsonDocument.Parse(responseContent);
                    if (errorDoc.RootElement.TryGetProperty("error", out var errorObj))
                    {
                        if (errorObj.TryGetProperty("message", out var message))
                        {
                            errorMessage = message.GetString() ?? "Error desconocido";
                        }
                        if (errorObj.TryGetProperty("code", out var code))
                        {
                            errorMessage += $" (Código: {code.GetInt32()})";
                        }
                    }
                }
                catch
                {
                    errorMessage = responseContent.Length > 200 ? responseContent.Substring(0, 200) : responseContent;
                }
                
                // Mensajes específicos según el código de error
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || 
                    errorMessage.Contains("User not found", StringComparison.OrdinalIgnoreCase))
                {
                    return $"❌ Error de autenticación: La API Key de OpenRouter no es válida o ha expirado.\n\n" +
                           $"**Solución:**\n" +
                           $"1. Ve a https://openrouter.ai/keys\n" +
                           $"2. Crea una nueva API Key o verifica que la actual sea válida\n" +
                           $"3. Actualiza la API Key en appsettings.json en la sección 'OpenRouter:ApiKey'\n" +
                           $"4. Reinicia la aplicación\n\n" +
                           $"Error técnico: {errorMessage}";
                }
                
                return $"Error al conectar con OpenRouter (Status: {response.StatusCode}).\n\n" +
                       $"Detalles: {errorMessage}\n\n" +
                       $"Verifica tu API Key en appsettings.json y que el servicio de OpenRouter esté disponible.";
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