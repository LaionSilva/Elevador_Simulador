using System;
using System.Collections.Generic;
using System.Text;

namespace Elevador_Simulador
{
    class Andar
    {
        private byte Numero;
        private bool subir;
        private bool descer;
        private bool desembarcar;

        public byte numero { get => this.Numero; private set { this.Numero = value; } }


        public Andar() { }

        public Andar(byte numero)
        {
            this.numero = numero;
            this.subir = false;
            this.descer = false;
            this.desembarcar = false;
        }


        #region get_solicitacoes
        /// <summary>
        /// Verifica se houve uma solicitação de subida neste andar
        /// </summary>
        public bool needSubir()
        {
            return this.subir;
        }

        /// <summary>
        /// Verifica se houve uma solicitação de descida neste andar
        /// </summary>
        public bool needDescer()
        {
            return this.descer;
        }

        /// <summary>
        /// Verifica se houve uma solicitação de desembarque neste andar
        /// </summary>
        public bool needDesembarcar()
        {
            return this.desembarcar;
        }
        #endregion


        #region set_solicitacoes
        /// <summary>
        /// Solicitar subida a partir deste andar
        /// </summary>
        public void Subir()
        {
            this.subir = true;
        }
        /// <summary>
        /// Elevador está neste andar, subindo
        /// </summary>
        public void Subiu()
        {
            this.subir = false;
        }

        /// <summary>
        /// Solicitar descida a partir deste andar
        /// </summary>
        public void Descer()
        {
            this.descer = true;
        }
        /// <summary>
        /// Elevador está neste andar, descendo
        /// </summary>
        public void Desceu()
        {
            this.descer = false;
        }

        /// <summary>
        /// Solicitar desembarque neste andar
        /// </summary>
        public void Desembarcar()
        {
            this.desembarcar = true;
        }

        /// <summary>
        /// Elevador está neste andar, desembarcando
        /// </summary>
        public void Desembarcou()
        {
            this.desembarcar = false;
        }
        #endregion

    }
}