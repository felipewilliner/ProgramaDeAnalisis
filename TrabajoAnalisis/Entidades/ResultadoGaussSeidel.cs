using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class ResultadoGaussSeidel
    {
        public bool Success { get; set; }
        public string Mensaje { get; set; }
        public double[] Resultados { get; set; }
        public int Iteraciones { get; set; }
        public double ErrorFinal { get; set; }

        public ResultadoGaussSeidel(int dimension)
        {
            Resultados = new double[dimension];
            Mensaje = string.Empty;
        }
    }
}
