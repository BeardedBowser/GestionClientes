using GestionCliente.Context;
using GestionCliente.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace GestionCliente.Services
{
    public class ClientService : IClientService
    {
        private readonly GestionClienteDbContext _dbContext;

        public ClientService(GestionClienteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Client>> GetClient() 
        {
            var client = await _dbContext.Clients.ToListAsync();

            return client;
        }

        public async Task UpdateClient(int idClient, Client updatedClient) 
        {
            string method = "PUT";

            var client = await _dbContext.Clients.FindAsync(idClient);
            if (client == null)
            {
                string error = "Cliente con ID {idClient} no fue encontrado.";

                await GenerateLogError(method, error, idClient.ToString());
                throw new ArgumentException(error);
            }

            await ValidateClientParameters(updatedClient, method);
            await ValidateExistingClient(updatedClient, method);

            client.Rut = updatedClient.Rut;
            client.Nombre = updatedClient.Nombre;
            client.SegundoNombre = updatedClient.SegundoNombre;
            client.ApellidoPaterno = updatedClient.ApellidoPaterno;
            client.ApellidoMaterno = updatedClient.ApellidoMaterno;
            client.Direccion = updatedClient.Direccion;
            client.Telefono = updatedClient.Telefono;
            client.Email = updatedClient.Email;

            await GenerateLogSuccess(method, $"Cliente con RUT {updatedClient.Rut} actualizado exitosamente");

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Client> CreateClient(Client newClient) 
        {
            string method = "POST";

            await ValidateClientParameters(newClient, method);
            await ValidateExistingClient(newClient, method);

            await _dbContext.Clients.AddAsync(newClient);
            await GenerateLogSuccess(method, $"Cliente con RUT {newClient.Rut} creado exitosamente");

            await _dbContext.SaveChangesAsync();

            return newClient;
        }

        private async Task ValidateExistingClient(Client client, string method) 
        {
            string error;
            var clients = await _dbContext.Clients.ToListAsync();

            if (clients.Any(e => e.Rut == client.Rut))
            {
                error = $"El RUT '{client.Rut}' ya existe en el sistema.";

                await GenerateLogError(method, error, client.Rut);
                throw new Exception($"El RUT '{client.Rut}' ya existe en el sistema.");
            }

            if (!string.IsNullOrEmpty(client.Telefono) && clients.Any(e => e.Telefono == client.Telefono)) 
            {
                error = $"El teléfono '{client.Telefono}' ya existe en el sistema.";

                await GenerateLogError(method, error, client.Telefono);
                throw new Exception(error);
            }



            if (!string.IsNullOrEmpty(client.Email) && clients.Any(e => e.Email == client.Email)) 
            {
                error = $"El email '{client.Email}' ya existe en el sistema.";

                await GenerateLogError(method, error, client.Email);
                throw new Exception(error);
            }
                
        }

        private async Task ValidateClientParameters(Client client, string method)
        {
            string error;
            // Validar RUT
            if (client.Rut.Length < 11 || !Regex.IsMatch(client.Rut, @"^\d{1,2}\.\d{3}\.\d{3}-[\d|kK]{1}$") || client.Rut.Length > 12)
            {
                error = "El RUT proporcionado no es válido. Debe tener el formato '11.111.111-1'";

                await GenerateLogError(method, error, client.Rut);
                throw new ArgumentException("El RUT proporcionado no es válido. Debe tener el formato '11.111.111-1'");
            }

            // Validar nombre, segundoNombre, apellidoPaterno, apellidoMaterno
            if (client.Nombre.Length > 15 ||
                client.SegundoNombre.Length > 15 ||
                client.ApellidoPaterno.Length > 15 ||
                client.ApellidoMaterno.Length > 15)
            {
                error = "La longitud de uno de los campos de nombre excede el límite permitido.";
                string parametro = client.Nombre + ' ' + client.SegundoNombre + ' ' + client.ApellidoPaterno + ' ' + client.ApellidoMaterno;

                await GenerateLogError(method, error, parametro);
                throw new ArgumentException(error);
            }

            // Validar dirección
            if (!string.IsNullOrEmpty(client.Direccion) && client.Direccion.Length > 70)
            {
                error = "La longitud de la dirección excede el límite permitido.";

                await GenerateLogError(method, error, client.Direccion);
                throw new ArgumentException(error);
            }

            // Validar teléfono
            if (!string.IsNullOrEmpty(client.Telefono) && (client.Telefono.Length < 11 || !Regex.IsMatch(client.Telefono, @"^\+569\d{8}$|^[9]\d{8}$") || client.Telefono.Length > 12))
            {
                error = "El formato del número de teléfono no es válido. Debe seguir el formato '+56912345678'.";
                await GenerateLogError(method, error, client.Telefono);
                throw new ArgumentException(error);
            }

            // Validar email
            if (!string.IsNullOrEmpty(client.Email) && !Regex.IsMatch(client.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                error = "El formato del correo electrónico no es válido.";

                await GenerateLogError(method, error, client.Email);
                throw new ArgumentException(error);
            }
        }

        private async Task GenerateLogError(string method, string error, string parameter) 
        {
            var log = new Log()
            {
                Metodo = method,
                Error = error,
                Resultado = "Error",
                ParametroErroneo = parameter
            };

            await _dbContext.Log.AddAsync(log);
            await _dbContext.SaveChangesAsync();
        }

        private async Task GenerateLogSuccess(string method, string result) 
        {
            var log = new Log()
            {
                Metodo = method,
                Resultado = result
            };

            await _dbContext.Log.AddAsync(log);
        }
    }
}
