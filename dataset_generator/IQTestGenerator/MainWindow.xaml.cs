using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace IQTestsGenerator
{
    public partial class MainWindow : Window
    {
        string basePath = @"C:\iqtests";
        int testInstances = 1000;

        DispatcherTimer timer = new DispatcherTimer();
        Random rand = new Random(777);
        RenderTargetBitmap rtb;

        public MainWindow()
        {
            InitializeComponent();
        }

        Grid testGrid, correctAnswerGrid, answersGrid;
        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            StreamWriter w_FigureType = new StreamWriter(basePath + @"\figure_type.csv", false);
            StreamWriter w_rotation = new StreamWriter(basePath + @"\rotation.csv", false);
            StreamWriter w_fillColor = new StreamWriter(basePath + @"\fillColor.csv", false);
            StreamWriter w_width = new StreamWriter(basePath + @"\width.csv", false);
            StreamWriter w_height = new StreamWriter(basePath + @"\height.csv", false);

            int count = 0;
            DateTime startTime = DateTime.Now;

            StreamWriter wr = new StreamWriter(basePath + @"\answers.csv");
            int[] groups = new int[] { 1, 2, 3 };
            int[][] subgroups = new int[][] { new int[]{ 3 }, new int[] { 1, 2, 3 }, new int[] { 1, 3, 4 },
            new int[]{ 1, 2, 3, 4, 5, 6, 7, 8 }};

            foreach (int group in groups)
                foreach (int test in subgroups[group - 1])
                {
                    for (int i = 1; i <= testInstances; i++)
                    {
                        TestsGenerator generator = new TestsGenerator();
                        generator.GenerateTest(group.ToString("D2") + "_" + test.ToString("D2"), out testGrid, out correctAnswerGrid, out answersGrid);

                        SaveGridToImage(basePath + @"\" + count.ToString("D5") + "_test.png", testGrid);
                        SaveGridToImage(basePath + @"\" + count.ToString("D5") + "_answers.png", answersGrid);
                        count++;

                        var items = answersGrid.Children.Cast<UIElement>().OfType<Shape>().ToList();

                        w_FigureType.WriteLine(string.Join(";", items.Select(x => x.Tag as Figure).Select(x => x.type)));
                        w_rotation.WriteLine(string.Join(";", items.Select(x => x.Tag as Figure).Select(x => x.rotation)));
                        w_fillColor.WriteLine(string.Join(";", items.Select(x => x.Tag as Figure).Select(x => x.fillColor)));
                        w_width.WriteLine(string.Join(";", items.Select(x => x.Tag as Figure).Select(x => x.width)));
                        w_height.WriteLine(string.Join(";", items.Select(x => x.Tag as Figure).Select(x => x.height)));

                        Figure correctFigure = null;
                        foreach (Shape s in correctAnswerGrid.Children.OfType<Shape>())
                        {
                            correctFigure = s.Tag as Figure;
                        }

                        for (int column = 0; column < TestsGenerator.answersCount; column++)
                        {
                            var items2 = answersGrid.Children.Cast<UIElement>().Where(x => Grid.GetColumn(x) == column).OfType<Shape>().ToList();
                            foreach (Shape s in items2)
                            {
                                Figure f = s.Tag as Figure;
                                if (!f.IsEquals(correctFigure))
                                {
                                    Grid g = new Grid();
                                    g.Height = 50;
                                    g.Width = 50;
                                    Shape sp = (s.Tag as Figure).GenerateShape();
                                    g.Children.Add(sp);
                                    myCanvas.Children.Add(g);
                                    myCanvas.UpdateLayout();
                                    myCanvas.Children.Remove(g);
                                }
                                else
                                    wr.WriteLine(column);
                            }
                        }

                    }
                }

            w_FigureType.Close();
            w_rotation.Close();
            w_fillColor.Close();
            w_width.Close();
            w_height.Close();

            wr.Close();
            messageTextBlock.Text = "System generated " + count + " tests in " + (int)DateTime.Now.Subtract(startTime).TotalSeconds + " seconds";
        }

        void SaveGridToImage(string path, Grid g)
        {
            rtb = new RenderTargetBitmap((int)g.Width, (int)g.Height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(g);

            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));
            FileStream stream = new FileStream(path, FileMode.Create);
            png.Save(stream);
            stream.Dispose();
            stream.Close();
            rtb.Freeze();
            rtb.Clear();
        }

    }
}