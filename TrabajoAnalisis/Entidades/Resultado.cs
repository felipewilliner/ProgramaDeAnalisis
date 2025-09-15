using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabajoAnalisis
{
    public class Resultado
    {
        private double _raiz;
        private double _error;
        public string Mensaje { get; set; }
        public bool Success { get; set; }
        public double Raiz { get => Math.Round(_raiz, 3); set => _raiz = value; }
        public int Iteraciones { get; set; }
        public double Error { get => Math.Round(_error, 8); set => _error = value; }


    }
}
