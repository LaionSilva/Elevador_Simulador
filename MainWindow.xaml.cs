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
        private Elevador elevador;
        private Predio Predio;


        private SolidColorBrush colorButtonOff = Brushes.AntiqueWhite;
        private SolidColorBrush colorButtonOn = Brushes.DarkOrange;
        private SolidColorBrush colorLedOff = Brushes.AntiqueWhite;
        private SolidColorBrush colorLedOn = Brushes.DarkOrange;
        private SolidColorBrush colorLedEmergenciaOff = Brushes.AntiqueWhite;
        private SolidColorBrush colorLedEmergenciaOn = Brushes.Red;
        private SolidColorBrush colorDisplayOff = Brushes.Black;
        private SolidColorBrush colorDisplayOn = Brushes.OrangeRed;


        #region construtores
        public MainWindow()
        {
            InitializeComponent();

            this.andarAtual = 0;

            // Flags
            this.modoManual = false;
            this.emergencia = false;
            this.moverPortas = false;
            this.posicaoPortas = false;
            this.encoderPotas = 0;

            // Inicialização das classes do elevador
            this.Predio = new Predio(
                new Andar[5]
                {
                    new Andar(0),
                    new Andar(1),
                    new Andar(2),
                    new Andar(3),
                    new Andar(4)
                }
            );
            this.elevador = new Elevador(this.Predio);

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


        #region botões_internos
        private void button_t_Click(object sender, RoutedEventArgs e)
        {
            if(!this.isSystemLock())
                this.elevador.EventClickButton(0);
        }

        private void button_1_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isSystemLock())
                this.elevador.EventClickButton(1);
        }

        private void button_2_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isSystemLock())
                this.elevador.EventClickButton(2);
        }

        private void button_3_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isSystemLock())
                this.elevador.EventClickButton(3);
        }

        private void button_4_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isSystemLock())
                this.elevador.EventClickButton(4);
        }

        private void button_em_Click(object sender, RoutedEventArgs e)
        {
            this.emergencia = !this.emergencia;

            if (this.emergencia)
            {
                this.elevador.Emergencia();
                foreach (Andar a in this.Predio.andares)
                {
                    a.Desembarcou();
                    a.Subiu();
                    a.Desceu();
                }
            }
        }
        #endregion


        #region botões_externos
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
            this.Legenda.Content = "1° Andar";
            this.andarAtual = 1;
        }

        /// <summary>
        /// Acessar painel externo do andar: 2° andar
        /// </summary>
        private void button_setAndar_2_Click(object sender, RoutedEventArgs e)
        {
            this.Legenda.Content = "2° Andar";
            this.andarAtual = 2;
        }

        /// <summary>
        /// Acessar painel externo do andar: 3° andar
        /// </summary>
        private void button_setAndar_3_Click(object sender, RoutedEventArgs e)
        {
            this.Legenda.Content = "3° Andar";
            this.andarAtual = 3;
        }

        /// <summary>
        /// Acessar painel externo do andar: 4° andar
        /// </summary>
        private void button_setAndar_4_Click(object sender, RoutedEventArgs e)
        {
            this.Legenda.Content = "4° Andar";
            this.andarAtual = 4;
        }
        #endregion


        #region botões_interface_painel_externo
        /// <summary>
        /// Aessar elevador para subir
        /// </summary>
        private void botao_subir_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.Predio.andares[this.andarAtual].needSubir() && !this.isSystemLock())
            {
                this.Predio.andares[this.andarAtual].Subir();
            }
        }

        /// <summary>
        /// Aessar elevador para descer
        /// </summary>
        private void botao_descer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.Predio.andares[this.andarAtual].needDescer() && !this.isSystemLock())
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
        }

        /// <summary>
        /// Definir sistema para o modo automático
        /// </summary>
        private void checkBox_automatico_Checked(object sender, RoutedEventArgs e)
        {
            this.modoManual = false;
            this.checkBox_manual.IsChecked = false;
        }
        #endregion


        #region visuais
        /// <summary>
        /// Atualizar cor dos botões externos de seleção dos andares
        /// </summary>
        private void AtualizarBotoesExternos(int andar)
        {
            this.button_setAndar_1.Background = colorButtonOff;
            this.button_setAndar_2.Background = colorButtonOff;
            this.button_setAndar_3.Background = colorButtonOff;
            this.button_setAndar_4.Background = colorButtonOff;
            this.button_setAndar_t.Background = colorButtonOff;
            var cor = andar switch
            {
                0 => this.button_setAndar_t.Background = colorButtonOn,
                1 => this.button_setAndar_1.Background = colorButtonOn,
                2 => this.button_setAndar_2.Background = colorButtonOn,
                3 => this.button_setAndar_3.Background = colorButtonOn,
                4 => this.button_setAndar_4.Background = colorButtonOn,
                _ => throw new NotImplementedException()
            };
            this.led_emergencia.Background = this.emergencia ? colorLedEmergenciaOn : colorLedEmergenciaOff;
        }
        
        /// <summary>
        /// Atualizar cor dos botões internos de seleção dos andares de desenbarque
        /// </summary>
        private void AtualizarBotoesInternos()
        {
            this.botao_t_interno.Background = this.Predio.andares[0].needDesembarcar() ? colorButtonOn : colorButtonOff;
            this.botao_1_interno.Background = this.Predio.andares[1].needDesembarcar() ? colorButtonOn : colorButtonOff;
            this.botao_2_interno.Background = this.Predio.andares[2].needDesembarcar() ? colorButtonOn : colorButtonOff;
            this.botao_3_interno.Background = this.Predio.andares[3].needDesembarcar() ? colorButtonOn : colorButtonOff;
            this.botao_4_interno.Background = this.Predio.andares[4].needDesembarcar() ? colorButtonOn : colorButtonOff;
            this.botao_em_interno.Background = this.emergencia ? colorLedEmergenciaOn : colorLedEmergenciaOff;
        }

        /// <summary>
        /// Método que imprime no display do painel o digito informado por parâmetro
        /// </summary>
        /// <param name="digito"></param>
        private void DisplayAndar(byte digito)
        {
            bool[] ledDisplay = (digito % 10) switch
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

            led_a.Background = ledDisplay[0] ? colorDisplayOn : colorDisplayOff;
            led_b.Background = ledDisplay[1] ? colorDisplayOn : colorDisplayOff;
            led_c.Background = ledDisplay[2] ? colorDisplayOn : colorDisplayOff;
            led_d.Background = ledDisplay[3] ? colorDisplayOn : colorDisplayOff;
            led_e.Background = ledDisplay[4] ? colorDisplayOn : colorDisplayOff;
            led_f.Background = ledDisplay[5] ? colorDisplayOn : colorDisplayOff;
            led_g.Background = ledDisplay[6] ? colorDisplayOn : colorDisplayOff;
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
                this.elevador.LiberarPortas();
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
            value = !(this.andarAtual == this.elevador.GetQtdAndar() - 1);
            this.botao_subir.Background = value ? Brushes.Transparent : Brushes.White;
            this.botao_subir.IsEnabled = value;

            // Desativar botão de descer no primeiro andar
            value = !(this.andarAtual == 0);
            this.botao_descer.Background = value ? Brushes.Transparent : Brushes.White;
            this.botao_descer.IsEnabled = value;

            this.AtualizarBotoesExternos(this.andarAtual);
            this.AtualizarBotoesInternos();

            // Atualizar status das leds dos botões de subir e descer do painel externo
            this.led_botao_subir.Background = this.Predio.andares[this.andarAtual].needSubir() ? colorLedOn : colorLedOff;
            this.led_botao_descer.Background = this.Predio.andares[this.andarAtual].needDescer() ? colorLedOn : colorLedOff;

            // Atualizar status das leds dos botões de subir e descer do painel externo


            // Acionar led indicativa do modo emergencial
            if (this.emergencia)
            {
                this.led_botao_subir.Background = colorLedOff;
                this.led_botao_descer.Background = colorLedOff;
            }

            this.DisplayAndar((byte)this.elevador.GetAndarAtual());
            this.label_status.Content = this.elevador.GetStatus().ToString();

            this.moverPortas = (this.elevador.CanMoverPortas() || this.emergencia);
        }

        private bool isSystemLock()
        {
            return this.emergencia && this.moverPortas;
        }

        private string GerarLog(int nAndarOrigem, int nAndarDestino)
        {
            string origem = nAndarOrigem != 0 ? $"{nAndarOrigem}° Andar" : "Térreo";
            string destino = nAndarDestino != 0 ? $"{nAndarDestino}° Andar" : "Térreo";

            return $"{DateTime.Now.ToString()} | {origem} => {destino}";
        }
    }
}
