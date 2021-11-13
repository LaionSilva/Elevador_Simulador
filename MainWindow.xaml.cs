using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
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

        private int andarAtual;

        private bool[] ledDisplay;

        // Flags
        private bool descer;
        private bool subir;
        private bool modoManual;
        private bool emergencia;

        private Elevador elevador;
        private Andar[] andares;

        private static DispatcherTimer timer0;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            this.ledDisplay = new bool[] { };
            this.andarAtual = 0;

            this.descer = false;
            this.subir = false;
            this.modoManual = false;
            this.emergencia = false;

            this.elevador = new Elevador();
            this.andares = new Andar[5]
            {
                new Andar(0),
                new Andar(1),
                new Andar(2),
                new Andar(3),
                new Andar(4)
            };


            timer0 = new DispatcherTimer();
            timer0.Interval = TimeSpan.FromMilliseconds(1000);
            timer0.Tick += executarElevador;
            timer0.Tick += CheckFlags;
            timer0.Start();

            this.CheckFlags();
        }


        #region botões_internos
        private void button_t_Click(object sender, RoutedEventArgs e)
        {
            this.elevador.ClickBotao(0);
        }

        private void button_1_Click(object sender, RoutedEventArgs e)
        {
            this.elevador.ClickBotao(1);
        }

        private void button_2_Click(object sender, RoutedEventArgs e)
        {
            this.elevador.ClickBotao(2);
        }

        private void button_3_Click(object sender, RoutedEventArgs e)
        {
            this.elevador.ClickBotao(3);
        }

        private void button_4_Click(object sender, RoutedEventArgs e)
        {
            this.elevador.ClickBotao(4);
        }

        private void button_em_Click(object sender, RoutedEventArgs e)
        {
            this.emergencia = true;
            this.CheckFlags();
        }
        #endregion


        #region botões_externos
        private void button_setAndar_t_Click(object sender, RoutedEventArgs e)
        {
            this.Legenda.Content = "Térreo";
            
            this.andarAtual = 0;
            this.CheckFlags();
        }

        private void button_setAndar_1_Click(object sender, RoutedEventArgs e)
        {
            this.Legenda.Content = "1° Andar";
            this.andarAtual = 1;
            this.CheckFlags();
        }

        private void button_setAndar_2_Click(object sender, RoutedEventArgs e)
        {
            this.Legenda.Content = "2° Andar";
            this.andarAtual = 2;
            this.CheckFlags();
        }

        private void button_setAndar_3_Click(object sender, RoutedEventArgs e)
        {
            this.Legenda.Content = "3° Andar";
            this.andarAtual = 3;
            this.CheckFlags();
        }

        private void button_setAndar_4_Click(object sender, RoutedEventArgs e)
        {
            this.Legenda.Content = "4° Andar";
            this.andarAtual = 4;
            this.CheckFlags();
        }
        #endregion


        #region botões_interface_painel_externo
        private void botao_subir_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.subir && !this.emergencia)
            {
                this.andares[this.andarAtual].Subir();
                //this.elevador.NovoDestino(this.andarAtual);
                this.CheckFlags();
            }
        }

        private void botao_descer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.descer && !this.emergencia)
            {
                this.andares[this.andarAtual].Descer();
                //this.elevador.NovoDestino(this.andarAtual);
                this.CheckFlags();
            }
        }

        private void checkBox_manual_Checked(object sender, RoutedEventArgs e)
        {
            this.modoManual = true;
            this.checkBox_automatico.IsChecked = false;
        }

        private void checkBox_automatico_Checked(object sender, RoutedEventArgs e)
        {
            this.modoManual = false;
            this.checkBox_manual.IsChecked = false;
        }
        #endregion

        private void executarElevador(object sender, EventArgs e)
        {
            this.elevador.Movimento(this.andares);
            
        }
        private void AtualizarBotoesExternos(int andar)
        {
            this.button_setAndar_1.Background = Brushes.DarkGray;
            this.button_setAndar_2.Background = Brushes.DarkGray;
            this.button_setAndar_3.Background = Brushes.DarkGray;
            this.button_setAndar_4.Background = Brushes.DarkGray;
            this.button_setAndar_t.Background = Brushes.DarkGray;
            var cor = andar switch
            {
                0 => this.button_setAndar_t.Background = Brushes.OrangeRed,
                1 => this.button_setAndar_1.Background = Brushes.OrangeRed,
                2 => this.button_setAndar_2.Background = Brushes.OrangeRed,
                3 => this.button_setAndar_3.Background = Brushes.OrangeRed,
                4 => this.button_setAndar_4.Background = Brushes.OrangeRed,
                _ => throw new NotImplementedException()
            };
        }

        private void DisplayAndar(int digito)
        {
            ledDisplay = (digito % 10) switch
            {
                //                  A     B     C     D     R     F     G
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
            
            led_a.Background = ledDisplay[0] ? Brushes.Red : Brushes.Black;
            led_b.Background = ledDisplay[1] ? Brushes.Red : Brushes.Black;
            led_c.Background = ledDisplay[2] ? Brushes.Red : Brushes.Black;
            led_d.Background = ledDisplay[3] ? Brushes.Red : Brushes.Black;
            led_e.Background = ledDisplay[4] ? Brushes.Red : Brushes.Black;
            led_f.Background = ledDisplay[5] ? Brushes.Red : Brushes.Black;
            led_g.Background = ledDisplay[6] ? Brushes.Red : Brushes.Black;
        }

        private void CheckFlags(object sender, EventArgs e)
        {
            this.CheckFlags();
        }
        private void CheckFlags()
        {
            bool value;

            value = !(this.andarAtual == 4);
            this.botao_subir.Background = value ? Brushes.Transparent : Brushes.White;
            this.botao_subir.IsEnabled = value;

            value = !(this.andarAtual == 0);
            this.botao_descer.Background = value ? Brushes.Transparent : Brushes.White;
            this.botao_descer.IsEnabled = value;

            this.AtualizarBotoesExternos(this.andarAtual);

            this.led_botao_subir.Background = this.andares[this.andarAtual].needSubir() ? Brushes.OrangeRed : Brushes.Gray;
            this.led_botao_descer.Background = this.andares[this.andarAtual].needDescer() ? Brushes.OrangeRed : Brushes.Gray;

            if (this.emergencia)
            {
                value = this.emergencia;
                this.led_emergencia.Background = value ? Brushes.Red : Brushes.DarkGray;
                this.button_em.Background = value ? Brushes.Red : Brushes.DarkGray;

                this.subir = false;
                this.descer = false;

                this.led_botao_subir.Background = Brushes.Gray;
                this.led_botao_descer.Background = Brushes.Gray;
            }
            this.DisplayAndar(this.elevador.GetAndarAtual());
            this.label_status.Content = this.elevador.GetStatus().ToString();
        }

        private string GerarLog(int nAndarOrigem, int nAndarDestino)
        {
            string origem = nAndarOrigem != 0 ? $"{nAndarOrigem}° Andar" : "Térreo";
            string destino = nAndarDestino != 0 ? $"{nAndarDestino}° Andar" : "Térreo";

            return $"{DateTime.Now.ToString()} | {origem} => {destino}";
        }
    }
}
