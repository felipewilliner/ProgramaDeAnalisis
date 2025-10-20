using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Unidad3Param
    {
        public double Tolerancia { get; set; }
        public int Grado { get; set; }
        public List<xy> Puntos { get; set; }

        public Unidad3Param()
        {
            Puntos = new List<xy>();
        }
    }
}
