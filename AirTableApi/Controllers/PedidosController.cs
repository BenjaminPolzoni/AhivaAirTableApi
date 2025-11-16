using AirTableApi.Dtos.AirTable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

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

        [HttpGet]
        [Route("GetByEstadoEntregar")]
        public async Task<IActionResult> GetPedidosEntregar()
        {
            try
            {
                var records = await _airtableService.GetAllRecordsAsync();

                var pedidos = records
                    .Select(r => DtoPedido.MapAirtable(r.Fields))
                    .Where(p => p.Estado?.ToLower() == "entregar")
                    .OrderBy(p => p.Pago == "Pago" ? 1 : 2)
                    .ThenBy(p => p.UltimoCambioEstado)
                    .ToList();

                return Ok(pedidos);
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }


        [HttpGet]
        [Route("GetByEstado{estado}")]
        public async Task<IActionResult> GetPedidosEntregar([FromRoute] string estado)
        {
            try
            {
                var records = await _airtableService.GetAllRecordsAsync();

                var pedidos = records
                    .Select(r => DtoPedido.MapAirtable(r.Fields))
                    .Where(p => p.Estado?.ToLower() == estado.ToLower())
                    .OrderBy(p => p.Pago == "Pago" ? 1 : 2)
                    .ThenBy(p => p.UltimoCambioEstado)
                    .ToList();

                return Ok(pedidos);
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }


        [HttpPost("actualizar-rastreo")]
        public async Task<IActionResult> ActualizarRastreo(string telefono, string fecha, string estado, string tracking)
        {
            try
            {
                // Permitir varios formatos de entrada
                var formatos = new[] { "dd/MM/yyyy", "dd-MM-yyyy" };

                if (!DateTime.TryParseExact(fecha, formatos,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaParsed))
                {
                    return BadRequest("Formato de fecha inválido. Usa dd/MM/yyyy.");
                }

                // Convertimos al formato EXACTO del Airtable
                var fechaFormateada = fechaParsed.ToString("DD/MM/yyyy")
                                                  .Replace("DD", "dd")   // seguridad extra
                                                  .Replace("YYYY", "yyyy");

                var records = await _airtableService.GetAllRecordsAsync();

                // Buscar el pedido exacto
                var record = records.FirstOrDefault(r =>
                    r.Fields.ContainsKey("Telefono") &&
                    r.Fields["Telefono"]?.ToString() == telefono &&

                    r.Fields.ContainsKey("Estado") &&
                    r.Fields["Estado"]?.ToString()?.ToLower() == estado.ToLower() 
                );

                if (record == null)
                    return NotFound("No se encontró un pedido con esos datos.");

                var ok = await _airtableService.UpdateTrackingNumber(record.Id, tracking);

                if (!ok)
                    return BadRequest("No se pudo actualizar el número de rastreo.");

                return Ok("Número de rastreo actualizado correctamente.");
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

    }
}
