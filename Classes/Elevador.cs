﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elevador_Simulador
{
    class Elevador
    {
        #region Construtores
        public Elevador() 
        {
            this.ConstrutorDefault(5);
        }
        public Elevador(int qtd) 
        {
            this.qtd_andar = qtd;
            this.ConstrutorDefault(this.qtd_andar);
        }
        private void ConstrutorDefault(int qtd)
        {
            this.qtd_andar = qtd;
            this.status = StatusElevador.Parado;
            this.andar_atual = 0; //Térreo
            this.botoes = new bool[qtd];
        }
        #endregion

        private StatusElevador status;
        private int qtd_andar;
        private int andar_atual;
        private int andar_destino;
        private bool[] botoes;
        private List<int> fila;

        #region Getters And Setters
        public int GetQtdAndar()
        {
            return qtd_andar;
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
        private List<int> GetFila()
        {
            if (this.fila == null)
                this.fila = new List<int>();
            return this.fila;
        }
        private void SetAndarAtual(StatusElevador status)
        {
            this.SetStatus(status);
            switch (status)
            {
                case StatusElevador.Subindo:
                    this.andar_atual++;
                    break;
                case StatusElevador.Descendo:
                    this.andar_atual--;
                    break;

            }
        }
        private void SetAndarDestino(Andar[] andares)
        {
            var aux = this.andar_destino;
            
            switch (this.GetStatus()){
                case StatusElevador.Subindo:
                    this.DefineDestinoAcima(andares);
                    if (this.andar_destino == aux)
                        this.DefineDestinoAbaixo(andares);
                    break;
                case StatusElevador.Descendo:
                    this.DefineDestinoAbaixo(andares);
                    if (this.andar_destino == aux)
                        this.DefineDestinoAcima(andares);
                    break;
                case StatusElevador.Parado:
                    this.DefineDestinoAcima(andares);
                    if (this.andar_destino == aux)
                        this.DefineDestinoAbaixo(andares);
                    break;
            }
        }
        private void DefineDestinoAcima(Andar[] andares)
        {
            for (int i = this.GetAndarDestino(); i < this.GetQtdAndar(); i++)
            {
                if (andares[i].needSubir() || andares[i].needDescer())
                {
                    this.andar_destino = i;
                    this.SetStatus(StatusElevador.Subindo);
                    return;
                }
            }
        }
        private void DefineDestinoAbaixo(Andar[] andares)
        {
            for (int i = this.GetAndarDestino(); i >= 0; i--)
            {
                if (andares[i].needDescer() || andares[i].needSubir())
                {
                    this.andar_destino = i;
                    this.SetStatus(StatusElevador.Descendo);
                    return;
                }
            }
        }

        private void SetStatus(StatusElevador stat)
        {
            this.status = stat;
        }
        private void SetFila(int element) 
        {
            this.fila.Add(element);
        }
        public void ClickBotao(int andar)
        {
            this.botoes[andar] = true;
        }
        #endregion

        public void NovoDestino(int novo_destino) 
        {
            
        }
        private void Emergencia() 
        {
            if (this.status == StatusElevador.Parado)
                Console.WriteLine("Alguém está preso");
            else
                this.status = StatusElevador.Parado;
        }

        public async void Movimento(Andar[] andares)
        {
            
            if (this.GetAndarDestino() > this.GetAndarAtual())
                this.SetAndarAtual(StatusElevador.Subindo);
            else if (this.GetAndarDestino() < this.GetAndarAtual())
                this.SetAndarAtual(StatusElevador.Descendo);
            else
                this.SetAndarDestino(andares);

            this.VerifyAndar(andares);
          
        }
        public void VerifyAndar(Andar[] andares)
        {
            Andar atual = andares[this.GetAndarAtual()];
            switch (this.GetStatus())
            {
                case StatusElevador.Subindo:
                    if (atual.needSubir() || this.botoes[this.GetAndarAtual()])
                    {
                        this.SetStatus(StatusElevador.Parado);
                        this.botoes[this.GetAndarAtual()] = false;
                        //atual.Desembarcar();
                        atual.Subiu();
                    }
                    if (this.GetAndarAtual() == this.GetQtdAndar() - 1)
                    {
                        this.SetStatus(StatusElevador.Parado);
                        this.botoes[this.GetAndarAtual()] = false;
                        atual.Desceu();
                    }
                            
                    break;
                case StatusElevador.Descendo:
                    if (atual.needDescer() || this.botoes[this.GetAndarAtual()])
                    { 
                        this.SetStatus(StatusElevador.Parado);
                        this.botoes[this.GetAndarAtual()] = false;
                        //atual.Desembarcar();
                        atual.Desceu();
                    }
                    break;
            }
            
        }

    }
}
