using Entidades;
using Microsoft.AspNetCore.Mvc;
using TrabajoAnalisis;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Unidad4Controller : ControllerBase
    {
        private Unidad4 llamar { get; set; }

        public Unidad4Controller()
        {
            llamar = new Unidad4();
        }

        // POST api/Unidad4/calcularintegral
        [HttpPost("calcularintegral")]
        public IActionResult PostCalcularIntegral([FromBody] Unidad4Param param)
        {
            var resultado = llamar.CalcularIntegral(param);
            return Ok(resultado);
        }
    }
}