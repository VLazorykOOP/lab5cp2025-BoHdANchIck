using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Collections.Generic;

namespace HermiteCurve
{
    public partial class MainWindow : Window
    {
        private Canvas MyCanvas;
        private TextBox[] inputBoxes = new TextBox[8];

        public MainWindow()
        {
            InitializeComponent();
            CreateUI();
            DrawHermiteCurve();
        }

        private void CreateUI()
        {
            Grid grid = new Grid();
            this.Content = grid;

            ColumnDefinition col1 = new ColumnDefinition { Width = new GridLength(200) };
            ColumnDefinition col2 = new ColumnDefinition();
            grid.ColumnDefinitions.Add(col1);
            grid.ColumnDefinitions.Add(col2);

            StackPanel panel = new StackPanel { Margin = new Thickness(10) };
            grid.Children.Add(panel);
            Grid.SetColumn(panel, 0);

            string[] labels = { "X1", "Y1", "Vx1", "Vy1", "X2", "Y2", "Vx2", "Vy2" };

            for (int i = 0; i < 8; i++)
            {
                panel.Children.Add(new TextBlock { Text = labels[i] });
                inputBoxes[i] = new TextBox { Text = (i % 2 == 0 ? "100" : "300"), Margin = new Thickness(0, 0, 0, 5) };
                panel.Children.Add(inputBoxes[i]);
            }

            Button drawButton = new Button { Content = "Намалювати криву", Margin = new Thickness(0, 10, 0, 0) };
            drawButton.Click += (s, e) => DrawHermiteCurve();
            panel.Children.Add(drawButton);

            MyCanvas = new Canvas { Background = Brushes.White };
            grid.Children.Add(MyCanvas);
            Grid.SetColumn(MyCanvas, 1);
        }

        private void DrawHermiteCurve()
        {
            MyCanvas.Children.Clear();

            try
            {
                double x1 = double.Parse(inputBoxes[0].Text);
                double y1 = double.Parse(inputBoxes[1].Text);
                double vx1 = double.Parse(inputBoxes[2].Text);
                double vy1 = double.Parse(inputBoxes[3].Text);
                double x2 = double.Parse(inputBoxes[4].Text);
                double y2 = double.Parse(inputBoxes[5].Text);
                double vx2 = double.Parse(inputBoxes[6].Text);
                double vy2 = double.Parse(inputBoxes[7].Text);

                Point P1 = new Point(x1, y1);
                Vector V1 = new Vector(vx1, vy1);
                Point P2 = new Point(x2, y2);
                Vector V2 = new Vector(vx2, vy2);

                Polyline curve = new Polyline
                {
                    Stroke = Brushes.DarkBlue,
                    StrokeThickness = 2
                };

                List<Point> points = new List<Point>();
                int steps = 100;
                for (int i = 0; i <= steps; i++)
                {
                    double t = (double)i / steps;
                    points.Add(Hermite(P1, V1, P2, V2, t));
                }

                curve.Points = new PointCollection(points);
                MyCanvas.Children.Add(curve);

                DrawPoint(P1, Brushes.Red);
                DrawPoint(P2, Brushes.Green);
                DrawArrow(P1, V1, Brushes.Orange);
                DrawArrow(P2, V2, Brushes.Purple);
            }
            catch { }
        }

        private Point Hermite(Point p0, Vector m0, Point p1, Vector m1, double t)
        {
            double t2 = t * t;
            double t3 = t2 * t;
            double h1 = 2 * t3 - 3 * t2 + 1;
            double h2 = t3 - 2 * t2 + t;
            double h3 = -2 * t3 + 3 * t2;
            double h4 = t3 - t2;

            double x = h1 * p0.X + h2 * m0.X + h3 * p1.X + h4 * m1.X;
            double y = h1 * p0.Y + h2 * m0.Y + h3 * p1.Y + h4 * m1.Y;

            return new Point(x, y);
        }

        private void DrawPoint(Point p, Brush color)
        {
            Ellipse dot = new Ellipse
            {
                Fill = color,
                Width = 6,
                Height = 6
            };
            Canvas.SetLeft(dot, p.X - 3);
            Canvas.SetTop(dot, p.Y - 3);
            MyCanvas.Children.Add(dot);
        }

        private void DrawArrow(Point start, Vector vec, Brush color)
        {
            Point end = start + vec;

            Line line = new Line
            {
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y,
                Stroke = color,
                StrokeThickness = 2
            };
            MyCanvas.Children.Add(line);

            Vector norm = vec;
            norm.Normalize();
            Vector ortho = new Vector(-norm.Y, norm.X);
            Point arrow1 = end - norm * 10 + ortho * 5;
            Point arrow2 = end - norm * 10 - ortho * 5;

            Polygon triangle = new Polygon
            {
                Fill = color,
                Points = new PointCollection { end, arrow1, arrow2 }
            };
            MyCanvas.Children.Add(triangle);
        }
    }
}
