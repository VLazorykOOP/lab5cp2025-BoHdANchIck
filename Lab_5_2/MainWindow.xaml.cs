using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FractalSky
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (s, e) => GenerateClouds();
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateClouds();
        }

        private void GenerateClouds()
        {
            FractalCanvas.Children.Clear();
            int order = (int)OrderSlider.Value;
            int cloudCount = (int)CloudCountSlider.Value;
            double segmentLength = LengthSlider.Value;
            byte colorIntensity = (byte)ColorSlider.Value;

            Random rand = new Random();

            for (int i = 0; i < cloudCount; i++)
            {
                Point startPoint = new Point(
                    rand.Next(100, (int)(FractalCanvas.ActualWidth - 100)),
                    rand.Next(100, (int)(FractalCanvas.ActualHeight - 100))
                );

                double angle = rand.NextDouble() * 2 * Math.PI;
                var turns = GenerateTurns(order);
                DrawFractal(startPoint, angle, turns, segmentLength, colorIntensity);
            }
        }

        private List<bool> GenerateTurns(int order)
        {
            List<bool> sequence = new List<bool>();
            for (int i = 0; i < order; i++)
            {
                var copy = new List<bool>(sequence);
                sequence.Add(true);
                for (int j = copy.Count - 1; j >= 0; j--)
                    sequence.Add(!copy[j]);
            }
            return sequence;
        }

        private void DrawFractal(Point startPoint, double angle, List<bool> turns, double length, byte colorBase)
        {
            Point current = startPoint;
            Random rand = new Random();

            foreach (var turn in turns)
            {
                byte blue = (byte)Math.Min(255, colorBase + rand.Next(30));
                byte green = (byte)rand.Next(50, 150);
                var brush = new SolidColorBrush(Color.FromRgb(0, green, blue));

                Point next = new Point(
                    current.X + length * Math.Cos(angle),
                    current.Y + length * Math.Sin(angle)
                );

                FractalCanvas.Children.Add(new Line
                {
                    X1 = current.X,
                    Y1 = current.Y,
                    X2 = next.X,
                    Y2 = next.Y,
                    Stroke = brush,
                    StrokeThickness = 2
                });

                angle += (turn ? Math.PI / 2 : -Math.PI / 2);
                current = next;
            }
        }
    }
}
