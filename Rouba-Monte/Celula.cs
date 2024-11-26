using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rouba_Monte
{
    class Celula
    {
        public Carta Elemento { get; set; }
        public Celula Prox { get; set; }

        public Celula(Carta elemento)
        {
            Elemento = elemento;
            Prox = null;
        }
    }
}
