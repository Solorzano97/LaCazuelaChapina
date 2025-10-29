# La Cazuela Chapina - API

Sistema integral para gestiÃ³n de ventas de tamales y bebidas tradicionales guatemaltecas.

## íº€ TecnologÃ­as

- .NET 8.0
- MySQL 8.0+
- Entity Framework Core
- AutoMapper
- FluentValidation
- Swagger/OpenAPI
- JWT Authentication

## í³ Arquitectura

```
LaCazuelaChapinaAPI/
â”œâ”€â”€ src/
â”‚   â”œâ”€â”€ LaCazuelaChapina.API/           # API REST Controllers
â”‚   â”œâ”€â”€ LaCazuelaChapina.Application/   # LÃ³gica de negocio
â”‚   â”œâ”€â”€ LaCazuelaChapina.Domain/        # Entidades y contratos
â”‚   â””â”€â”€ LaCazuelaChapina.Infrastructure/ # EF Core, Repositorios
â””â”€â”€ tests/
    â””â”€â”€ LaCazuelaChapina.Tests/         # Pruebas unitarias
```

## í» ï¸ ConfiguraciÃ³n

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

4. **Ejecutar la aplicaciÃ³n:**
   ```bash
   dotnet run --project src/LaCazuelaChapina.API
   ```

5. **Acceder a Swagger:**
   ```
   https://localhost:7001/swagger
   ```

## í³š Endpoints Principales

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

## í·ª Testing

```bash
dotnet test
```

## í³ Licencia

Proyecto privado - La Cazuela Chapina
