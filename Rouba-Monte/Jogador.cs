using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rouba_Monte
{
    public class Jogador
    {
        private string nome;
        private int posicao;
        private Pilha monte;
        private Fila Ranking;

        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        public int Posição
        {
            get { return posicao; }
            set { posicao = value; }
        }

        public Pilha Monte
        {
            get { return monte; }
            set { monte = value; }
        }
        public Jogador(string nome)
        {
            Nome = nome;
            Posição = 0;
            Monte = new Pilha();
            Ranking = new Fila(5);
        }

        public void AdicionarRanking(int posicao)
        {
            if (Ranking.Quantidade() == 5)
            {
                Ranking.Remover();
            }
            Ranking.Inserir(posicao);
        }

        public void LimparMonte()
        {
            while (!Monte.Vazia())
            {
                Monte.Desempilhar();
            }
        }
        public void MostrarRanking(StreamWriter logWriter)
        {
            for (int i = 0; i < Ranking.Quantidade(); i++)
            {
                int posicao = Ranking.Remover();
                logWriter.WriteLine($"Partida: {i + 1} Posição: {posicao}");
                Console.WriteLine($"Partida: {i + 1} Posição: {posicao}");
                Ranking.Inserir(posicao);
            }
        }
    }
}
