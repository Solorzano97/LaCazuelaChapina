// ============================================
// ARCHIVO: src/LaCazuelaChapina.Infrastructure/Services/MockOpenRouterService.cs
// SERVICIO DE PRUEBA - Usar mientras obtienes API Key real
// ============================================

using Microsoft.Extensions.Logging;

namespace LaCazuelaChapina.Infrastructure.Services;

public class MockOpenRouterService : IOpenRouterService
{
    private readonly ILogger<MockOpenRouterService> _logger;

    public MockOpenRouterService(ILogger<MockOpenRouterService> logger)
    {
        _logger = logger;
    }

    public async Task<string> GenerarRespuestaAsync(string prompt, string? contexto = null)
    {
        _logger.LogWarning("⚠️ Usando MockOpenRouterService - Respuestas simuladas");
        
        await Task.Delay(500); // Simular latencia de API

        return @"**Respuesta Simulada de IA**

Los tamales más populares en Guatemala son:

1. **Tamal de Recado Rojo** - El clásico tamal guatemalteco con carne de cerdo y recado rojo, envuelto en hoja de plátano.

2. **Tamal Negro (Tamal Colorado)** - Preparado con recado negro y pollo, tiene un sabor distintivo y es muy popular en ocasiones especiales.

3. **Chuchitos** - Más pequeños que los tamales tradicionales, se envuelven en tusa de maíz y son perfectos para el desayuno.

4. **Tamal de Chipilín** - Opción vegetariana popular, con hierbas de chipilín que le dan un sabor único.

Estos tamales se disfrutan especialmente los jueves y domingos, acompañados de atol de elote o cacao caliente.

*Nota: Esta es una respuesta simulada. Configura tu API Key de OpenRouter para obtener respuestas reales de IA.*";
    }

    public async Task<string> SugerirComboPersonalizadoAsync(string preferenciasCliente)
    {
        _logger.LogWarning("⚠️ Usando MockOpenRouterService");
        await Task.Delay(500);

        return $@"**Combo Personalizado Sugerido** 🎁

Basándonos en tus preferencias:
{preferenciasCliente}

**Te recomendamos el ""Combo Chapín Familiar"":**

🫔 **Tamales:**
- 6 Tamales de Recado Rojo de Cerdo con picante chapín
- 6 Tamales Negro de Pollo (nivel suave)
- Todos envueltos en hoja de plátano tradicional

🥤 **Bebidas:**
- 2 Jarros de 1L de Atol de Elote con panela
- 1 Jarro de 1L de Cacao Batido con malvaviscos

💰 **Precio Estimado:** Q150.00

✨ **¿Por qué este combo?**
Esta combinación es perfecta porque combina los sabores tradicionales guatemaltecos que disfrutas, con variedad para toda la familia. El atol de elote complementa perfectamente el picante de los tamales, mientras que el cacao batido añade ese toque especial para los más pequeños.

*Nota: Respuesta simulada. Usa API Key real para sugerencias personalizadas con IA.*";
    }

    public async Task<string> AnalizarVentasAsync(string datosVentas)
    {
        _logger.LogWarning("⚠️ Usando MockOpenRouterService");
        await Task.Delay(500);

        return $@"**Análisis de Ventas** 📊

Datos analizados:
{datosVentas.Substring(0, Math.Min(200, datosVentas.Length))}...

**Tendencias Identificadas:**

1. **Alta demanda de tamales tradicionales** - Los tamales de recado rojo siguen siendo los más vendidos
2. **Preferencia por picante moderado** - El nivel ""Suave"" es el más popular
3. **Bebidas calientes lideran** - Atol de elote tiene mayor demanda

**Recomendaciones:**

✅ Aumentar stock de recado rojo para los fines de semana
✅ Considerar combo familiar con variedad de picantes
✅ Promocionar bebidas frías en temporada calurosa
✅ Ofrecer descuento en combos de docena

**Oportunidades:**
- Crear combo ""Tarde Chapina"" con tamales y atol
- Introducir tamal del mes con ingredientes especiales

*Nota: Análisis simulado. API Key real proporciona insights más detallados.*";
    }

    public async Task<string> RecomendarProductosAsync(string historialCliente)
    {
        _logger.LogWarning("⚠️ Usando MockOpenRouterService");
        await Task.Delay(500);

        return $@"**Recomendaciones Personalizadas** 🎯

Basándonos en tu historial de compras:
{historialCliente.Substring(0, Math.Min(150, historialCliente.Length))}...

**Te recomendamos probar:**

1. **Combo ""Madrugada del 24""** (Q420)
   - 3 docenas de tamales variados
   - 4 jarros de bebidas
   - Termo de barro conmemorativo
   
   *¿Por qué?* Has comprado tamales en grandes cantidades antes, este combo es ideal para celebraciones familiares.

2. **Tamal de Chipilín Vegetariano** (Q8)
   
   *¿Por qué?* Notamos que disfrutas variedad, este sabor único complementará tus favoritos.

3. **Bebida: Pinol con Canela** (Q15)
   
   *¿Por qué?* Si te gusta el atol de elote, el pinol te encantará por su textura similar pero sabor más intenso.

*Nota: Recomendaciones simuladas. API Key real analiza patrones más complejos.*";
    }

    public async Task<string> OptimizarInventarioAsync(string datosInventario)
    {
        _logger.LogWarning("⚠️ Usando MockOpenRouterService");
        await Task.Delay(500);

        return $@"**Optimización de Inventario** 📦

Estado actual analizado:
{datosInventario.Substring(0, Math.Min(200, datosInventario.Length))}...

**Acciones Urgentes:**

🔴 **Reposición Inmediata:**
- Harina de maíz amarillo: Pedir 100 kg
- Hojas de plátano: Pedir 500 unidades
- Recado rojo: Pedir 10 kg

⚠️ **Stock Bajo (próximos 3 días):**
- Pollo: Considerar pedido de 30 kg
- Panela: 50 kg recomendados

**Predicción de Demanda:**
- Fin de semana: +40% en ventas de tamales
- Jueves tradicional: +25% en bebidas calientes

**Sugerencias para Reducir Desperdicio:**
✅ Preparar lotes más pequeños de chipilín (menor rotación)
✅ Usar masa de maíz blanco antes del viernes
✅ Monitorear temperatura de almacenamiento de proteínas

**Órdenes Sugeridas:**
1. Orden prioritaria a Distribuidora La Esperanza (masas)
2. Orden semanal a Carnicería El Buen Sabor
3. Orden quincenal de empaques

*Nota: Optimización simulada. API Key real usa machine learning para predicciones más precisas.*";
    }
}