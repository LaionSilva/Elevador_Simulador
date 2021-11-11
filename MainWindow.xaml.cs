using System;
using System.Collections.Generic;
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
        private bool subir = false;

        private byte[] digitos;

        public MainWindow()
        {
            InitializeComponent();
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

        private void DisplayAndar(byte digito)
        {
        }
    }
}
