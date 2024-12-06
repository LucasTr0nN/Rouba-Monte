using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rouba_Monte
{
    public class RoubaMonte
    {
        private Dictionary<string, Jogador> jogadoresAtuais;
        private Dictionary<string, Jogador> jogadoresJogados;
        private Stack<Carta> monteCompra;
        private List<Carta> areaDescarte;
        private StreamWriter logWriter;
        private int numBaralhos;

        public RoubaMonte()
        {
            jogadoresAtuais = new Dictionary<string, Jogador>();
            jogadoresJogados = new Dictionary<string, Jogador>();
            monteCompra = new Stack<Carta>();
            areaDescarte = new List<Carta>();

            logWriter = new StreamWriter("E:\\ATP's\\Rouba-Monte\\Rouba-Monte\\log.txt", append: true);
        }
        public void IniciarJogo(bool novoJogo)
        {
            if (novoJogo)
            {
                Console.WriteLine("Digite o número de baralhos (1 baralho = 52 cartas):");
                numBaralhos = int.Parse(Console.ReadLine());
                int totalCartas = numBaralhos * 52;
                Console.WriteLine($"Número total de cartas: {totalCartas}");
                logWriter.WriteLine($"O número de baralhos da partida é de: {numBaralhos}, totalizando {totalCartas} Cartas.");
                Console.WriteLine("Digite o número de jogadores:");
                int numeroDeJogadores = int.Parse(Console.ReadLine());
                logWriter.WriteLine($"A quantidade de jogares dessa partida é de: {numeroDeJogadores} jogadores.");

                foreach (var jogador in jogadoresAtuais.Values)
                {
                    jogadoresJogados[jogador.Nome] = jogador;
                }
                jogadoresAtuais.Clear();

                for (int i = 0; i < numeroDeJogadores; i++)
                {
                    Console.WriteLine($"Digite o nome do jogador {i + 1}:");
                    string nomeJogador = Console.ReadLine();
                    logWriter.WriteLine($"Jogador {nomeJogador} foi adicionado na partida.");

                    Jogador jogador;

                    if (jogadoresAtuais.ContainsKey(nomeJogador))
                    {
                        jogador = jogadoresAtuais[nomeJogador];
                        logWriter.WriteLine($"Usando o Jogador {nomeJogador} (jogador atual).");
                        Console.WriteLine($"Jogador {nomeJogador} já existe, usando jogador existente.");
                        jogador.LimparMonte();
                    }
                    else if (jogadoresJogados.ContainsKey(nomeJogador))
                    {
                        jogador = jogadoresJogados[nomeJogador];
                        jogadoresAtuais.Add(nomeJogador, jogador);
                        logWriter.WriteLine($"Usando o Jogador {nomeJogador} (jogador anterior).");
                        Console.WriteLine($"Jogador {nomeJogador} já jogou anteriormente, usando jogador existente.");
                        jogador.LimparMonte();
                    }
                    else
                    {
                        jogador = new Jogador(nomeJogador);
                        jogadoresAtuais.Add(nomeJogador, jogador);
                        logWriter.WriteLine($"Jogador {nomeJogador} foi adicionado como novo.");
                    }
                }

                monteCompra = new Stack<Carta>();
                areaDescarte = new List<Carta>();
                CriarMonteDeCompra(numBaralhos);
                logWriter.WriteLine("Monte de compras criado...");
                EmbaralharMonte();
                logWriter.WriteLine("Monte embaralhado...");
            }
            else
            {
                int totalCartas = numBaralhos * 52;
                monteCompra = new Stack<Carta>();
                areaDescarte = new List<Carta>();
                CriarMonteDeCompra(numBaralhos);
                logWriter.WriteLine("Monte de compras criado...");
                EmbaralharMonte();
                logWriter.WriteLine("Monte embaralhado...");

                foreach (var jogador in jogadoresAtuais.Values)
                {
                    jogador.LimparMonte();
                    logWriter.WriteLine($"Monte do jogador {jogador.Nome} limpo.");
                }
            }
            Console.WriteLine("Escolha o jogador que iniciará: (Escolha por número)");

            int jogadorInicio = int.Parse(Console.ReadLine()) - 1;
            logWriter.WriteLine($"Jogador {jogadorInicio} iniciará a partida.");

            while (monteCompra.Count() > 0)
            {
                foreach (var jogador in jogadoresAtuais.Values)
                {
                    if (monteCompra.Count() == 0)
                    {
                        break;
                    }
                    RealizarJogada(jogador);
                }
            }
            FinalizarJogo();
        }
        private void CriarMonteDeCompra(int numBaralhos)
        {
            for (int b = 0; b < numBaralhos; b++)
            {
                for (int valor = 1; valor <= 13; valor++)
                {
                    foreach (string naipe in new[] { "Paus", "Copas", "Espadas", "Ouros" })
                    {
                        monteCompra.Push(new Carta(valor, naipe));
                    }
                }
            }
        }

        private void EmbaralharMonte()
        {
            var cartas = new List<Carta>();
            while (monteCompra.Count() > 0)
            {
                cartas.Add(monteCompra.Pop());
            }

            Random rand = new Random();
            while (cartas.Count > 0)
            {
                int index = rand.Next(cartas.Count);
                monteCompra.Push(cartas[index]);
                cartas.RemoveAt(index);
            }
            logWriter.WriteLine("Monte de compra embaralhado.");
            Console.WriteLine("Monte de compra embaralhado.");
        }
        private void RealizarJogada(Jogador jogador)
        {
            Console.WriteLine($"\nVez do jogador: {jogador.Nome}\n");

            while (true)
            {
                if (monteCompra.Count() == 0)
                {
                    Console.WriteLine("O monte de compra está vazio. Fim do jogo.\n");
                    break;
                }
                Carta cartaComprada = monteCompra.Pop();
                Console.WriteLine($"{jogador.Nome} comprou a carta: {cartaComprada}");

                bool jogadaEfetuada = false;

                if (VerificarRoubo(jogador, cartaComprada))
                {
                    Console.WriteLine($"{jogador.Nome} roubou o monte de outro jogador! Jogue novamente.\n");
                    continue;
                }
                else if (VerificarProprioMonte(jogador, cartaComprada))
                {
                    Console.WriteLine($"{jogador.Nome} colocou a carta em seu próprio monte! Jogue novamente.\n");
                    continue;
                }
                else if (VerificarDescarte(jogador, cartaComprada))
                {
                    Console.WriteLine($"{jogador.Nome} pegou cartas da área de descarte! Jogue novamente.\n");
                    continue;
                }
                else
                {
                    areaDescarte.Add(cartaComprada);
                    Console.WriteLine($"{jogador.Nome} descartou a carta: {cartaComprada}");
                    LogJogada(jogador, "Descartou", cartaComprada);
                    jogadaEfetuada = true;
                }
                if (jogadaEfetuada)
                {
                    break;
                }
            }
        }

        private bool VerificarRoubo(Jogador jogador, Carta cartaComprada)
        {
            foreach (var outroJogador in jogadoresAtuais)
            {
                if (outroJogador.Value != jogador && !outroJogador.Value.Monte.Vazia())
                {
                    Carta cartaTopoOutroMonte = outroJogador.Value.Monte.Peek();
                    if (cartaTopoOutroMonte.Valor == cartaComprada.Valor)
                    {
                        while (!outroJogador.Value.Monte.Vazia())
                        {
                            jogador.Monte.Empilhar(outroJogador.Value.Monte.Desempilhar());
                        }

                        jogador.Monte.Empilhar(cartaComprada);
                        logWriter.WriteLine($"{DateTime.Now}: {jogador.Nome} Roubou o monte inteiro do {outroJogador.Key} {cartaComprada.ToString()}");
                        LogJogada(jogador, "Roubou o monte inteiro com", cartaComprada);
                        return true;
                    }
                }
            }
            return false;
        }
        private bool VerificarDescarte(Jogador jogador, Carta cartaComprada)
        {

            for (int i = 0; i < areaDescarte.Count; i++)
            {
                if (areaDescarte[i].Valor == cartaComprada.Valor)
                {

                    Carta cartaCorrespondente = areaDescarte[i];
                    areaDescarte.RemoveAt(i);

                    jogador.Monte.Empilhar(cartaComprada);
                    jogador.Monte.Empilhar(cartaCorrespondente);

                    LogJogada(jogador, "Pegou da área de descarte", cartaComprada);
                    return true;
                }
            }
            return false;
        }

        private bool VerificarProprioMonte(Jogador jogador, Carta cartaComprada)
        {
            if (!jogador.Monte.Vazia() && jogador.Monte.Peek().Valor == cartaComprada.Valor)
            {
                jogador.Monte.Empilhar(cartaComprada);
                LogJogada(jogador, "Colocou em seu próprio monte", cartaComprada);
                return true;
            }
            return false;
        }

        private void LogJogada(Jogador jogador, string acao, Carta carta)
        {
            string log = $"{DateTime.Now}: {jogador.Nome} {acao} a carta {carta.ToString()}";
            logWriter.WriteLine(log);
        }

        private void FinalizarJogo()
        {
            var ranking = jogadoresAtuais.Values.ToList();
            Ordenar.Selecao(ranking);
            Console.ReadLine();
            Console.Clear();
            
            for (int i = 0; i < ranking.Count; i++)
            {
                var jogador = ranking[i];
                jogador.AdicionarRanking(i + 1);
            }

            var vencedor = ranking.First();
            logWriter.WriteLine("Ranking Final da Partida:");
            Console.WriteLine("\nRanking Final da Partida:\n");
            Console.WriteLine($"O vencedor é: {vencedor.Nome} com {vencedor.Monte.Tamanho()} cartas.\n");
            logWriter.WriteLine($"O vencedor é: {vencedor.Nome} com {vencedor.Monte.Tamanho()} cartas.");
            for (int i = 0; i < ranking.Count; i++)
            {
                var jogador = ranking[i];
                logWriter.WriteLine($"{i + 1}°:{jogador.Nome} Cartas:{jogador.Monte.Tamanho()}.\n");
                Console.WriteLine($"{i + 1}°:{jogador.Nome} Cartas:{jogador.Monte.Tamanho()}.\n");
            }
        }
        public void Sair()
        {
            logWriter.WriteLine("Saindo do jogo...");
            Console.WriteLine("Saindo do jogo...");
            logWriter.Close();
            Environment.Exit(0);
        }

        public void NovoJogo()
        {
            logWriter.WriteLine("Iniciando um novo jogo...");
            Console.WriteLine("Iniciando um novo jogo...");
            IniciarJogo(true);
        }

        public void JogarNovamente()
        {
            logWriter.WriteLine("Reiniciando o jogo...");
            Console.WriteLine("Reiniciando o jogo...");
            IniciarJogo(false);
        }
        public void MostrarJogadoresAnteriores()
        {
            Console.WriteLine("\n=== Jogadores de jogos anteriores ===");
            logWriter.WriteLine("=== Jogadores de jogos anteriores ===");
            foreach (var jogador in jogadoresJogados)
            {
                Console.WriteLine($"{jogador.Value.Nome}");
                logWriter.WriteLine($"{jogador.Value.Nome}");

            }
        }

        public void MostrarHistoricoJogador()
        {
            Console.WriteLine("Digite o nome do jogador para ver o histórico:");
            string nomeJogador = Console.ReadLine();
            logWriter.WriteLine($"Procurando histórico do jogador {nomeJogador}");

            List<Jogador> jogadoresCombinados = new List<Jogador>();
            jogadoresCombinados.AddRange(jogadoresAtuais.Values);
            jogadoresCombinados.AddRange(jogadoresJogados.Values);

            Jogador jogador = null;
            foreach (var j in jogadoresCombinados)
            {
                if (j.Nome == nomeJogador)
                {
                    jogador = j;
                    break;
                }
            }

            if (jogador != null)
            {
                jogador.MostrarRanking(logWriter);
            }
            else
            {
                logWriter.WriteLine($"Jogador {nomeJogador} não foi encontrado");
                Console.WriteLine("Jogador não encontrado.");
            }
        }

    }

}
