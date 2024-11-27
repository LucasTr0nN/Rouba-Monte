using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rouba_Monte
{
    public class Carta
    {
        private int valor;
        private string naipe;

        public int Valor
        {
            get { return valor; }
            set { valor = value; }
        }

        public string Naipe
        {
            get { return naipe; }
            set { naipe = value; }
        }

        public Carta(int valor, string naipe)
        {
            Valor = valor;
            Naipe = naipe;
        }

        public override string ToString()
        {
            return $"{Valor} de {Naipe}";
        }
    }
}
