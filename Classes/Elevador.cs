using Elevador_Simulador.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elevador_Simulador
{
    class Elevador
    {
        #region Construtores
        public Elevador(Predio predio) 
        {
            this.Predio = predio;
            this.status = StatusElevador.Parado;
            this.andar_atual = 0; //Térreo
            this.botoes = new bool[this.Predio.qtdAndares];

            this.ClickButton += this.ClicarBotao;

            this.MoverPortas = false;

            this.Movimento = new Task(this.IniciarOperação);
            this.Movimento.Start();
        }
        #endregion

        // Task de movimentação do elevador
        private Task Movimento;

        private delegate void ClickButtonElevador(int button);
        private event ClickButtonElevador ClickButton;

        // Milissegundos
        private const int timerTrocarAndar = 1000;
        private const int timerPartidaElevador = 1000;
        private const int timerFindNewDestiny = 500;

        private Predio Predio;
        private StatusElevador status;
        private int andar_atual;
        private int andar_destino;
        private bool[] botoes;
        private bool MoverPortas;


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
        private void UpStatus()
        {
            if (GetAndarDestino() == this.GetAndarAtual())
                this.SetStatus(StatusElevador.Parado);
            else if (GetAndarDestino() > this.GetAndarAtual())
                this.SetStatus(StatusElevador.Subindo);
            else if (GetAndarDestino() < this.GetAndarAtual())
                this.SetStatus(StatusElevador.Descendo);
        }

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
                        {
                            atual.Desceu();
                        }
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
                        {
                            atual.Subiu();
                        }
                        return true;
                    }
                    break;
                case StatusElevador.Parado:
                    if (atual.needDescer() || atual.needSubir() || atual.needDesembarcar() || this.botoes[this.GetAndarAtual()])
                    {
                        atual.Desceu();
                        atual.Subiu();
                        atual.Desembarcou();
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
        #endregion


        public void Emergencia() 
        {
            this.SetStatus(StatusElevador.Parado);
            this.andar_destino = this.GetAndarAtual();

            if (this.status == StatusElevador.Parado)
                Console.WriteLine("Alguém está preso");
            else
                this.status = StatusElevador.Parado;
        }
    }
}
