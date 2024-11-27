using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rouba_Monte
{
    public class Ordenar
    {
        public static void Selecao(List<Jogador> jogadores)
        {
            int n = jogadores.Count;

            for (int i = 0; i < n - 1; i++)
            {
                int maior = i;

                for (int j = i + 1; j < n; j++)
                {
                    if (jogadores[j].Monte.Tamanho() > jogadores[maior].Monte.Tamanho())
                    {
                        maior = j;
                    }
                }
                if (maior != i)
                {
                    Jogador temp = jogadores[maior];
                    jogadores[maior] = jogadores[i];
                    jogadores[i] = temp;
                }
            }
        }
    }
}
