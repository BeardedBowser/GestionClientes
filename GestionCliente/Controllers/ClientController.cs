using GestionCliente.Models;
using GestionCliente.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace GestionCliente.Controllers
{
    [Route("Clients")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>
        /// [ODATA] Retorna los clientes. Tiene OData habilitado para obtener 1 o más clientes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> GetClients()
        {
            try
            {
                var res = await _clientService.GetClient();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(409, new { mensajeError = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un cliente. Valida que todos los campos tengan un formato correcto y que el cliente con ciertos datos no exista previamente.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, Client updatedClient) 
        {
            try
            {
                await _clientService.UpdateClient(id, updatedClient);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(409, new { mensajeError = ex.Message });
            }
        }

        /// <summary>
        /// Crea un cliente. Valida que todos los campos tengan un formato correcto y que el cliente con ciertos datos no exista previamente.
        /// </summary>
        /// <param name="newClient"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateClient(Client newClient) 
        {
            try
            {
                var res = await _clientService.CreateClient(newClient);

                return Created("", res);
            }
            catch (ArgumentException ex) 
            { 
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(409, new { mensajeError = ex.Message });
            }
        }
    }
}
