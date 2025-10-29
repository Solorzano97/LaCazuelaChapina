using Microsoft.EntityFrameworkCore;
using LaCazuelaChapina.Domain.Entities;

namespace LaCazuelaChapina.Infrastructure.Data;

public class CazuelaChapinaContext : DbContext
{
    public CazuelaChapinaContext(DbContextOptions<CazuelaChapinaContext> options)
        : base(options)
    {
    }

    // ============================================
    // DBSETS - TABLAS PRINCIPALES
    // ============================================
    
    // Usuarios y Sucursales
    public DbSet<Sucursal> Sucursales { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    // Productos
    public DbSet<Producto> Productos { get; set; }
    
    // Atributos de Tamales
    public DbSet<MasaTamal> MasasTamal { get; set; }
    public DbSet<RellenoTamal> RellenosTamal { get; set; }
    public DbSet<EnvolturaTamal> EnvolturasTamal { get; set; }
    public DbSet<PicanteTamal> PicantesTamal { get; set; }

    // Atributos de Bebidas
    public DbSet<TipoBebida> TiposBebida { get; set; }
    public DbSet<EndulzanteBebida> EndulzantesBebida { get; set; }
    public DbSet<ToppingBebida> ToppingsBebida { get; set; }

    // Combos
    public DbSet<Combo> Combos { get; set; }
    public DbSet<ComboDetalle> CombosDetalle { get; set; }

    // Inventario
    public DbSet<MateriaPrima> MateriasPrimas { get; set; }
    public DbSet<MovimientoInventario> MovimientosInventario { get; set; }
    public DbSet<RecetaProducto> RecetasProducto { get; set; }

    // Ventas
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<VentaDetalle> VentasDetalle { get; set; }

    // Notificaciones
    public DbSet<Notificacion> Notificaciones { get; set; }

    // Reportes
    public DbSet<ReporteDiario> ReportesDiarios { get; set; }
    public DbSet<PreferenciaBebida> PreferenciasBebida { get; set; }
    public DbSet<EstadisticaPicante> EstadisticasPicante { get; set; }

    // Módulos Opcionales
    public DbSet<Vaporera> Vaporeras { get; set; }
    public DbSet<LoteCoccion> LotesCoccion { get; set; }
    public DbSet<Proveedor> Proveedores { get; set; }
    public DbSet<OrdenCompra> OrdenesCompra { get; set; }
    public DbSet<OrdenCompraDetalle> OrdenesCompraDetalle { get; set; }

    // ============================================
    // CONFIGURACIÓN DEL MODELO
    // ============================================
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas las configuraciones de entidades
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CazuelaChapinaContext).Assembly);

        // ============================================
        // CONFIGURAR NOMBRES DE TABLAS (SIN PLURALIZAR)
        // ============================================
        
        modelBuilder.Entity<Sucursal>().ToTable("Sucursal");
        modelBuilder.Entity<Usuario>().ToTable("Usuario");
        modelBuilder.Entity<Producto>().ToTable("Producto");
        modelBuilder.Entity<MasaTamal>().ToTable("MasaTamal");
        modelBuilder.Entity<RellenoTamal>().ToTable("RellenoTamal");
        modelBuilder.Entity<EnvolturaTamal>().ToTable("EnvolturaTamal");
        modelBuilder.Entity<PicanteTamal>().ToTable("PicanteTamal");
        modelBuilder.Entity<TipoBebida>().ToTable("TipoBebida");
        modelBuilder.Entity<EndulzanteBebida>().ToTable("EndulzanteBebida");
        modelBuilder.Entity<ToppingBebida>().ToTable("ToppingBebida");
        modelBuilder.Entity<Combo>().ToTable("Combo");
        modelBuilder.Entity<ComboDetalle>().ToTable("ComboDetalle");
        modelBuilder.Entity<MateriaPrima>().ToTable("MateriaPrima");
        modelBuilder.Entity<MovimientoInventario>().ToTable("MovimientoInventario");
        modelBuilder.Entity<RecetaProducto>().ToTable("RecetaProducto");
        modelBuilder.Entity<Venta>().ToTable("Venta");
        modelBuilder.Entity<VentaDetalle>().ToTable("VentaDetalle");
        modelBuilder.Entity<Notificacion>().ToTable("Notificacion");
        modelBuilder.Entity<ReporteDiario>().ToTable("ReporteDiario");
        modelBuilder.Entity<PreferenciaBebida>().ToTable("PreferenciaBebida");
        modelBuilder.Entity<EstadisticaPicante>().ToTable("EstadisticaPicante");
        modelBuilder.Entity<Vaporera>().ToTable("Vaporera");
        modelBuilder.Entity<LoteCoccion>().ToTable("LoteCoccion");
        modelBuilder.Entity<Proveedor>().ToTable("Proveedor");
        modelBuilder.Entity<OrdenCompra>().ToTable("OrdenCompra");
        modelBuilder.Entity<OrdenCompraDetalle>().ToTable("OrdenCompraDetalle");

        // ============================================
        // CONVERSIÓN DE ENUMS A STRINGS
        // ============================================
        
        // Usuario
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Rol)
            .HasConversion<string>();

        // Producto
        modelBuilder.Entity<Producto>()
            .Property(p => p.Tipo)
            .HasConversion<string>();

        // Combo
        modelBuilder.Entity<Combo>()
            .Property(c => c.Tipo)
            .HasConversion<string>();

        // MateriaPrima
        modelBuilder.Entity<MateriaPrima>()
            .Property(m => m.Categoria)
            .HasConversion<string>();

        // MovimientoInventario
        modelBuilder.Entity<MovimientoInventario>()
            .Property(m => m.Tipo)
            .HasConversion<string>();

        // Venta
        modelBuilder.Entity<Venta>()
            .Property(v => v.Estado)
            .HasConversion<string>();

        // Notificacion
        modelBuilder.Entity<Notificacion>()
            .Property(n => n.Tipo)
            .HasConversion<string>();

        // PreferenciaBebida
        modelBuilder.Entity<PreferenciaBebida>()
            .Property(p => p.FranjaHoraria)
            .HasConversion<string>();

        // Vaporera
        modelBuilder.Entity<Vaporera>()
            .Property(v => v.Estado)
            .HasConversion<string>();

        // LoteCoccion
        modelBuilder.Entity<LoteCoccion>()
            .Property(l => l.Estado)
            .HasConversion<string>();

        // OrdenCompra
        modelBuilder.Entity<OrdenCompra>()
            .Property(o => o.Estado)
            .HasConversion<string>();

        // ============================================
        // ÍNDICES PERSONALIZADOS
        // ============================================
        
        modelBuilder.Entity<Venta>()
            .HasIndex(v => v.NumeroOrden)
            .IsUnique();

        modelBuilder.Entity<Venta>()
            .HasIndex(v => new { v.SucursalId, v.Fecha });

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<MateriaPrima>()
            .HasIndex(m => m.PuntoCritico);

        modelBuilder.Entity<ReporteDiario>()
            .HasIndex(r => new { r.SucursalId, r.Fecha })
            .IsUnique();

        // ============================================
        // CONFIGURACIONES DE PRECISIÓN DECIMAL
        // ============================================
        
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                {
                    property.SetPrecision(10);
                    property.SetScale(2);
                }
            }
        }

        // ============================================
        // CONFIGURACIÓN DE CASCADE DELETE
        // ============================================
        
        // ComboDetalle se elimina al eliminar Combo
        modelBuilder.Entity<ComboDetalle>()
            .HasOne(cd => cd.Combo)
            .WithMany(c => c.Detalles)
            .HasForeignKey(cd => cd.ComboId)
            .OnDelete(DeleteBehavior.Cascade);

        // VentaDetalle se elimina al eliminar Venta
        modelBuilder.Entity<VentaDetalle>()
            .HasOne(vd => vd.Venta)
            .WithMany(v => v.Detalles)
            .HasForeignKey(vd => vd.VentaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Notificaciones se eliminan al eliminar Usuario
        modelBuilder.Entity<Notificacion>()
            .HasOne(n => n.Usuario)
            .WithMany(u => u.Notificaciones)
            .HasForeignKey(n => n.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // RecetaProducto se elimina al eliminar Producto
        modelBuilder.Entity<RecetaProducto>()
            .HasOne(r => r.Producto)
            .WithMany(p => p.Recetas)
            .HasForeignKey(r => r.ProductoId)
            .OnDelete(DeleteBehavior.Cascade);

        // OrdenCompraDetalle se elimina al eliminar OrdenCompra
        modelBuilder.Entity<OrdenCompraDetalle>()
            .HasOne(o => o.OrdenCompra)
            .WithMany(oc => oc.Detalles)
            .HasForeignKey(o => o.OrdenCompraId)
            .OnDelete(DeleteBehavior.Cascade);

        // ============================================
        // CONFIGURACIÓN DE CONSTRAINTS
        // ============================================
        
        // VentaDetalle debe tener ProductoId O ComboId (no ambos)
        modelBuilder.Entity<VentaDetalle>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_VentaDetalle_ProductoOCombo",
                "(`ProductoId` IS NOT NULL AND `ComboId` IS NULL) OR (`ProductoId` IS NULL AND `ComboId` IS NOT NULL)"
            ));

        // ============================================
        // VALORES POR DEFECTO
        // ============================================
        
        modelBuilder.Entity<Sucursal>()
            .Property(s => s.Activa)
            .HasDefaultValue(true);

        modelBuilder.Entity<Usuario>()
            .Property(u => u.Activo)
            .HasDefaultValue(true);

        modelBuilder.Entity<Producto>()
            .Property(p => p.Activo)
            .HasDefaultValue(true);

        modelBuilder.Entity<Combo>()
            .Property(c => c.Activo)
            .HasDefaultValue(true);

        modelBuilder.Entity<MateriaPrima>()
            .Property(m => m.PuntoCritico)
            .HasDefaultValue(false);

        modelBuilder.Entity<Venta>()
            .Property(v => v.Sincronizada)
            .HasDefaultValue(true);

        modelBuilder.Entity<Notificacion>()
            .Property(n => n.Leida)
            .HasDefaultValue(false);

        // ============================================
        // TIMESTAMPS AUTOMÁTICOS
        // ============================================
        
        modelBuilder.Entity<Sucursal>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Usuario>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Producto>()
            .Property(p => p.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Producto>()
            .Property(p => p.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();

        modelBuilder.Entity<Combo>()
            .Property(c => c.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Combo>()
            .Property(c => c.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();

        modelBuilder.Entity<MateriaPrima>()
            .Property(m => m.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<MateriaPrima>()
            .Property(m => m.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();

        modelBuilder.Entity<MovimientoInventario>()
            .Property(m => m.Fecha)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Venta>()
            .Property(v => v.Fecha)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Venta>()
            .Property(v => v.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Notificacion>()
            .Property(n => n.Fecha)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<ReporteDiario>()
            .Property(r => r.GeneradoAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<OrdenCompra>()
            .Property(o => o.FechaOrden)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<LoteCoccion>()
            .Property(l => l.Inicio)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }

    // ============================================
    // OVERRIDE SAVECHANGES PARA AUDITORÍA
    // ============================================
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Aquí podrías agregar lógica de auditoría automática
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            // Lógica personalizada si es necesario
            // Por ejemplo, registrar quién hizo el cambio
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}