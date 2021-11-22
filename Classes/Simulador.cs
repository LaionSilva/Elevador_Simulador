using System;
using System.Threading;
using System.Threading.Tasks;


namespace Elevador_Simulador.Classes
{
    internal class Simulador
    {
        private Random random = new Random();
        private Predio Predio;
        private Task Simulator;

        // Intervalos de tempo entre as chamadas - Em milissegundos
        private int minTimeCall;
        private int maxTimeCall;

        private bool parar;


        public Simulador(Predio predio, int minTimeCall, int maxTimeCall)
        {
            this.Predio = predio;
            this.minTimeCall = minTimeCall;
            this.maxTimeCall = maxTimeCall;
            this.parar = false;

            this.Simulator = new Task(this.Simular);
        }


        /// <summary>
        /// Método que realiza a simulação de chamadas ao elevador
        /// </summary>
        private void Simular()
        {
            while (!this.parar)
            {
                if (random.Next(10) % 2 == 0)
                    this.Predio.andares[random.Next(this.Predio.qtdAndares)].Subir();
                else
                    this.Predio.andares[random.Next(this.Predio.qtdAndares)].Descer();

                Thread.Sleep(random.Next(this.minTimeCall, this.maxTimeCall));
            }
            this.parar = false;
        }

        /// <summary>
        /// Iniciar simulação de chamadas
        /// </summary>
        public void IniciarSimulacao()
        {
            if(!this.parar)
                this.Simulator.Start();
        }

        /// <summary>
        /// Parar simulação de chamadas
        /// </summary>
        public void PararSimulacao()
        {
            this.parar = true;
        }
    }
}
