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
        public string Nome { get; set; }
        public int Posição { get; set; }
        public Pilha Monte { get; set; }
        public Queue<int> Ranking { get; set; }

        public Jogador(string nome)
        {
            Nome = nome;
            Posição = 0;
            Monte = new Pilha();
            Ranking = new Queue<int>(5);
        }

        public void AdicionarRanking(int posicao)
        {
            if (Ranking.Count() == 5)
            {
                Ranking.Dequeue();
            }
            Ranking.Enqueue(posicao);
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
            int i = 1;
            foreach (int posicao in Ranking)
            {
                logWriter.WriteLine($"Partida: {i} Posição: {posicao}");
                Console.WriteLine($"Partida: {i} Posição: {posicao}");
                i++;
            }
        }
    }
}
