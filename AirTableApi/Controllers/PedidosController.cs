using AirTableApi.Dtos;
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
            try
            {
                var records = await _airtableService.GetAllRecordsAsync();

                var pedidos = records
                    .Select(r => DtoPedido.MapAirtable(r.Fields))
                    .ToList();

                return Ok(pedidos);
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

    }
}
