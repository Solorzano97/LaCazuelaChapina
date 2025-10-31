# La Cazuela Chapina - API

Sistema integral para gestión de ventas de tamales y bebidas tradicionales guatemaltecas.

## 🔧 Tecnologías

- .NET 8.0
- MySQL 8.0+
- Entity Framework Core
- AutoMapper
- FluentValidation
- Swagger/OpenAPI
- JWT Authentication

## 🏗️ Arquitectura

```
LaCazuelaChapinaAPI/
├── src/
│   ├── LaCazuelaChapina.API/           # API REST Controllers
│   ├── LaCazuelaChapina.Application/   # Lógica de negocio
│   ├── LaCazuelaChapina.Domain/        # Entidades y contratos
│   └── LaCazuelaChapina.Infrastructure/ # EF Core, Repositorios
└── tests/
    └── LaCazuelaChapina.Tests/         # Pruebas unitarias
```

## ⚙️ Configuración

### 1. Configurar appsettings.json

**⚠️ IMPORTANTE:** El archivo `appsettings.json` contiene información sensible y **NO debe subirse al repositorio**.

1. **Copia el archivo de ejemplo:**
   ```bash
   cp src/LaCazuelaChapina.API/appsettings.example.json src/LaCazuelaChapina.API/appsettings.json
   ```
   
   O en Windows PowerShell:
   ```powershell
   Copy-Item src\LaCazuelaChapina.API\appsettings.example.json src\LaCazuelaChapina.API\appsettings.json
   ```

2. **Edita `src/LaCazuelaChapina.API/appsettings.json`** y configura los siguientes valores:

#### 📊 Base de Datos
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=127.0.0.1;Port=3306;Database=LaCazuelaChapina;User=TU_USUARIO;Password=TU_PASSWORD;CharSet=utf8mb4;AllowPublicKeyRetrieval=True;SslMode=None;"
}
```

#### 🔐 JWT (Token de autenticación)
```json
"Jwt": {
  "Key": "TU_CLAVE_SECRETA_MINIMO_32_CARACTERES_AQUI",
  "Issuer": "LaCazuelaChapinaAPI",
  "Audience": "LaCazuelaChapinaClients",
  "ExpiryInMinutes": 1440
}
```
**Nota:** Genera una clave secreta segura de al menos 32 caracteres. Puedes usar cualquier texto largo y único.

#### 🤖 OpenRouter API Key (Para funciones de IA)

**Es necesario para que funcionen las funciones de IA del sistema (recomendaciones, análisis, etc.)**

1. Ve a https://openrouter.ai/keys
2. Crea una cuenta o inicia sesión (si no tienes una)
3. Haz clic en "Create Key" o "Nueva Key"
4. Copia la API Key completa (debe empezar con `sk-or-v1-` o `sk-`)
5. Actualiza en `appsettings.json`:
```json
"OpenRouter": {
  "ApiKey": "sk-or-v1-TU_API_KEY_AQUI",
  "BaseUrl": "https://openrouter.ai/api/v1",
  "DefaultModel": "meta-llama/llama-3.2-3b-instruct:free",
  "MaxTokens": 1000,
  "Temperature": 0.7
}
```

**⚠️ Sin esta API Key, las funciones de IA usarán respuestas simuladas.**

#### 🔑 Google OAuth (Para autenticación con Google)

**Es necesario para permitir que los usuarios inicien sesión con su cuenta de Google**

1. Ve a https://console.cloud.google.com/
2. Crea un proyecto nuevo o selecciona uno existente
3. Habilita la API "Google Sign-In API"
4. Ve a "Credenciales" → "Crear credenciales" → "ID de cliente OAuth 2.0"
5. Configura:
   - Tipo de aplicación: Aplicación web
   - Orígenes JavaScript autorizados: `http://localhost:5173` (o el puerto de tu frontend)
   - URI de redirección autorizados: `http://localhost:5173` (o el puerto de tu frontend)
6. Copia el Client ID (debe terminar en `.apps.googleusercontent.com`)
7. Actualiza en `appsettings.json`:
```json
"Google": {
  "ClientId": "TU_CLIENT_ID_AQUI.apps.googleusercontent.com"
}
```

### 2. Restaurar base de datos MySQL

```bash
mysql -u root -p < database/schema.sql
```

### 3. Ejecutar migraciones

```bash
dotnet ef database update --project src/LaCazuelaChapina.Infrastructure
```

### 4. Ejecutar la aplicación

```bash
dotnet run --project src/LaCazuelaChapina.API
```

### 5. Acceder a Swagger

```
https://localhost:7001/swagger
```

## 📡 Endpoints Principales

### Productos
- `GET /api/productos` - Listar productos
- `GET /api/productos/{id}` - Obtener producto
- `POST /api/productos` - Crear producto
- `PUT /api/productos/{id}` - Actualizar producto

### Ventas
- `GET /api/ventas` - Listar ventas
- `POST /api/ventas` - Registrar venta
- `GET /api/ventas/{id}` - Detalle de venta

### Inventario
- `GET /api/inventario` - Estado de inventario
- `POST /api/inventario/movimiento` - Registrar movimiento

### Dashboard
- `GET /api/dashboard/indicadores` - KPIs principales
- `GET /api/dashboard/reportes` - Reportes consolidados

### IA
- `POST /api/ia/pregunta` - Hacer preguntas al asistente IA
- `POST /api/ia/sugerir-combo` - Sugerir combos personalizados
- `POST /api/ia/analizar-ventas` - Análisis de ventas con IA

## 🧪 Testing

```bash
dotnet test
```

## 🔒 Seguridad

**⚠️ IMPORTANTE:** 
- ❌ **NO subas** `appsettings.json` al repositorio (ya está en `.gitignore`)
- ✅ **SÍ sube** `appsettings.example.json` como plantilla
- 🛡️ Mantén tus API Keys y contraseñas privadas
- 🔄 Si compartes el proyecto, los demás deben copiar `appsettings.example.json` a `appsettings.json` y configurar sus propias credenciales
- 🔑 Si expones una API Key por error, revócala inmediatamente en el panel de OpenRouter

## 📝 Licencia

Proyecto privado - La Cazuela Chapina
