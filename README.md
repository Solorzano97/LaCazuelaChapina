# La Cazuela Chapina - API

Sistema integral para gestión de ventas de tamales y bebidas tradicionales guatemaltecas.

## � Tecnologías

- .NET 8.0
- MySQL 8.0+
- Entity Framework Core
- AutoMapper
- FluentValidation
- Swagger/OpenAPI
- JWT Authentication

## � Arquitectura

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

## �️ Configuración

1. **Restaurar base de datos MySQL:**
   ```bash
   mysql -u root -p < database/schema.sql
   ```

2. **Configurar connection string en appsettings.json:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=LaCazuelaChapina;User=root;Password=your_password;"
     }
   }
   ```

3. **Ejecutar migraciones:**
   ```bash
   dotnet ef database update --project src/LaCazuelaChapina.Infrastructure
   ```

4. **Ejecutar la aplicación:**
   ```bash
   dotnet run --project src/LaCazuelaChapina.API
   ```

5. **Acceder a Swagger:**
   ```
   https://localhost:7001/swagger
   ```

## � Endpoints Principales

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

## � Testing

```bash
dotnet test
```

## � Licencia

Proyecto privado - La Cazuela Chapina
