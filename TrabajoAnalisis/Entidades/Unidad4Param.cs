using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Unidad4Param
    {
        public string Funcion { get; set; }
        public double Xi { get; set; }
        public double Xd { get; set; }
        public int SubintervalosN { get; set; } 
        public string Metodo { get; set; }   
        
        public Unidad4Param()
        {
            Funcion = string.Empty;
           Metodo = string.Empty;
        }
    }
}

