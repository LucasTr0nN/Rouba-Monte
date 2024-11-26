using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Rouba_Monte
{

    class Program
    {
        static void Main(string[] args)
        {
            RoubaMonte jogo = new RoubaMonte();
            bool continuarJogando = true;
            bool primeiravez = true;

            while (continuarJogando)
            {
                if (primeiravez)
                {
                    Console.WriteLine("Bem-vindo ao jogo Rouba Monte!");
                    jogo.NovoJogo();
                    primeiravez = false;
                }
                else
                {
                    Console.WriteLine("\nMENU:\n");
                    Console.WriteLine("1. Novo Jogo");
                    Console.WriteLine("2. Jogar novamente");
                    Console.WriteLine("3. Mostrar Histórico de Jogador");
                    Console.WriteLine("4. Mostrar jogadores anteriores");
                    Console.WriteLine("5. Sair");
                    Console.Write("Escolha uma opção: ");
                    int tec = int.Parse(Console.ReadLine());
                    switch (tec)
                    {
                        case 1:
                            Console.Clear();
                            jogo.NovoJogo();
                            break;
                        case 2:
                            Console.Clear();
                            jogo.JogarNovamente();
                            break;
                        case 3:
                            Console.Clear();
                            jogo.MostrarHistoricoJogador();
                            break;
                        case 4:
                            Console.Clear();
                            jogo.MostrarJogadoresAnteriores();
                            break;
                        case 5:
                            Console.Clear();
                            jogo.Sair();
                            continuarJogando = false;
                            break;
                        default:
                            Console.WriteLine("Opção inválida. Tente novamente.");
                            break;
                    }

                    if (continuarJogando)
                    {
                        Console.WriteLine("\nDeseja jogar novamente? (s/n)");
                        string resposta = Console.ReadLine().ToLower();
                        continuarJogando = resposta == "s";
                    }
                }
            }
        }
    }
}