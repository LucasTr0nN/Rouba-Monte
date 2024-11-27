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
        private Queue<Carta> monteCompra;
        private List<Carta> areaDescarte;
        private StreamWriter logWriter;
        private int numBaralhos;

        public RoubaMonte()
        {
            jogadoresAtuais = new Dictionary<string, Jogador>();
            jogadoresJogados = new Dictionary<string, Jogador>();
            monteCompra = new Queue<Carta>();
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
                    }
                    else if (jogadoresJogados.ContainsKey(nomeJogador))
                    {
                        jogador = jogadoresJogados[nomeJogador];
                        jogadoresAtuais.Add(nomeJogador, jogador);
                        logWriter.WriteLine($"Usando o Jogador {nomeJogador} (jogador anterior).");
                        Console.WriteLine($"Jogador {nomeJogador} já jogou anteriormente, usando jogador existente.");
                    }
                    else
                    {
                        jogador = new Jogador(nomeJogador);
                        jogadoresAtuais.Add(nomeJogador, jogador);
                        logWriter.WriteLine($"Jogador {nomeJogador} foi adicionado como novo.");
                    }
                }

                monteCompra = new Queue<Carta>();
                areaDescarte = new List<Carta>();
                CriarMonteDeCompra(numBaralhos);
                logWriter.WriteLine("Monte de compras criado...");
                EmbaralharMonte();
                logWriter.WriteLine("Monte embaralhado...");
            }
            else
            {
                int totalCartas = numBaralhos * 52;
                monteCompra = new Queue<Carta>();
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
                    if (monteCompra.Count() == 0) break;
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
                        monteCompra.Enqueue(new Carta(valor, naipe));
                    }
                }
            }
        }

        private void EmbaralharMonte()
        {
            var cartas = new List<Carta>();
            while (monteCompra.Count() > 0)
            {
                cartas.Add(monteCompra.Dequeue());
            }

            Random rand = new Random();
            while (cartas.Count > 0)
            {
                int index = rand.Next(cartas.Count);
                monteCompra.Enqueue(cartas[index]);
                cartas.RemoveAt(index);
            }
            logWriter.WriteLine("Monte de compra embaralhado.");
            Console.WriteLine("Monte de compra embaralhado.");
        }

        private void RealizarJogada(Jogador jogador)
        {
            Console.WriteLine($"\nVez do jogador: {jogador.Nome}\n");

            if (monteCompra.Count() > 0)
            {
                Carta cartaComprada = monteCompra.Dequeue();
                Console.WriteLine($"{jogador.Nome} comprou a carta: {cartaComprada}");

                bool jogadaEfetuada = false;

                while (!jogadaEfetuada)
                {
                    if (VerificarRoubo(jogador, cartaComprada))
                    {
                        jogadaEfetuada = true;
                    }
                    else if (VerificarDescarte(jogador, cartaComprada))
                    {
                        jogadaEfetuada = true;
                    }
                    else if (VerificarProprioMonte(jogador, cartaComprada))
                    {
                        jogadaEfetuada = true;
                    }
                    else
                    {
                        areaDescarte.Add(cartaComprada);
                        jogadaEfetuada = true;
                        LogJogada(jogador, "Descartou", cartaComprada);
                    }
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
                        LogJogada(jogador, "Roubou o monte inteiro de", cartaComprada);
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
            Console.WriteLine(log);
        }

        private void FinalizarJogo()
        {
            var ranking = jogadoresAtuais.Values.ToList();
            Ordenar.Selecao(ranking);

            logWriter.WriteLine("Ranking Final da Partida:");
            Console.WriteLine("Ranking Final da Partida:");
            for (int i = 0; i < ranking.Count; i++)
            {
                var jogador = ranking[i];
                logWriter.WriteLine($"{i + 1}° Lugar: {jogador.Nome} com {jogador.Monte.Tamanho()} cartas.");
                Console.WriteLine($"{i + 1}° Lugar: {jogador.Nome} com {jogador.Monte.Tamanho()} cartas.");

                jogador.AdicionarRanking(i + 1);
            }

            var vencedor = ranking.First();
            Console.Clear();
            Console.WriteLine($"\nO vencedor é: {vencedor.Nome} com {vencedor.Monte.Tamanho()} cartas.");
            logWriter.WriteLine($"\nO vencedor é: {vencedor.Nome} com {vencedor.Monte.Tamanho()} cartas.");

            MostrarRanking();
        }
        private void MostrarRanking()
        {
            var ranking = jogadoresAtuais.Values.ToList();
            Ordenar.Selecao(ranking);
            Console.WriteLine("\nRanking da partida:");
            logWriter.WriteLine("\nRanking da partida:");
            foreach (var jogador in ranking)
            {
                Console.WriteLine($"{jogador.Nome}: {jogador.Monte.Tamanho()} cartas");
                logWriter.WriteLine($"{jogador.Nome}: {jogador.Monte.Tamanho()} cartas");
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
                Console.WriteLine($"Jogador {jogador.Value.Nome} foi adicionado na partida.");
                logWriter.WriteLine($"Jogador {jogador.Value.Nome} foi adicionado na partida.");

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

