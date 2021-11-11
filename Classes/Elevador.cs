using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elevador_Simulador.Classes
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
        private void SetAndarDestino()
        {
            if(this.GetFila() != null)
            {
                this.andar_destino = this.GetFila()[0];
                this.fila.RemoveAt(0);
            }
        }
        private void SetFila(int element) 
        {
            this.fila.Add(element);
        }
        #endregion

        public void NovoDestino(int novo_destino) 
        {
            if ( (this.GetAndarAtual() != novo_destino) && (this.GetAndarDestino() != novo_destino) )
            {
                if (!this.GetFila().Contains(novo_destino))
                    this.SetFila(novo_destino);
            }
        }
        private void Emergencia() 
        {
            if (this.status == StatusElevador.Parado)
                Console.WriteLine("Alguém está preso");
            else
                this.status = StatusElevador.Parado;
        }

        public async Task Movimento()
        {
            if(this.GetStatus() != StatusElevador.Parado)
            {
                if (this.GetAndarDestino() > this.GetAndarAtual())
                    this.SetAndarAtual(StatusElevador.Subindo);
                else if (this.GetAndarDestino() < this.GetAndarAtual())
                    this.SetAndarAtual(StatusElevador.Descendo);
                else
                    this.SetAndarDestino();
            }
        }

    }
}
