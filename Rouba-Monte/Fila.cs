using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rouba_Monte
{
    public class Fila
    {
        private Carta[] fila;
        private int capacidadeMaxima;
        private int Primeiro;
        private int Ultimo;
        private int tamanho;

        public Fila(int capacidadeMaxima)
        {
            this.capacidadeMaxima = capacidadeMaxima;
            fila = new Carta[capacidadeMaxima];
            Primeiro = 0;
            Ultimo = 0;
            tamanho = 0;
        }

        public void Inserir(Carta carta)
        {
            if (tamanho == capacidadeMaxima)
            {
                throw new InvalidOperationException("Fila cheia.");
            }

            fila[Ultimo] = carta;
            Ultimo = (Ultimo + 1) % capacidadeMaxima;
            tamanho++;
        }

        public Carta Remover()
        {
            if (tamanho == 0)
            {
                throw new InvalidOperationException("Fila vazia.");
            }

            Carta carta = fila[Primeiro];
            Primeiro = (Primeiro + 1) % capacidadeMaxima;
            tamanho--;
            return carta;
        }

        public int Quantidade()
        {
            return tamanho;
        }

        public bool EstaVazia()
        {
            return tamanho == 0;
        }
    }
}