using Entidades;
using Microsoft.AspNetCore.Mvc;
using TrabajoAnalisis;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Unidad2Controller : ControllerBase
    {

        private Unidad2 llamar { get; set; }
        public Unidad2Controller()
        {
            llamar = new Unidad2();
        }



        // POST api/<ValuesController>
        [HttpPost("gaussjordan")]
        public IActionResult PostGaussJordan([FromBody] EcuacionesParam param)
        {
            var resultado = llamar.ResolverGaussJordan(param);
            return Ok(resultado);
        }

        [HttpPost("gaussseidel")]
        public IActionResult PostGaussSeidel([FromBody] GaussSeidelParam param)
        {
            var resultado = llamar.ResolverGaussSeidel(param);
            return Ok(resultado);
        }



    }
}
