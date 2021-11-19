using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elevador_Simulador.Classes
{
    internal class Simulador
    {
        private Random random = new Random();
        private Predio Predio;

        private Task Simulator;

        // Tempo em milissegundos
        private int minTimeCall;
        private int maxTimeCall;

        public Simulador(Predio predio, int minTimeCall, int maxTimeCall)
        {
            this.Predio = predio;
            this.minTimeCall = minTimeCall;
            this.maxTimeCall = maxTimeCall;

            this.Simulator = new Task(this.Simular);
        }

        private void Simular()
        {
            while (true)
            {
                Thread.Sleep(random.Next(this.minTimeCall, this.maxTimeCall));

                if (random.Next(10) % 2 == 0)
                    this.Predio.andares[random.Next(this.Predio.qtdAndares)].Subir();
                else
                    this.Predio.andares[random.Next(this.Predio.qtdAndares)].Descer();
            }
        }

        public void IniciarSimulacao()
        {
            this.Simulator.Start();
        }

        public void PararSimulacao()
        {
            this.Simulator.Dispose();
        }
    }
}
