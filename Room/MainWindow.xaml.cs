using Microsoft.Win32;
using System;
using System.IO;
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
using Newtonsoft.Json;

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
            double StartCircleDiameter = 10.0;


            if (points.Count() > 0)
            {
                // make a circle to draw
                Ellipse el = new Ellipse();
                el.Width = StartCircleDiameter;
                el.Height = StartCircleDiameter;

                // set the colour
                el.Fill = Brushes.Green;
                el.Stroke = Brushes.Blue;
                el.StrokeThickness = 1;

                // work out where to put it, only was is using the margin to place it
                double left = points[0].X - (StartCircleDiameter / 2);
                double top = points[0].Y - (StartCircleDiameter / 2);
                el.Margin = new Thickness(left, top, 0, 0);

                // draw it
                myCanvas.Children.Add(el);
            }
            Polyline polyline = new Polyline();
            polyline.Stroke = Brushes.Black;
            polyline.StrokeThickness = 2;
            polyline.Points = points;
            myCanvas.Children.Add(polyline);

            CalculateGridValues();
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
            if (double.TryParse(Width.Text, out double W) && double.TryParse(Height.Text, out double H) && double.TryParse(Length.Text, out double L)&& double.TryParse(Coats.Text, out double C) && double.TryParse(AntiArea.Text, out double A))
            {
                double area = W * L;
                double volume = area * H;
                double paint = 2 * H * L + 2 * H * W;
                //I got a value of 1 litre of paint per 10m^2 from the B&Q website
                double litres = (paint - A) / 10.0 * C;
                if (litres < 0.0)
                {
                    MessageBox.Show("You have more doors and windows than wall to put them in", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    
                    Area.Text = area.ToString("0.##");
                    Volume.Text = volume.ToString("0.##");
                    Paint.Text = paint.ToString("0.##");
                    Litres.Text = litres.ToString("0.##");
                }
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

        private void Save(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "room file|*.room";
            saveFileDialog1.FileName = "firstroom.room";
            saveFileDialog1.Title = "Save a room";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, points);
                }
            
            }
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "room file|*.room";
            openFileDialog1.FileName = "firstroom.room";
            openFileDialog1.Title = "Open a room";
            openFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (openFileDialog1.FileName != "")
            {
                string json = File.ReadAllText(openFileDialog1.FileName);
                points = JsonConvert.DeserializeObject<PointCollection>(json);
                DrawEverything();

            }
        }

        private void CalculateGridValues()
        {
            PointCollection temp = new PointCollection();
            foreach (Point p in points)
            {
                temp.Add(p);
            }
            double area = AreaOfPolygon(temp) / 10000;
            
            if (double.TryParse(Height.Text, out double H) && double.TryParse(Coats.Text, out double C) && double.TryParse(AntiArea.Text, out double A))
            {
                double distance = 0;
                if (temp.Count > 1)
                {
                    for (int i = 0; i < temp.Count - 1; i++)
                    {
                        distance += GetDistance(temp[i], temp[i + 1]) / 100.0 * H;
                    }
                }
                double paint = distance * H;
                double litres = (paint - A) / 10.0 * C;
                if (litres < 0.0)
                {
                    MessageBox.Show("You have more doors and windows than wall to put them in", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    Area.Text = area.ToString("0.##");
                    Volume.Text = (area * H).ToString("0.##");
                    Paint.Text = paint.ToString("0.##");
                    Litres.Text = litres.ToString("0.##");
                }
            }
        }

        private void CalculateGrid(object sender, RoutedEventArgs e)
        {
            CalculateGridValues();
        }
    }
}
