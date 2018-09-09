using System;
using System.Collections.Generic;
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

namespace Room
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// It takes the dimensions of the room and calculates the area, volume and amount of paint needed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalculateValues(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Width.Text, out double W) && double.TryParse(Height.Text, out double H) && double.TryParse(Length.Text, out double L)&& double.TryParse(Coats.Text, out double C))
            {
                double area = W * L;
                Area.Text = area.ToString();
                double volume = area * H;
                Volume.Text = volume.ToString();
                double paint = 2 * H * L + 2 * H * W;
                Paint.Text = paint.ToString();
                //I got a value of 1 litre of paint per 10m^2 from the B&Q website
                double litres =  paint / 10.0 *  C;
                Litres.Text = litres.ToString();
            }
            //An error message if people don't enter a number in each box
            else
            {
                MessageBox.Show("Enter a number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
