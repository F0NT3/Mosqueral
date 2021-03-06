﻿namespace BiomasaEUPT.Modelos
{
    using BiomasaEUPT.Modelos.Tablas;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.EntityClient;
    using System.Data.Entity.Validation;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Z.EntityFramework.Plus;

    public class BiomasaEUPTContext : DbContext
    {
        public BiomasaEUPTContext()
        : base("name=BiomasaEUPTContext")
        //: base(nameOrConnectionString: ConnectionString())       
        {
            //Database.Connection.ConnectionString = Database.Connection.ConnectionString.Replace("********", "usuario");

            // CUIDADO -> Borra la tabla y la vuelve a crear
            //Database.SetInitializer(new BiomasaEUPTContextInitializer());

            //Configuration.AutoDetectChangesEnabled = false;

            // https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Audit-%7C-Entity-Framework-Audit-Trail-Context-and-Track-Changes
            AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
             // añadir "Where(x => x.AuditEntryID == 0)" para permitir múltiples SaveChanges con el mismo Audit
             (context as BiomasaEUPTContext).AuditoriaTablas.AddRange(audit.Entries);
        }


        // SaveChanges con auditoría
        public override int SaveChanges()
        {
            var auditoria = new Audit()
            {
                CreatedBy = Properties.Settings.Default.usuario
            };
            auditoria.PreSaveChanges(this);
            var filasAfectadas = base.SaveChanges();
            auditoria.PostSaveChanges();

            if (auditoria.Configuration.AutoSavePreAction != null)
            {
                auditoria.Configuration.AutoSavePreAction(this, auditoria);
                base.SaveChanges();
            }

            return filasAfectadas;
        }

        // SaveChangesAsync con auditoría
        public override Task<int> SaveChangesAsync()
        {
            return SaveChangesAsync(CancellationToken.None);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            var auditoria = new Audit()
            {
                CreatedBy = Properties.Settings.Default.usuario
            };
            auditoria.PreSaveChanges(this);
            var filasAfectadas = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            auditoria.PostSaveChanges();

            if (auditoria.Configuration.AutoSavePreAction != null)
            {
                auditoria.Configuration.AutoSavePreAction(this, auditoria);
                await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            return filasAfectadas;
        }


        public int GuardarCambios<TEntity>() where TEntity : class
        {
            // http://stackoverflow.com/questions/33403838/how-can-i-tell-entity-framework-to-save-changes-only-for-a-specific-dbset
            var original = ChangeTracker.Entries()
                        .Where(x => !typeof(TEntity).IsAssignableFrom(x.Entity.GetType()) && x.State != EntityState.Unchanged)
                        .GroupBy(x => x.State)
                        .ToList();

            foreach (var entry in ChangeTracker.Entries().Where(x => !typeof(TEntity).IsAssignableFrom(x.Entity.GetType())))
            {
                entry.State = EntityState.Unchanged;
            }

            var rows = base.SaveChanges();

            foreach (var state in original)
            {
                foreach (var entry in state)
                {
                    entry.State = state.Key;
                }
            }

            return rows;
        }

        public bool HayCambios<TEntity>(bool detectarEstadoInicial = true) where TEntity : class
        {
            return ChangeTracker.Entries().Where(
                x => typeof(TEntity).IsAssignableFrom(x.Entity.GetType()) &&
                x.State != EntityState.Unchanged &&
                (detectarEstadoInicial && x.State == EntityState.Modified ? (x.OriginalValues.PropertyNames.Where(y => x.OriginalValues[y] != null && x.CurrentValues[y] != null && x.OriginalValues[y].ToString() != x.CurrentValues[y].ToString()).ToList().Count > 0) : true)
                ).ToList().Count > 0;
        }

        private static string ConnectionString()
        {
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder
            {
                DataSource = @"(LocalDb)\MSSQLLocalDB",
                InitialCatalog = "BiomasaEUPT",
                //PersistSecurityInfo = true,
                IntegratedSecurity = true,
                MultipleActiveResultSets = true,
                ApplicationName = "EntityFramework",
                ConnectTimeout = 10
            };


            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder
            {
                ProviderConnectionString = sqlBuilder.ToString(),
                //Metadata = @"res://*/Modelo.csdl|res://*/Modelo.ssdl|res://*/Modelo.msl",
                Provider = "System.Data.SqlClient"
            };

            //Console.WriteLine(entityBuilder.ToString());
            return entityBuilder.ToString();
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            

            // Se evita que se cree la columna HistorialHuecosAlmacenajes_HistorialHuecoId en la tabla ProductoEnvasadoComposicion
            modelBuilder.Entity<ProductoEnvasadoComposicion>()
                        .HasRequired(pec => pec.HistorialHuecoAlmacenaje)
                        .WithMany()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<PedidoDetalle>()
                    .HasRequired(pd => pd.ProductoEnvasado)
                    .WithMany()
                    .WillCascadeOnDelete(false);

            // Cambio del nombre por defecto de las tablas de auditoría
            modelBuilder.Entity<AuditEntry>().ToTable("AuditoriaTablas");
            modelBuilder.Entity<AuditEntryProperty>().ToTable("AuditoriaDatosTablas");
        }

        // Tablas
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TipoUsuario> TiposUsuarios { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<TipoCliente> TiposClientes { get; set; }
        public DbSet<GrupoCliente> GruposClientes { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<TipoProveedor> TiposProveedores { get; set; }
        public DbSet<MateriaPrima> MateriasPrimas { get; set; }
        public DbSet<TipoMateriaPrima> TiposMateriasPrimas { get; set; }
        public DbSet<GrupoMateriaPrima> GruposMateriasPrimas { get; set; }
        public DbSet<Recepcion> Recepciones { get; set; }
        public DbSet<Procedencia> Procedencias { get; set; }
        public DbSet<EstadoRecepcion> EstadosRecepciones { get; set; }
        public DbSet<SitioRecepcion> SitiosRecepciones { get; set; }
        public DbSet<HuecoRecepcion> HuecosRecepciones { get; set; }
        public DbSet<HistorialHuecoRecepcion> HistorialHuecosRecepciones { get; set; }
        public DbSet<OrdenElaboracion> OrdenesElaboraciones { get; set; }
        public DbSet<EstadoElaboracion> EstadosElaboraciones { get; set; }
        public DbSet<ProductoTerminadoComposicion> ProductosTerminadosComposiciones { get; set; }
        public DbSet<ProductoTerminado> ProductosTerminados { get; set; }
        public DbSet<TipoProductoTerminado> TiposProductosTerminados { get; set; }
        public DbSet<GrupoProductoTerminado> GruposProductosTerminados { get; set; }
        public DbSet<SitioAlmacenaje> SitiosAlmacenajes { get; set; }
        public DbSet<HuecoAlmacenaje> HuecosAlmacenajes { get; set; }
        public DbSet<HistorialHuecoAlmacenaje> HistorialHuecosAlmacenajes { get; set; }
        public DbSet<EstadoEnvasado> EstadosEnvasados{ get; set; }
        public DbSet<OrdenEnvasado> OrdenesEnvasados { get; set; }
        public DbSet<ProductoEnvasado> ProductosEnvasados { get; set; }
        public DbSet<ProductoEnvasadoComposicion> ProductosEnvasadosComposiciones { get; set; }
        public DbSet<TipoProductoEnvasado> TiposProductosEnvasados { get; set; }
        public DbSet<GrupoProductoEnvasado> GruposProductosEnvasados { get; set; }
        public DbSet<PedidoCabecera> PedidosCabeceras { get; set; }
        public DbSet<EstadoPedido> EstadosPedidos { get; set; }
        public DbSet<PedidoDetalle> PedidosDetalles { get; set; }
        public DbSet<PedidoLinea> PedidosLineas { get; set; }
        public DbSet<Picking> Picking { get; set; }
        public DbSet<Pais> Paises { get; set; }
        public DbSet<Comunidad> Comunidades { get; set; }
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Municipio> Municipios { get; set; }


        // Auditoría
        public DbSet<AuditEntry> AuditoriaTablas { get; set; }
        public DbSet<AuditEntryProperty> AuditoriaDatosTablas { get; set; }
    }

}