using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Unidad3ModificadaParam
    {
        public string FuncionModificada { get; set; }
        public List<xy> Puntos { get; set; }
        public double Tolerancia { get; set; }

        public Unidad3ModificadaParam()
        {
            FuncionModificada = string.Empty;
            Puntos = new List<xy>();
        }
    }
}
