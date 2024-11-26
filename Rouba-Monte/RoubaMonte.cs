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
        private List<Jogador> jogadoresAtuais;
        private List<Jogador> jogadoresJogados;
        private Fila monteCompra;
        private Pilha areaDescarte;
        private StreamWriter logWriter;
        private int numBaralhos;

        public RoubaMonte()
        {
            jogadoresAtuais = new List<Jogador>();
            jogadoresJogados = new List<Jogador>();
            monteCompra = new Fila(52);
            areaDescarte = new Pilha();

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
                logWriter.WriteLine($"A quantidade de jogares dessa partida é de: {numeroDeJogadores}");

                jogadoresJogados.AddRange(jogadoresAtuais);
                jogadoresAtuais.Clear();
                for (int i = 0; i < numeroDeJogadores; i++)
                {
                    Console.WriteLine($"Digite o nome do jogador {i + 1}:");
                    string nomeJogador = Console.ReadLine();
                    logWriter.WriteLine($"Jogador {nomeJogador} foi adicionado na partida.");
                    Jogador jogador = new Jogador(nomeJogador);
                    jogadoresAtuais.Add(jogador);
                }

                monteCompra = new Fila(totalCartas);
                areaDescarte = new Pilha();
                CriarMonteDeCompra(numBaralhos);
                logWriter.WriteLine("Monte de compras criado...");
                EmbaralharMonte();
                logWriter.WriteLine("Monte embaralhado...");
            }
            else
            {
                int totalCartas = numBaralhos * 52;
                monteCompra = new Fila(totalCartas);
                areaDescarte = new Pilha();
                CriarMonteDeCompra(numBaralhos);
                logWriter.WriteLine("Monte de compras criado...");
                EmbaralharMonte();
                logWriter.WriteLine("Monte embaralhado...");
                foreach (var jogador in jogadoresAtuais)
                {
                    jogador.LimparMonte();
                    logWriter.WriteLine($"Monte do jogador {jogador} limpo.");
                }
            }

            Console.WriteLine("Escolha o jogador que iniciará: (Escolha por número)");

            int jogadorInicio = int.Parse(Console.ReadLine()) - 1;
            logWriter.WriteLine($"Jogador {jogadorInicio} iniciará a partida.");

            while (monteCompra.Quantidade() > 0)
            {
                foreach (var jogador in jogadoresAtuais)
                {
                    if (monteCompra.Quantidade() == 0) break;
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
                        monteCompra.Inserir(new Carta(valor, naipe));
                    }
                }
            }
        }

        private void EmbaralharMonte()
        {
            var cartas = new List<Carta>();
            while (monteCompra.Quantidade() > 0)
            {
                cartas.Add(monteCompra.Remover());
            }

            Random rand = new Random();
            while (cartas.Count > 0)
            {
                int index = rand.Next(cartas.Count);
                monteCompra.Inserir(cartas[index]);
                cartas.RemoveAt(index);
            }
            logWriter.WriteLine("Monte de compra embaralhado.");
            Console.WriteLine("Monte de compra embaralhado.");
        }

        private void RealizarJogada(Jogador jogador)
        {
            Console.WriteLine($"\nVez do jogador: {jogador.Nome}\n");

            if (monteCompra.Quantidade() > 0)
            {
                Carta cartaComprada = monteCompra.Remover();
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
                        areaDescarte.Empilhar(cartaComprada);
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
                if (outroJogador != jogador && !outroJogador.Monte.Vazia())
                {
                    Carta cartaTopoOutroMonte = outroJogador.Monte.Peek();
                    if (cartaTopoOutroMonte.Valor == cartaComprada.Valor)
                    {
                        jogador.Monte.Empilhar(cartaComprada);
                        jogador.Monte.Empilhar(outroJogador.Monte.Desempilhar());
                        LogJogada(jogador, "Roubou o monte de", cartaComprada);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool VerificarDescarte(Jogador jogador, Carta cartaComprada)
        {
            if (!areaDescarte.Vazia() && areaDescarte.Peek().Valor == cartaComprada.Valor)
            {
                jogador.Monte.Empilhar(cartaComprada);
                jogador.Monte.Empilhar(areaDescarte.Desempilhar());
                LogJogada(jogador, "Pegou da área de descarte", cartaComprada);
                return true;
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

            var ranking = jogadoresAtuais.OrderByDescending(j => j.Monte.Tamanho()).ToList();


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

        // Mostra o ranking de jogadores
        private void MostrarRanking()
        {
            var ranking = jogadoresAtuais.OrderByDescending(j => j.Monte.Tamanho()).ToList();
            Console.WriteLine("\nRanking da partida:");
            logWriter.WriteLine("\nRanking da partida:");
            foreach (var jogador in ranking)
            {
                Console.WriteLine($"{jogador.Nome}: {jogador.Monte.Tamanho()} cartas");
                logWriter.WriteLine($"{jogador.Nome}: {jogador.Monte.Tamanho()} cartas");
            }
        }

        // Método para sair do jogo
        public void Sair()
        {
            logWriter.WriteLine("Saindo do jogo...");
            Console.WriteLine("Saindo do jogo...");
            logWriter.Close();
            Environment.Exit(0);
        }

        // Método para iniciar um novo jogo
        public void NovoJogo()
        {
            logWriter.WriteLine("Iniciando um novo jogo...");
            Console.WriteLine("Iniciando um novo jogo...");
            IniciarJogo(true);
        }

        // Método para jogar novamente
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
                Console.WriteLine(jogador.Nome);
                logWriter.WriteLine(jogador.Nome);
            }
        }
        // Método para mostrar o histórico de um jogador
        public void MostrarHistoricoJogador()
        {
            Console.WriteLine("Digite o nome do jogador para ver o histórico:");
            string nomeJogador = Console.ReadLine();
            logWriter.WriteLine($"Procurando histórico do jogador {nomeJogador}");

            // Combina as listas de jogadores atuais e anteriores
            var jogador = jogadoresAtuais.Concat(jogadoresJogados).FirstOrDefault(j => j.Nome == nomeJogador);

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
