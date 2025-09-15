using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class GaussSeidelParam
    {
        public int Dimension { get; set; }
        public double[][] Matriz { get; set; }
        public double Tolerancia { get; set; } = 0.0001;
        public int MaxIteraciones { get; set; } = 100;
    }
}
