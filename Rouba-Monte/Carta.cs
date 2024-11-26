using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rouba_Monte
{
    public class Carta
    {
        public int Valor { get; set; }
        public string Naipe { get; set; }

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
