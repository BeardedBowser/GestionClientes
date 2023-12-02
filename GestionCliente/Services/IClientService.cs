using GestionCliente.Models;

namespace GestionCliente.Services
{
    public interface IClientService
    {
        Task<List<Client>> GetClient();
        Task UpdateClient(int idClient, Client updatedClient);
        Task<Client> CreateClient(Client newClient);
    }
}