using Entidades;
using Microsoft.AspNetCore.Mvc;
using TrabajoAnalisis;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Unidad3Controller : ControllerBase
    {
        private Unidad3 llamar { get; set; }

        public Unidad3Controller()
        {
            llamar = new Unidad3();
        }


        // POST api/<Unidad3Controller>
        [HttpPost("regresionlineal")]
        public IActionResult PostRegresionLineal([FromBody] Unidad3Param param)
        {
            var resultado = llamar.CalcularRegresionLineal(param);

            // Retorna 200 OK con el objeto Unidad3Resultado
            return Ok(resultado);
        }

        [HttpPost("regresionpolinomial")]
        public IActionResult PostRegresionPolinomial([FromBody] Unidad3Param param)
        {
            var resultado = llamar.CalcularRegresionPolinomial(param);

            // Retorna 200 OK con el objeto Unidad3Resultado
            return Ok(resultado);
        }


    }
}
