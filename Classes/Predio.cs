using System;
using System.Collections.Generic;
using System.Text;

namespace Elevador_Simulador.Classes
{
    class Predio
    {
        private Andar[] Andares;

        // Variáveis para controle de multiplos andares em painéis pequenos
        private byte nGrupos;
        private byte sizeGrupo;

        public Andar[] andares { get { return this.Andares; } private set { this.Andares = value; } }
        public int qtdAndares { get { return this.Andares.Length; } }


        public Predio(byte nAndares, byte sizeGrupoBotao = 0)
        {
            nAndares %= 100; // Limitar número de andares para no máximo 99
            this.nGrupos = (byte)((sizeGrupoBotao > 0) ? Math.Ceiling((decimal)(nAndares - 1) / (decimal)sizeGrupoBotao) : 0);
            this.sizeGrupo = sizeGrupoBotao != 0 ? sizeGrupoBotao : nAndares;

            this.Andares = new Andar[nAndares];
            for(byte i = 0; i < nAndares; i++)
            {
                this.andares[i] = new Andar(i);
            }
        }

        public int[] getGrupoBotoes(ref byte encoder)
        {
            int[] ids = new int[this.sizeGrupo];
            for(byte i = 0; i < this.sizeGrupo; i++)
            {
                ids[i] = this.andares[1 + encoder * this.sizeGrupo + i].numero;
                if (encoder * this.sizeGrupo + i == this.qtdAndares - 2) 
                    break;
            }
            return ids;
        }

        public void avancarGrupo(ref byte encoder)
        {
            if (encoder < this.nGrupos - 1)
                encoder++;
        }

        public void voltarGrupo(ref byte encoder)
        {
            if (encoder > 0)
                encoder--;
        }
    }
}
