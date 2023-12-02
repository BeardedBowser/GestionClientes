using GestionCliente.Context;
using GestionCliente.Models;
using GestionCliente.Services;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace GestionCliente
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(
                endpoints => {
                    endpoints.MapControllers();
                }
            );

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<GestionClienteDbContext>();

                // Inicializar la base de datos (crear si no existe)
                dbContext.Database.EnsureCreated();

                // Aplicar migraciones pendientes
                dbContext.Database.Migrate();
            }
        }

        public void ConfigureServices(IServiceCollection services) 
        {
            services.AddControllers().AddOData(opt => 
                opt.AddRouteComponents("odata", GetEdmModel()).Select().Filter().SetMaxTop(100).OrderBy().Expand().Count()
            );

            AddServices(services);

            services.AddDbContext<GestionClienteDbContext>(
                options =>
                options.UseSqlite("dbSqlite.db")
            );

        }

        private static void AddServices(IServiceCollection services) 
        {
            services.AddScoped<IClientService, ClientService>();
        }
        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Client>("Clients");
            return builder.GetEdmModel();
        }



    }
}
