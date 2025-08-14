using Entidades;
using Microsoft.AspNetCore.Mvc;
using TrabajoAnalisis;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Unidad1Controller : ControllerBase
    {

        private Unidad1 llamar { get; set; }
        public Unidad1Controller()
        {
            llamar = new Unidad1();
        }

       

        // POST api/<ValuesController>
        [HttpPost("biseccion")]
        public IActionResult PostBiseccion([FromBody] CerradosParam param)
        {
            var resultado = llamar.Biseccion(param);
            return Ok(resultado);
        }

        [HttpPost("reglafalsa")]
        public IActionResult PostReglaFalsa([FromBody] CerradosParam param)
        {
            var resultado = llamar.ReglaFalsa(param);
            return Ok(resultado);
        }


    }
}
