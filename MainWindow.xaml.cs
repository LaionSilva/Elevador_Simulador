using Elevador_Simulador.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;

namespace Elevador_Simulador
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region atributos da classe
        private const int nBotoesPainelInterno = 6; // Número de botões numerados do painel internos
        private const int nAndares = 15; // Número Máximo de andares, contando com o térreo

        // Temporizador da aplicação
        private static DispatcherTimer timer0;
        private static DispatcherTimer timer1;

        private int andarAtual;

        // Flags
        private bool modoManual;
        private bool emergencia;

        // Variáveis de controle do movimento das portas
        private bool moverPortas;
        private bool posicaoPortas;
        private int encoderPotas;

        // Classes
        private Elevador Elevador;
        private Predio Predio;
        private Simulador Simulador;

        private int[] idBotoesInt;
        private int[] idBotoesExt;
        private byte encoderInt;
        private byte encoderExt;

        // Configuração de cores da interface
        private SolidColorBrush colorButtonOff = Brushes.AntiqueWhite;
        private SolidColorBrush colorButtonOnpending = Brushes.Yellow;
        private SolidColorBrush colorButtonOn = Brushes.DarkOrange;
        private SolidColorBrush colorLedOff = Brushes.AntiqueWhite;
        private SolidColorBrush colorLedOn = Brushes.DarkOrange;
        private SolidColorBrush colorLedEmergenciaOff = Brushes.AntiqueWhite;
        private SolidColorBrush colorLedEmergenciaOn = Brushes.Red;
        private SolidColorBrush colorDisplayOff = Brushes.Black;
        private SolidColorBrush colorDisplayOn = Brushes.OrangeRed;
        #endregion


        #region construtores
        public MainWindow()
        {
            // Inicialização das classes do elevador
            this.Predio = new Predio(nAndares, nBotoesPainelInterno);
            this.Elevador = new Elevador(this.Predio);

            this.Simulador = new Simulador(this.Predio, 5000, 20000);
            InitializeComponent();

            Files.WriteFile(@"D:\Usuário\Desktop\teste.txt", "Laion Fernandes");

            this.andarAtual = 0;

            // Flags
            this.modoManual = false;
            this.emergencia = false;
            this.moverPortas = false;
            this.posicaoPortas = false;
            this.encoderPotas = 0;

            this.idBotoesInt = new int[nBotoesPainelInterno];
            this.idBotoesExt = new int[nBotoesPainelInterno];
            this.encoderInt = 0;
            this.encoderExt = 0;

            // Inscrever evento em um temporizador
            timer0 = new DispatcherTimer();
            timer0.Interval = TimeSpan.FromMilliseconds(100);
            timer0.Tick += CheckFlags;
            timer0.Start();
            
            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(33);
            timer1.Tick += MoverPortas;
            timer1.Start();
        }
        #endregion


        #region botões_interface_painel_internos
        private void button_t_Click(object sender, RoutedEventArgs e)
        {
            if(!this.isSystemLock())
                this.Elevador.EventClickButton(0);
        }

        private void button_1_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isSystemLock() && this.idBotoesInt[0] > 0)
                this.Elevador.EventClickButton(this.idBotoesInt[0]);
        }

        private void button_2_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isSystemLock() && this.idBotoesInt[1] > 0)
                this.Elevador.EventClickButton(this.idBotoesInt[1]);
        }

        private void button_3_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isSystemLock() && this.idBotoesInt[2] > 0)
                this.Elevador.EventClickButton(this.idBotoesInt[2]);
        }

        private void button_4_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isSystemLock() && this.idBotoesInt[3] > 0)
                this.Elevador.EventClickButton(this.idBotoesInt[3]);
        }

        private void button_5_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isSystemLock() && this.idBotoesInt[4] > 0)
                this.Elevador.EventClickButton(this.idBotoesInt[4]);
        }

        private void button_6_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isSystemLock() && this.idBotoesInt[5] > 0)
                this.Elevador.EventClickButton(this.idBotoesInt[5]);
        }

        private void button_em_Click(object sender, RoutedEventArgs e)
        {
            this.emergencia = !this.emergencia;

            if (this.emergencia)
            {
                this.Elevador.Emergencia();
                foreach (Andar a in this.Predio.andares)
                {
                    a.Desembarcou();
                    a.Subiu();
                    a.Desceu();
                }
            }
        }

        private void upGrupoInt_Click(object sender, RoutedEventArgs e)
        {
            this.Predio.avancarGrupo(ref this.encoderInt);
        }

        private void downGrupoInt_Click(object sender, RoutedEventArgs e)
        {
            this.Predio.voltarGrupo(ref this.encoderInt);
        }
        #endregion


        #region botões_interface_painel_externo
        /// <summary>
        /// Chamar elevador para subir
        /// </summary>
        private void botaoPainelExterno_subir(object sender, MouseButtonEventArgs e)
        {
            if (!this.Predio.andares[this.andarAtual].needSubir() && !this.isSystemLock() && this.modoManual)
            {
                this.Predio.andares[this.andarAtual].Subir();
            }
        }

        /// <summary>
        /// Chamar elevador para descer
        /// </summary>
        private void botaoPainelExterno_descer(object sender, MouseButtonEventArgs e)
        {
            if (!this.Predio.andares[this.andarAtual].needDescer() && !this.isSystemLock() && this.modoManual)
            {
                this.Predio.andares[this.andarAtual].Descer();
            }
        }

        /// <summary>
        /// Definir sistema para o modo manual
        /// </summary>
        private void checkBox_manual_Checked(object sender, RoutedEventArgs e)
        {
            this.modoManual = true;
            this.checkBox_automatico.IsChecked = false;
            this.Simulador.IniciarSimulacao();
        }

        /// <summary>
        /// Definir sistema para o modo automático
        /// </summary>
        private void checkBox_automatico_Checked(object sender, RoutedEventArgs e)
        {
            this.modoManual = false;
            this.checkBox_manual.IsChecked = false;
            this.Simulador.PararSimulacao();
        }
        #endregion


        #region botões_acessar_andares
        /// <summary>
        /// Acessar painel externo do andar: Térreo
        /// </summary>
        private void button_setAndar_t_Click(object sender, RoutedEventArgs e)
        {
            this.Legenda.Content = "Térreo";
            this.andarAtual = 0;
        }

        /// <summary>
        /// Acessar painel externo do andar: 1° andar
        /// </summary>
        private void button_setAndar_1_Click(object sender, RoutedEventArgs e)
        {
            if (this.idBotoesExt[0] > 0)
            {
                this.Legenda.Content = $"{this.idBotoesExt[0]}° Andar";
                this.andarAtual = this.idBotoesExt[0];
            }
        }

        /// <summary>
        /// Acessar painel externo do andar: 2° andar
        /// </summary>
        private void button_setAndar_2_Click(object sender, RoutedEventArgs e)
        {
            if (this.idBotoesExt[1] > 0)
            {
                this.Legenda.Content = $"{this.idBotoesExt[1]}° Andar";
                this.andarAtual = this.idBotoesExt[1];
            }
        }

        /// <summary>
        /// Acessar painel externo do andar: 3° andar
        /// </summary>
        private void button_setAndar_3_Click(object sender, RoutedEventArgs e)
        {
            if (this.idBotoesExt[2] > 0)
            {
                this.Legenda.Content = $"{this.idBotoesExt[2]}° Andar";
                this.andarAtual = this.idBotoesExt[2];
            }
        }

        /// <summary>
        /// Acessar painel externo do andar: 4° andar
        /// </summary>
        private void button_setAndar_4_Click(object sender, RoutedEventArgs e)
        {
            if (this.idBotoesExt[3] > 0)
            {
                this.Legenda.Content = $"{this.idBotoesExt[3]}° Andar";
                this.andarAtual = this.idBotoesExt[3];
            }
        }

        /// <summary>
        /// Acessar painel externo do andar: 5° andar
        /// </summary>
        private void button_setAndar_5_Click(object sender, RoutedEventArgs e)
        {
            if (this.idBotoesExt[4] > 0)
            {
                this.Legenda.Content = $"{this.idBotoesExt[4]}° Andar";
                this.andarAtual = this.idBotoesExt[4];
            }
        }

        /// <summary>
        /// Acessar painel externo do andar: 6° andar
        /// </summary>
        private void button_setAndar_6_Click(object sender, RoutedEventArgs e)
        {
            if (this.idBotoesExt[5] > 0)
            {
                this.Legenda.Content = $"{this.idBotoesExt[5]}° Andar";
                this.andarAtual = this.idBotoesExt[5];
            }
        }

        private void upGrupoExt_Click(object sender, RoutedEventArgs e)
        {
            this.Predio.avancarGrupo(ref this.encoderExt);
        }

        private void downGrupoExt_Click(object sender, RoutedEventArgs e)
        {
            this.Predio.voltarGrupo(ref this.encoderExt);
        }
        #endregion


        #region visuais
        /// <summary>
        /// Atualizar cor dos botões externos de seleção dos andares
        /// </summary>
        private void AtualizarBotoesExternos(int andar)
        {
            this.button_setAndar_a.Background = this.Predio.andares[this.idBotoesExt[0]].hasPendency() ? colorButtonOnpending : colorButtonOff;
            this.button_setAndar_b.Background = this.Predio.andares[this.idBotoesExt[1]].hasPendency() ? colorButtonOnpending : colorButtonOff;
            this.button_setAndar_c.Background = this.Predio.andares[this.idBotoesExt[2]].hasPendency() ? colorButtonOnpending : colorButtonOff;
            this.button_setAndar_d.Background = this.Predio.andares[this.idBotoesExt[3]].hasPendency() ? colorButtonOnpending : colorButtonOff;
            this.button_setAndar_e.Background = this.Predio.andares[this.idBotoesExt[4]].hasPendency() ? colorButtonOnpending : colorButtonOff;
            this.button_setAndar_f.Background = this.Predio.andares[this.idBotoesExt[5]].hasPendency() ? colorButtonOnpending : colorButtonOff;
            this.button_setAndar_t.Background = this.Predio.andares[0].hasPendency() ? colorButtonOnpending : colorButtonOff;

            int idBotao = (andar > 0) ? (((andar - 1) % nBotoesPainelInterno)) : 0;
            if (this.idBotoesExt[idBotao] == andar || (idBotao == 0 && andar < nBotoesPainelInterno))
            {
                if (!(idBotao == 0 && andar < nBotoesPainelInterno) || andar == 1)
                {
                    idBotao++;
                }
                var cor = idBotao switch
                {
                    0 => this.button_setAndar_t.Background = colorButtonOn,
                    1 => this.button_setAndar_a.Background = colorButtonOn,
                    2 => this.button_setAndar_b.Background = colorButtonOn,
                    3 => this.button_setAndar_c.Background = colorButtonOn,
                    4 => this.button_setAndar_d.Background = colorButtonOn,
                    5 => this.button_setAndar_e.Background = colorButtonOn,
                    6 => this.button_setAndar_f.Background = colorButtonOn,
                    _ => throw new NotImplementedException()
                };
            }

            this.button_setAndar_a.Content = this.idBotoesExt[0] > 0 ? $"{this.idBotoesExt[0]}° Andar" : "";
            this.button_setAndar_b.Content = this.idBotoesExt[1] > 0 ? $"{this.idBotoesExt[1]}° Andar" : "";
            this.button_setAndar_c.Content = this.idBotoesExt[2] > 0 ? $"{this.idBotoesExt[2]}° Andar" : "";
            this.button_setAndar_d.Content = this.idBotoesExt[3] > 0 ? $"{this.idBotoesExt[3]}° Andar" : "";
            this.button_setAndar_e.Content = this.idBotoesExt[4] > 0 ? $"{this.idBotoesExt[4]}° Andar" : "";
            this.button_setAndar_f.Content = this.idBotoesExt[5] > 0 ? $"{this.idBotoesExt[5]}° Andar" : "";

            this.button_setAndar_a.Visibility = this.idBotoesExt[0] > 0 ? Visibility.Visible : Visibility.Hidden;
            this.button_setAndar_b.Visibility = this.idBotoesExt[1] > 0 ? Visibility.Visible : Visibility.Hidden;
            this.button_setAndar_c.Visibility = this.idBotoesExt[2] > 0 ? Visibility.Visible : Visibility.Hidden;
            this.button_setAndar_d.Visibility = this.idBotoesExt[3] > 0 ? Visibility.Visible : Visibility.Hidden;
            this.button_setAndar_e.Visibility = this.idBotoesExt[4] > 0 ? Visibility.Visible : Visibility.Hidden;
            this.button_setAndar_f.Visibility = this.idBotoesExt[5] > 0 ? Visibility.Visible : Visibility.Hidden;

            this.led_emergencia.Background = this.emergencia ? colorLedEmergenciaOn : colorLedEmergenciaOff;
        }
        
        /// <summary>
        /// Atualizar cor dos botões internos de seleção dos andares de desenbarque
        /// </summary>
        private void AtualizarBotoesInternos()
        {
            this.botao_t_interno.Background = this.Predio.andares[0].needDesembarcar() ? colorButtonOn : colorButtonOff;
            this.botao_a_interno.Background = this.Predio.andares[this.idBotoesInt[0]].needDesembarcar() ? colorButtonOn : colorButtonOff;
            this.botao_b_interno.Background = this.Predio.andares[this.idBotoesInt[1]].needDesembarcar() ? colorButtonOn : colorButtonOff;
            this.botao_c_interno.Background = this.Predio.andares[this.idBotoesInt[2]].needDesembarcar() ? colorButtonOn : colorButtonOff;
            this.botao_d_interno.Background = this.Predio.andares[this.idBotoesInt[3]].needDesembarcar() ? colorButtonOn : colorButtonOff;
            this.botao_e_interno.Background = this.Predio.andares[this.idBotoesInt[4]].needDesembarcar() ? colorButtonOn : colorButtonOff;
            this.botao_f_interno.Background = this.Predio.andares[this.idBotoesInt[5]].needDesembarcar() ? colorButtonOn : colorButtonOff;
            this.botao_em_interno.Background = this.emergencia ? colorLedEmergenciaOn : colorLedEmergenciaOff;

            this.botao_a_interno.Content = this.idBotoesInt[0] > 0 ? this.idBotoesInt[0].ToString() : "";
            this.botao_b_interno.Content = this.idBotoesInt[1] > 0 ? this.idBotoesInt[1].ToString() : "";
            this.botao_c_interno.Content = this.idBotoesInt[2] > 0 ? this.idBotoesInt[2].ToString() : "";
            this.botao_d_interno.Content = this.idBotoesInt[3] > 0 ? this.idBotoesInt[3].ToString() : "";
            this.botao_e_interno.Content = this.idBotoesInt[4] > 0 ? this.idBotoesInt[4].ToString() : "";
            this.botao_f_interno.Content = this.idBotoesInt[5] > 0 ? this.idBotoesInt[5].ToString() : "";

            this.botao_a_interno.Visibility = this.idBotoesInt[0] > 0 ? Visibility.Visible : Visibility.Hidden;
            this.botao_b_interno.Visibility = this.idBotoesInt[1] > 0 ? Visibility.Visible : Visibility.Hidden;
            this.botao_c_interno.Visibility = this.idBotoesInt[2] > 0 ? Visibility.Visible : Visibility.Hidden;
            this.botao_d_interno.Visibility = this.idBotoesInt[3] > 0 ? Visibility.Visible : Visibility.Hidden;
            this.botao_e_interno.Visibility = this.idBotoesInt[4] > 0 ? Visibility.Visible : Visibility.Hidden;
            this.botao_f_interno.Visibility = this.idBotoesInt[5] > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// Método que imprime no display do painel o digito informado por parâmetro
        /// </summary>
        /// <param name="digito"></param>
        private void DisplayAndar(byte digito)
        {
            bool[] ledDisplay = this.GetSinaisDigito(digito,1);
            led_a.Background = ledDisplay[0] ? colorDisplayOn : colorDisplayOff;
            led_b.Background = ledDisplay[1] ? colorDisplayOn : colorDisplayOff;
            led_c.Background = ledDisplay[2] ? colorDisplayOn : colorDisplayOff;
            led_d.Background = ledDisplay[3] ? colorDisplayOn : colorDisplayOff;
            led_e.Background = ledDisplay[4] ? colorDisplayOn : colorDisplayOff;
            led_f.Background = ledDisplay[5] ? colorDisplayOn : colorDisplayOff;
            led_g.Background = ledDisplay[6] ? colorDisplayOn : colorDisplayOff;
            
            ledDisplay = this.GetSinaisDigito(digito,2);
            led_10a.Background = ledDisplay[0] ? colorDisplayOn : colorDisplayOff;
            led_10b.Background = ledDisplay[1] ? colorDisplayOn : colorDisplayOff;
            led_10c.Background = ledDisplay[2] ? colorDisplayOn : colorDisplayOff;
            led_10d.Background = ledDisplay[3] ? colorDisplayOn : colorDisplayOff;
            led_10e.Background = ledDisplay[4] ? colorDisplayOn : colorDisplayOff;
            led_10f.Background = ledDisplay[5] ? colorDisplayOn : colorDisplayOff;
            led_10g.Background = ledDisplay[6] ? colorDisplayOn : colorDisplayOff;
        }

        private bool[] GetSinaisDigito(byte digito, byte ordem = 1)
        {
            for (int i = 1; i <= ordem; i++)
            {
                digito = (byte)(i < ordem ? digito / 10 : digito % 10);
            }

            return digito switch
            {
                //                  A      B      C      D      R      F      G
                0 => new bool[] { true, true, true, true, true, true, false },
                1 => new bool[] { false, true, true, false, false, false, false },
                2 => new bool[] { true, true, false, true, true, false, true },
                3 => new bool[] { true, true, true, true, false, false, true },
                4 => new bool[] { false, true, true, false, false, true, true },
                5 => new bool[] { true, false, true, true, false, true, true },
                6 => new bool[] { true, false, true, true, true, true, true },
                7 => new bool[] { true, true, true, false, false, false, false },
                8 => new bool[] { true, true, true, true, true, true, true },
                9 => new bool[] { true, true, true, true, false, true, true },
                _ => new bool[] { true, true, true, true, true, true, false },
            };
        }

        private void MoverPortas(object source, EventArgs e)
        {
            if (!this.moverPortas && encoderPotas == 0) 
                return;

            if (encoderPotas < 30)
            {
                this.encoderPotas++;
            }
            else if (!this.posicaoPortas && this.encoderPotas < 60)
            {
                this.porta_esquerda.Width -= 1;
                this.porta_direita.Width += 1;
                this.encoderPotas++;
                if (!this.posicaoPortas && this.encoderPotas == 60)
                {
                    this.posicaoPortas = true;
                    this.encoderPotas++;
                }
            }
            else if(encoderPotas > 60 && encoderPotas < 90)
            {
                if(!this.emergencia)
                    this.encoderPotas++;
            }
            else if (this.posicaoPortas && this.encoderPotas < 120)
            {
                this.porta_esquerda.Width += 1;
                this.porta_direita.Width -= 1;
                this.encoderPotas++;
                this.posicaoPortas = !(this.posicaoPortas && this.encoderPotas == 120);
            }    
            if (!this.posicaoPortas && encoderPotas == 120)
            {
                this.Elevador.LiberarPortas();
                this.moverPortas = false;
                this.encoderPotas = 0;
            }
        }
        #endregion    


        /// <summary>
        /// Método responsável pela atualização de todas as flags e os sistemas dependentes das mesmas
        /// </summary>
        private void CheckFlags(object source, EventArgs e)
        {
            bool value;

            // Desativar botão de subir no último andar
            value = !(this.andarAtual == this.Elevador.GetQtdAndar() - 1);
            this.botao_subir.Background = value ? Brushes.Transparent : Brushes.White;
            this.botao_subir.IsEnabled = value;

            // Desativar botão de descer no primeiro andar
            value = !(this.andarAtual == 0);
            this.botao_descer.Background = value ? Brushes.Transparent : Brushes.White;
            this.botao_descer.IsEnabled = value;

            // Atualizar atributos visuais dos botões
            this.AtualizarBotoesExternos(this.andarAtual);
            this.AtualizarBotoesInternos();

            // Atualizar status das leds dos botões de subir e descer do painel externo
            this.led_botao_subir.Background = this.Predio.andares[this.andarAtual].needSubir() ? colorLedOn : colorLedOff;
            this.led_botao_descer.Background = this.Predio.andares[this.andarAtual].needDescer() ? colorLedOn : colorLedOff;

            // Acionar led indicativo para o modo emergencial
            if (this.emergencia)
            {
                this.led_botao_subir.Background = colorLedOff;
                this.led_botao_descer.Background = colorLedOff;
            }

            // Atualizar displays e a label de status
            this.DisplayAndar((byte)this.Elevador.GetAndarAtual());
            this.label_status.Content = this.Elevador.GetStatus().ToString();

            // Controle de animação das portas
            this.moverPortas = (this.Elevador.CanMoverPortas() || this.emergencia);

            // Variáveis de controle para acessar botões ocultos
            this.idBotoesInt = this.Predio.getGrupoBotoes(ref this.encoderInt);
            this.idBotoesExt= this.Predio.getGrupoBotoes(ref this.encoderExt);
        }

        /// <summary>
        /// Verifica se o sistema esta travado: status de emergencia ativo ou porta aberta
        /// </summary>
        private bool isSystemLock()
        {
            return this.emergencia && this.moverPortas;
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
    }
}
