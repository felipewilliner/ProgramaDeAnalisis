using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class ResultadoEcuaciones
    {
        public bool Success { get; set; }
        public string Mensaje { get; set; }
        public double[] Resultados { get; set; }

        public ResultadoEcuaciones(int dimension)
        {
            Resultados = new double[dimension];
        }
    }
}
