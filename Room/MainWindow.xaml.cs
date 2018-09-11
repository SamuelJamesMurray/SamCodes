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

        PointCollection points = new PointCollection();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        /// <summary>
        /// Loads the Draweverything function when everything else is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DrawEverything();
        }

        /// <summary>
        /// Clears the grid and redraws it
        /// </summary>
        public void DrawEverything()
        {
            myCanvas.Children.Clear();
            DrawGrid();
            DrawLines();

        }

        /// <summary>
        /// Draws lines amd calculates values for polygon
        /// </summary>
        private void DrawLines()
        {
            Polyline polyline = new Polyline();
            polyline.Stroke = Brushes.Black;
            polyline.StrokeThickness = 2;
            polyline.Points = points;
            myCanvas.Children.Add(polyline);

            PointCollection temp = new PointCollection();
            foreach (Point p in points)
            {
                temp.Add (p);
            }
            double area = AreaOfPolygon(temp)/10000;
            Area.Text = area.ToString("0.##");
            if (double.TryParse(Height.Text, out double H) && double.TryParse(Coats.Text, out double C))
            {
                double distance = 0;
                if (temp.Count > 1)
                {
                    for (int i = 0; i < temp.Count - 1; i++)
                    {
                        distance += GetDistance(temp[i], temp[i + 1]) / 100.0*H;
                    }
                }
                Paint.Text = distance.ToString("0.##");
                Volume.Text = (area * H).ToString("0.##");
                Litres.Text = (distance / 10.0 * C).ToString("0.##");
            }
        }
        /// <summary>
        /// Work out distance between two points
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        private static double GetDistance(Point point1, Point point2)
        {
            //pythagorean theorem c^2 = a^2 + b^2
            //thus c = square root(a^2 + b^2)
            double a = (double)(point2.X - point1.X);
            double b = (double)(point2.Y - point1.Y);

            return Math.Sqrt(a * a + b * b);
        }

        /// <summary>
        /// Calculate the area of a polygon
        /// https://stackoverflow.com/questions/2034540/calculating-area-of-irregular-polygon-in-c-sharp
        /// </summary>
        private double AreaOfPolygon(PointCollection points)
        {
            if (points.Count() < 2) return 0.0;

            points.Add(points[0]);
            var area = Math.Abs(points.Take(points.Count - 1)
               .Select((p, i) => (points[i + 1].X - p.X) * (points[i + 1].Y + p.Y))
               .Sum() / 2);

            return area;
        }


        int gridSpacing = 100;

        /// <summary>
        /// Draws grid onto myCanvas
        /// </summary>
        private void DrawGrid()
        {
            // from left to right
            for (int x = 0; x < myCanvas.ActualWidth; x += gridSpacing)
            {
                Line line = new Line();
                line.Stroke = Brushes.LightGray;
                line.StrokeThickness = 2;
                line.X1 = x;
                line.Y1 = 0;
                line.X2 = x;
                line.Y2 = myCanvas.ActualHeight;
                myCanvas.Children.Add(line);
            }

            // from bottom to top
            for (int x = 0; x < myCanvas.ActualHeight; x += gridSpacing)
            {
                Line line = new Line();
                line.Stroke = Brushes.LightGray;
                line.StrokeThickness = 2;
                line.X1 = 0;
                line.Y1 = x;
                line.X2 = myCanvas.ActualWidth;
                line.Y2 = x;
                myCanvas.Children.Add(line);
            }
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
                Area.Text = area.ToString("0.##");
                double volume = area * H;
                Volume.Text = volume.ToString("0.##");
                double paint = 2 * H * L + 2 * H * W;
                Paint.Text = paint.ToString("0.##");
                //I got a value of 1 litre of paint per 10m^2 from the B&Q website
                double litres =  paint / 10.0 *  C;
                Litres.Text = litres.ToString("0.##");
            }
            //An error message if people don't enter a number in each box
            else
            {
                MessageBox.Show("Enter a number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Closes the Application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Loads Draweverything when screen size is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawEverything();
        }

        /// <summary>
        /// Adds point where you click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point p = Mouse.GetPosition(myCanvas);
            points.Add(p);
            DrawEverything();
        }

        /// <summary>
        /// Deletes all points and relodes DrawEverything
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear(object sender, RoutedEventArgs e)
        {
            points.Clear();
            DrawEverything();
        }

        /// <summary>
        /// Deletes last point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Undo(object sender, RoutedEventArgs e)
        {
            if (points.Count > 0)
            {
                points.RemoveAt(points.Count - 1);
                DrawEverything();
            }
        }
    }
}
