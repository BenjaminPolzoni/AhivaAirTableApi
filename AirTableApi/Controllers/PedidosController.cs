using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirTableApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {

        // Inyeccion de dependencias
        private readonly AirtableService _airtableService;

        public PedidosController(AirtableService airtable)
        {
            _airtableService = airtable;
        }

        [HttpGet]
        public async Task<IActionResult> GetPedidos()
        {
            var pedidos = await _airtableService.GetPedidosAsync();
            return Ok(pedidos);
        }
    }
}
