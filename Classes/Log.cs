using System;
using System.Collections.Generic;
using System.IO;


namespace Elevador_Simulador.Classes
{
    class Log
    {
        // Lista de logs
        private List<string> Logs = new List<string>();

        // Caminho do arquivo de registro dos logs
        private string pathDataLog;

        public List<string> logs { get => this.Logs; }

        public Log(string pathDataLog, string labelAndarInicial = "Térreo")
        {
            this.pathDataLog = pathDataLog;

            string[] logs = new string[0];
            Files.ReadFile(pathDataLog, ref logs);
            this.Logs.AddRange(logs);

            //this.Logs.Add($"{DateTime.Now} | Inicio de operação \t Andar atual: {labelAndarInicial}");
            this.SaveLogs();
        }


        /// <summary>
        /// Registrar novo log
        /// </summary>
        public void NovoLog(string log)
        {
            this.Logs.Add(log);
            this.SaveLogs();
        }

        /// <summary>
        /// Registrar logs salvos na memória
        /// </summary>
        private void SaveLogs()
        {
            Files.WriteFile(pathDataLog, this.Logs.ToArray());
        }

        /// <summary>
        /// Registrar log de final de operação
        /// </summary>
        public void FimOperacao(string labelAndarAtual)
        {
            this.Logs.Add($"{DateTime.Now} | Final de operação \t Andar atual: {labelAndarAtual}");
            this.SaveLogs();
        }
    }


    internal class Files
    {
        /// <summary>
        /// Ler documento de texto.txt
        /// </summary>
        /// <param name="path">Caminho até o arquivo de texto.txt</param>
        /// <param name="lines">array para armazenar o conteudo lido</param>
        public static void ReadFile(string path, ref string[] lines, int size = 5)
        {
            try { 
                lines = new string[size];
                string[] dados = System.IO.File.ReadAllLines(path);
                Array.Copy(dados, dados.Length - 1 - size, lines, 0, size);
            }
            catch (FileNotFoundException) { }
            catch (Exception) { }
        }

        /// <summary>
        /// Escrever em um arquivo de texto.txt
        /// </summary>
        /// <param name="path">Caminho até o arquivo de texto.txt</param>
        /// <param name="lines">Conteudo a ser escrito</param>
        public static void WriteFile(string path, string[] lines)
        {
            try { WriteFile(path, lines.Length > 0 ? String.Join("\n", lines) : ""); }
            catch (FileNotFoundException) { }
            catch (Exception) { }
        }

        /// <summary>
        /// Escrever em um arquivo de texto.txt
        /// </summary>
        /// <param name="path">Caminho até o arquivo de texto.txt</param>
        /// <param name="text">Conteudo a ser escrito</param>
        public static void WriteFile(string path, string text = "")
        {
            try { System.IO.File.WriteAllText(path, text); }
            catch (FileNotFoundException) { }
            catch (Exception) { }
        }
    }
}
