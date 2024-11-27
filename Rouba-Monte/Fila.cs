using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rouba_Monte
{
    public class Fila
    {
        private int[] fila;
        private int capacidadeMaxima;
        private int Primeiro;
        private int Ultimo;
        private int tamanho;

        public Fila(int capacidadeMaxima)
        {
            this.capacidadeMaxima = capacidadeMaxima;
            fila = new int[capacidadeMaxima];
            Primeiro = 0;
            Ultimo = 0;
            tamanho = 0;
        }

        public void Inserir(int valor)
        {
            if (tamanho == capacidadeMaxima)
            {
                throw new Exception("Fila cheia.");
            }

            fila[Ultimo] = valor;
            Ultimo = (Ultimo + 1) % capacidadeMaxima;
            tamanho++;
        }

        public int Remover()
        {
            if (tamanho == 0)
            {
                throw new Exception("Fila vazia.");
            }

            int valor = fila[Primeiro];
            Primeiro = (Primeiro + 1) % capacidadeMaxima;
            tamanho--;
            return valor;
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