using System;
using System.Collections.Generic;
using System.Text;

namespace Elevador_Simulador.Classes
{
    class Predio
    {
        private Andar[] Andares;

        public Andar[] andares { get { return this.Andares; } private set { this.Andares = value; } }
        public int qtdAndares { get { return this.Andares.Length; } }


        public Predio(Andar[] andares)
        {
            this.andares = andares;
        }
    }
}
