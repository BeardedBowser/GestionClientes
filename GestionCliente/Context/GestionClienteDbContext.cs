using GestionCliente.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GestionCliente.Context
{
    public class GestionClienteDbContext : DbContext
    {
        static readonly string db = "dbSqlite.db";
        public DbSet<Client> Clients { get; set; }
        public DbSet<Log> Log { get; set; }

        public GestionClienteDbContext(DbContextOptions<GestionClienteDbContext> options) : base(options)
        {
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connectionString: "Filename=" + db,
                sqliteOptionsAction: op => 
                {
                    op.MigrationsAssembly(
                            Assembly.GetExecutingAssembly().FullName
                        );   
                });

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(
                entity => 
                {
                    entity.HasKey(e => e.Id);

                    // Se le da un entity para al momento de ingresar no tener que preocuparse del Id
                    entity.Property(e => e.Id).ValueGeneratedOnAdd();

                    entity.Property(e => e.Rut)
                        .HasColumnType("VARCHAR(12)")
                        .HasMaxLength(12)
                        .IsRequired();

                    entity.Property(e => e.Nombre)
                        .HasColumnType("VARCHAR(15)")
                        .HasMaxLength(15)
                        .IsRequired();

                    entity.Property(e => e.SegundoNombre)
                        .HasColumnType("VARCHAR(15)")
                        .HasMaxLength(15)
                        .IsRequired();

                    entity.Property(e => e.ApellidoPaterno)
                        .HasColumnType("VARCHAR(15)")
                        .HasMaxLength(15)
                        .IsRequired();

                    entity.Property(e => e.ApellidoMaterno)
                        .HasColumnType("VARCHAR(15)")
                        .HasMaxLength(15)
                        .IsRequired();

                    // Se decide dejar los datos demográficos no requeridos para no forzar al cliente a colocar
                    // datos sensibles en caso de ser necesario.
                    entity.Property(e => e.Direccion)
                        .HasColumnType("VARCHAR(100)")
                        .HasMaxLength(100);

                    entity.Property(e => e.Telefono)
                        .HasColumnType("VARCHAR(12)")
                        .HasMaxLength(12);

                    entity.Property(e => e.Email)
                        .HasColumnType("VARCHAR(70)")
                        .HasMaxLength(70);

                    // Se crean dos clientes de prueba:
                    // 1. Con datos demográficos.
                    // 2. Sin datos demográficos.

                    entity.HasData(
                        new Client() 
                        { 
                            Id = 1,
                            Rut = "10.471.820-5",
                            Nombre = "Juan",
                            SegundoNombre = "Gonzalo",
                            ApellidoPaterno = "Perez",
                            ApellidoMaterno = "Muñoz",
                            Direccion = "Álvarez 2560, Viña del Mar, Valparaíso",
                            Telefono = "912345678",
                            Email = "correo@correo.com"
                        },
                        new Client()
                        {
                            Id = 2,
                            Rut = "5.981.081-2",
                            Nombre = "Camila",
                            SegundoNombre = "María",
                            ApellidoPaterno = "Montero",
                            ApellidoMaterno = "Espinoza"
                        }
                    );
                }
            );

            modelBuilder.Entity<Log>(
                entity => 
                {
                    entity.HasKey(e => e.Id);

                    entity.Property(e => e.Id).ValueGeneratedOnAdd();

                    entity.Property(e => e.Metodo)
                        .HasColumnType("VARCHAR(50)")
                        .HasMaxLength(50).IsRequired();

                    entity.Property(e => e.Error)
                        .HasColumnType("VARCHAR(200)")
                        .HasMaxLength(200);

                    entity.Property(e => e.ParametroErroneo)
                        .HasColumnType("VARCHAR(100)")
                        .HasMaxLength(100);

                    entity.Property(e => e.Resultado)
                        .HasColumnType("VARCHAR(200)")
                        .HasMaxLength(100)
                        .IsRequired();
                }    
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
