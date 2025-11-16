using AirTableApi.Dtos.Ahiva;
using AirTableApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AirTableApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AhivaController : ControllerBase
    {
        private readonly AhivaService _ahivaService;

        public AhivaController(AhivaService ahivaService)
        {
            _ahivaService = ahivaService;
        }

        // =======================================================
        //  1) ENVIAR CARGA MASIVA
        // =======================================================
        [HttpPost("carga-masiva")]
        public async Task<IActionResult> EnviarCargaMasiva([FromBody] DtoCargaMasivaRequest request)
        {
            var result = await _ahivaService.EnviarCargaMasiva(request);
            return Ok(result);
        }

        // =======================================================
        //  2) CONSULTAR ESTADO
        // =======================================================
        [HttpGet("estado/{tracking}")]
        public async Task<IActionResult> GetEstado(string tracking)
        {
            var response = await _ahivaService.ConsultarEstado(tracking);
            return Ok(response);
        }
    }
}
