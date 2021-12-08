using Elevador_Simulador.Classes;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace Elevador_Simulador
{
    /// <summary>
    /// Classe enum que representa os estados de movimento de um elevador
    /// </summary>
    public enum StatusElevador
    {
        Subindo,
        Descendo,
        Parado
    }


    class Elevador
    {
        #region atributos da classe
        // Task de movimentação do elevador
        private Task Movimento;

        // Eventos
        private delegate void ClickButtonElevador(int button);
        private event ClickButtonElevador ClickButton;

        // Variáveis de temporização - Em milissegundos
        private int timerTrocarAndar;
        private int timerPartidaElevador;
        private int timerFindNewDestiny;

        // Classes
        private Predio Predio;
        private Log Logs;

        private StatusElevador status;
        private int andar_atual;
        private int andar_destino;
        private bool[] botoes;
        private bool MoverPortas;
        #endregion


        #region Construtores
        public Elevador(Predio predio, Log logs, int timerTrocarAndar = 1000, int timerPartidaElevador = 1000, int timerFindNewDestiny = 500) 
        {
            this.timerTrocarAndar = timerTrocarAndar;
            this.timerPartidaElevador = timerPartidaElevador;
            this.timerFindNewDestiny = timerFindNewDestiny;

            this.Predio = predio;
            this.status = StatusElevador.Parado;
            this.andar_atual = 0; //Térreo
            this.botoes = new bool[this.Predio.qtdAndares];

            this.Logs = logs;

            this.ClickButton += this.GerarLog;
            this.ClickButton += this.ClicarBotao;

            this.MoverPortas = false;

            this.Movimento = new Task(this.IniciarOperação);
            this.Movimento.Start();
        }
        #endregion


        #region Getters And Setters
        public bool CanMoverPortas()
        {
            return this.MoverPortas;
        }
        public int GetQtdAndar()
        {
            return Predio.qtdAndares;
        }
        public int GetAndarAtual()
        {
            return andar_atual;
        }
        public int GetAndarDestino()
        {
            return andar_destino;
        }
        public string[] GetLogs()
        {
            return this.Logs.logs.ToArray();
        }
        public StatusElevador GetStatus()
        {
            return status;
        }
        private void SetStatus(StatusElevador stat)
        {
            this.status = stat;
        }
        public void LiberarPortas()
        {
            this.MoverPortas = false;
        }
        #endregion


        #region tasks_movimentação        
        /// <summary>
        /// Método que controla o funcionamento do elevador
        /// </summary>
        private void IniciarOperação()
        {
            // Loop que verifica se há novas solicitações
            while (true)
            {
                // Loop que realiza os movimentos
                while (true)
                {
                    if (this.MoverPortas) continue;

                    // Atualizar status atual do elevador
                    this.UpStatus();

                    // Buscar novo destino caso o elevador estiver parado
                    if (status == StatusElevador.Parado)
                    {
                        this.defineAndarDestino();
                        this.UpStatus();
                        if(this.GetStatus() != StatusElevador.Parado)
                        {
                            Thread.Sleep(timerPartidaElevador);
                        }
                    }

                    // Se houver intenção de se mover, mova-se
                    if (status != StatusElevador.Parado)
                    {
                        this.MoverElevador();
                    }

                    // Verificar solicitações do andar atual
                    this.MoverPortas = this.verifyAndar();

                    // Fim da série de movimentos
                    if (status == StatusElevador.Parado)
                        break;
                }
                Thread.Sleep(timerFindNewDestiny);
            }
        }

        /// <summary>
        /// Executa um movimento de um andar para outro vizinho
        /// </summary>
        private void MoverElevador()
        {
            if (this.GetStatus() != StatusElevador.Parado)
            {
                Thread.Sleep(timerTrocarAndar);
                switch (this.GetStatus())
                {
                    case StatusElevador.Subindo:
                        this.andar_atual++;
                        break;
                    case StatusElevador.Descendo:
                        this.andar_atual--;
                        break;
                }
            }
        }
        #endregion


        #region métodos de operação do elevador
        /// <summary>
        /// Atualiza o Status do elevador
        /// </summary>
        private void UpStatus()
        {
            if (GetAndarDestino() == this.GetAndarAtual())
                this.SetStatus(StatusElevador.Parado);
            else if (GetAndarDestino() > this.GetAndarAtual())
                this.SetStatus(StatusElevador.Subindo);
            else if (GetAndarDestino() < this.GetAndarAtual())
                this.SetStatus(StatusElevador.Descendo);
        }

        /// <summary>
        /// Define um novo andar de destino 
        /// </summary>
        private void defineAndarDestino()
        {
            var aux = this.andar_destino;
            
            switch (this.GetStatus()){
                case StatusElevador.Subindo:
                    this.andar_destino = this.BuscarDestinoAcima();
                    if (this.andar_destino == aux)
                        this.andar_destino = this.BuscarDestinoAbaixo();
                    break;
                case StatusElevador.Descendo:
                    this.andar_destino = this.BuscarDestinoAbaixo();
                    if (this.andar_destino == aux)
                        this.andar_destino = this.BuscarDestinoAcima();
                    break;
                case StatusElevador.Parado:
                    this.andar_destino = this.BuscarDestinoAcima();
                    if (this.andar_destino == aux)
                        this.andar_destino = this.BuscarDestinoAbaixo();
                    break;
            }
        }

        /// <summary>
        /// Verifica todos os andares acima do atual em busca de uma solicitação pendente
        /// </summary>
        private int BuscarDestinoAcima()
        {
            for (int i = this.GetAndarDestino(); i < this.GetQtdAndar(); i++)
            {
                if (this.Predio.andares[i].needSubir() || this.Predio.andares[i].needDesembarcar())
                {
                    this.SetStatus(StatusElevador.Subindo);
                    return i;
                }
            }
            for (int i = this.GetAndarDestino() + 1; i < this.GetQtdAndar(); i++)
            {
                if (this.Predio.andares[i].needDescer() || this.Predio.andares[i].needDesembarcar())
                {
                    this.SetStatus(StatusElevador.Subindo );
                    return i;
                }
            }
            return this.andar_destino;
        }

        /// <summary>
        /// Verifica todos os andares abaixo do atual em busca de uma solicitação pendente
        /// </summary>
        private int BuscarDestinoAbaixo()
        {
            for (int i = this.GetAndarDestino(); i >= 0; i--)
            {
                if (this.Predio.andares[i].needDescer() || this.Predio.andares[i].needDesembarcar())
                {
                    this.SetStatus(StatusElevador.Descendo);
                    return i;
                }
            }
            for (int i = this.GetAndarDestino() - 1; i >= 0; i--)
            {
                if (this.Predio.andares[i].needSubir() || this.Predio.andares[i].needDesembarcar())
                {
                    this.SetStatus(StatusElevador.Descendo);
                    return i;
                }
            }
            return this.andar_destino;
        }

        /// <summary>
        /// Verifica e atende às solicitações do andar atual
        /// Retorna "True" caso houver solicitação ou "False" caso não houver
        /// </summary>
        public bool verifyAndar()
        {
            Andar atual = this.Predio.andares[this.GetAndarAtual()];
            this.botoes[this.GetAndarAtual()] = false;

            switch (this.GetStatus())
            {
                case StatusElevador.Subindo:
                    if (this.andar_atual == this.GetQtdAndar() - 1 || atual.needSubir() || atual.needDesembarcar() || this.botoes[this.GetAndarAtual()])
                    {
                        this.SetStatus(StatusElevador.Parado);
                        atual.Subiu();
                        atual.Desembarcou();
                        if (this.andar_atual == this.BuscarDestinoAcima())
                            atual.Desceu();
                        return true;
                    }
                    break;
                case StatusElevador.Descendo:
                    if (this.andar_atual == 0 || atual.needDescer() || atual.needDesembarcar() || this.botoes[this.GetAndarAtual()])
                    {
                        this.SetStatus(StatusElevador.Parado);
                        atual.Desceu();
                        atual.Desembarcou();
                        if (this.andar_atual == this.BuscarDestinoAbaixo())
                            atual.Subiu();
                        return true;
                    }
                    break;
                case StatusElevador.Parado:
                    if (atual.needDescer() || atual.needSubir() || atual.needDesembarcar() || this.botoes[this.GetAndarAtual()])
                    {
                        atual.clearSolicitacoes();
                        return true;
                    }
                    break;
            }
            return false;
        }
        #endregion


        #region event click botões elevador
        /// <summary>
        /// Método que dispara o evento responsável pelos comandos dos botões
        /// </summary>
        public void EventClickButton(int andar)
        {
            this.ClickButton(andar);
        }

        /// <summary>
        /// Método inscrito no Event ClickButton para startar os comandos de cada botão
        /// </summary>
        private void ClicarBotao(int botao)
        {
            this.Predio.andares[botao].Desembarcar();
        }

        /// <summary>
        /// Método inscrito no Event ClickButton para gerar registro de log da solicitação realizada 
        /// </summary>
        private void GerarLog(int andar)
        {
            if(!this.Predio.andares[andar].needDesembarcar())
                this.Logs.NovoLog(this.GerarLog(this.GetAndarAtual(), andar));
        }
        #endregion


        /// <summary>
        /// Método que executa as operações de emergência
        /// </summary>
        public void Emergencia() 
        {
            this.SetStatus(StatusElevador.Parado);
            this.andar_destino = this.GetAndarAtual();

            if (this.status == StatusElevador.Parado)
                Console.WriteLine("Alguém está preso");
            else
                this.status = StatusElevador.Parado;
        }

        /// <summary>
        /// Gerar mensagens de log
        /// </summary>
        private string GerarLog(int nAndarOrigem, int nAndarDestino)
        {
            string origem = nAndarOrigem != 0 ? $"{nAndarOrigem}° Andar" : "Térreo";
            string destino = nAndarDestino != 0 ? $"{nAndarDestino}° Andar" : "Térreo";

            return $"{DateTime.Now.ToString()} | {origem} => {destino}";
        }

        /// <summary>
        /// Alerta a classe Logs que a aplicação será finalizada
        /// Solicita a inserção do log de final de operação
        /// </summary>
        public void FinalizarLogs()
        {
            this.Logs.FimOperacao(this.GetAndarAtual() == 0 ? "Térreo" : $"{this.GetAndarAtual()}° Andar");
        }
    }
}
