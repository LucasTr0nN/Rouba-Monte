using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rouba_Monte
{
    public class Pilha
    {
        private Celula topo;

        public Pilha()
        {
            topo = null;
        }

        public void Empilhar(Carta x)
        {
            Celula tmp = new Celula(x);
            tmp.Prox = topo;
            topo = tmp;
        }

        public Carta Desempilhar()
        {
            if (topo == null)
            {
                throw new Exception("Pilha vazia");
            }
            Carta elemento = topo.Elemento;
            topo = topo.Prox;
            return elemento;
        }

        public int Tamanho()
        {
            int tamanho = 0;
            Celula atual = topo;
            while (atual != null)
            {
                tamanho++;
                atual = atual.Prox;
            }
            return tamanho;
        }

        public bool Vazia()
        {
            return topo == null;
        }

        public Carta Peek()
        {
            if (topo == null)
            {
                return null;
            }
            return topo.Elemento;
        }
    }
}
