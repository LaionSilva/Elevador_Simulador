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

namespace Elevador_Simulador
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private int andarAtual;

        private bool[] LEDS;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            LEDS = new bool[] { };

            DisplayAndar(3);
        }


        #region botões_internos
        private void button_t_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_4_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_em_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion


        #region ecolher_andar_externo
        private void button_setAndar_t_Click(object sender, RoutedEventArgs e)
        {
            this.label_status.Content = "Térreo";
        }

        private void button_setAndar_1_Click(object sender, RoutedEventArgs e)
        {
            this.label_status.Content = "1° Andar";
        }

        private void button_setAndar_2_Click(object sender, RoutedEventArgs e)
        {
            this.label_status.Content = "2° Andar";
        }

        private void button_setAndar_3_Click(object sender, RoutedEventArgs e)
        {
            this.label_status.Content = "3° Andar";
        }

        private void button_setAndar_4_Click(object sender, RoutedEventArgs e)
        {
            this.label_status.Content = "4° Andar";
        }
        #endregion


        private void DisplayAndar(int digito)
        {
            LEDS = (digito % 10) switch
            {
                //                  A     B     C     D     R     F     G
                0 => new bool[] { true, true, true, true, true, true, false },
                1 => new bool[] { false, true, true, false, false, false, false },
                2 => new bool[] { true, true, false, true, true, false, false },
                3 => new bool[] { true, true, true, true, false, false, false },
                4 => new bool[] { false, true, true, false, false, true, true },
                5 => new bool[] { true, false, true, true, false, true, true },
                6 => new bool[] { true, false, true, true, true, true, true },
                7 => new bool[] { true, true, true, false, false, false, false },
                8 => new bool[] { true, true, true, true, true, true, true },
                9 => new bool[] { true, true, true, true, false, true, true },
                _ => new bool[] { true, true, true, true, true, true, false },
            };
            
            led_a.Background = LEDS[0] ? Brushes.Red : Brushes.Black;
            led_b.Background = LEDS[1] ? Brushes.Red : Brushes.Black;
            led_c.Background = LEDS[2] ? Brushes.Red : Brushes.Black;
            led_d.Background = LEDS[3] ? Brushes.Red : Brushes.Black;
            led_e.Background = LEDS[4] ? Brushes.Red : Brushes.Black;
            led_f.Background = LEDS[5] ? Brushes.Red : Brushes.Black;
            led_g.Background = LEDS[6] ? Brushes.Red : Brushes.Black;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
        }
    }
}
