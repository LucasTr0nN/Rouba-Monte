using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rouba_Monte
{
    class Celula
    {
        private Carta elemento;
        private Celula prox;

        public Carta Elemento
        {
            get { return elemento; }
            set { elemento = value; }
        }

        public Celula Prox
        {
            get { return prox; }
            set { prox = value; }
        }

        public Celula(Carta elemento)
        {
            Elemento = elemento;
            Prox = null;
        }
    }
}
