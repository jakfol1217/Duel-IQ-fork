using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IQTestsGenerator
{
    public class Figure
    {
        public FigureType type;

        public double width = 100;

        public double height = 100;

        public double rotation = 0;

        public Point center = new Point(0, 0);

        public int borderWidth = 0;

        public Color fillColor = Colors.Black;


        public Shape GenerateShape()
        {
            /*if (type == FigureType.line)
            {
                Rectangle line = new Rectangle();
                line.Height = height;
                line.Width = width;
                line.LayoutTransform = new RotateTransform(rotation);
                line.Fill = new SolidColorBrush(Colors.Black);
                line.Tag = Clone();
                return line;
            }*/
            if (type == FigureType.triangle)
            {
                Path triangle = new Path();
                int size = (int)(height * (rotation != 0 ? 0.75 : 1.0));
                string sData = string.Format("M 0,0 L{0},0 L{1},{0} Z", size, (int)(width * (rotation != 0 ? 0.75 : 1.0)));
                var converter = TypeDescriptor.GetConverter(typeof(Geometry));
                triangle.Data = (Geometry)converter.ConvertFrom(sData);

                triangle.Height = triangle.Width = size;
                triangle.Stretch = Stretch.Uniform;
                triangle.LayoutTransform = new RotateTransform(rotation);
                triangle.StrokeThickness = borderWidth;
                triangle.Stroke = new SolidColorBrush(Colors.Black);
                triangle.Fill = new SolidColorBrush(fillColor);
                triangle.Tag = Clone();
                return triangle;
            }
            if (type == FigureType.ellipse)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Height = height/1.2;
                ellipse.Width = width/1.2;
                ellipse.LayoutTransform = new RotateTransform(rotation);
                ellipse.StrokeThickness = borderWidth;
                ellipse.Stroke = new SolidColorBrush(Colors.Black);
                ellipse.Fill = new SolidColorBrush(fillColor);
                ellipse.Tag = Clone();
                return ellipse;
            }
            if (type == FigureType.rectangle)
            {
                Rectangle rectangle = new Rectangle();
                rectangle.Height = height;
                rectangle.Width = width;

                if (rotation != 0)
                {
                    rectangle.Height *= 0.7;
                    rectangle.Width *= 0.7;
                }

                rectangle.LayoutTransform = new RotateTransform(rotation);
                rectangle.StrokeThickness = borderWidth;
                rectangle.Stroke = new SolidColorBrush(Colors.Black);
                rectangle.Fill = new SolidColorBrush(fillColor);
                rectangle.Tag = Clone();
                return rectangle;
            }
            if (type == FigureType.circle)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Height = height;
                ellipse.Width = height;
                ellipse.StrokeThickness = borderWidth;
                ellipse.Stroke = new SolidColorBrush(Colors.Black);
                ellipse.Fill = new SolidColorBrush(fillColor);
                ellipse.Tag = Clone();
                return ellipse;
            }
            if (type == FigureType.square)
            {
                Rectangle square = new Rectangle();
                square.Height = height;
                square.Width = height;
                if (rotation != 0)
                {
                    square.Height *= 0.7;
                    square.Width *= 0.7;
                }

                square.LayoutTransform = new RotateTransform(rotation);
                square.StrokeThickness = borderWidth;
                square.Stroke = new SolidColorBrush(Colors.Black);
                square.Fill = new SolidColorBrush(fillColor);
                square.Tag = Clone();
                return square;
            }
            if (type == FigureType.triangleEq)
            {
                Path triangle = new Path();
                int size = (int)(height*0.8);
                string sData = string.Format("M 0,0 L{0},0 L{1},{2} Z", size, size / 2, (int)(size * Math.Sqrt(3) / 2));
                var converter = TypeDescriptor.GetConverter(typeof(Geometry));
                triangle.Data = (Geometry)converter.ConvertFrom(sData);

                triangle.Height = triangle.Width = size;
                triangle.Stretch = Stretch.Uniform;
                triangle.LayoutTransform = new RotateTransform(rotation);
                triangle.StrokeThickness = borderWidth;
                triangle.Stroke = new SolidColorBrush(Colors.Black);
                triangle.Fill = new SolidColorBrush(fillColor);
                triangle.Tag = Clone();
                return triangle;
            }
            if (type == FigureType.t_letter)
            {
                Path t_letter = new Path();
                int size = (int)height;
                string sData = "M0,75 75,75 75,50 50,50 50,0 25,0 25,50 0,50 z";
                var converter = TypeDescriptor.GetConverter(typeof(Geometry));
                t_letter.Data = (Geometry)converter.ConvertFrom(sData);

                t_letter.Height = t_letter.Width = size / 1.5;
                t_letter.Stretch = Stretch.Uniform;
                t_letter.RenderTransformOrigin = new Point(0.5, 0.5);
                t_letter.LayoutTransform = new RotateTransform(rotation);
                t_letter.StrokeThickness = borderWidth;
                t_letter.Stroke = new SolidColorBrush(Colors.Black);
                t_letter.Fill = new SolidColorBrush(fillColor);
                t_letter.Tag = Clone();
                return t_letter;
            }
            if (type == FigureType.star)
            {
                Path star = new Path();
                int size = (int)height;
                string sData = "F1 M 145.637,174.227L 127.619,110.39L 180.809,70.7577L 114.528,68.1664L 93.2725,5.33333L 70.3262,67.569L 4,68.3681L 56.0988,109.423L 36.3629,172.75L 91.508,135.888L 145.637,174.227 Z";
                var converter = TypeDescriptor.GetConverter(typeof(Geometry));
                star.Data = (Geometry)converter.ConvertFrom(sData);

                star.Height = star.Width = size;
                star.Stretch = Stretch.Uniform;
                star.RenderTransformOrigin = new Point(0.5, 0.5);
                star.LayoutTransform = new RotateTransform(rotation);
                star.StrokeThickness = borderWidth;
                star.Stroke = new SolidColorBrush(Colors.Black);
                star.Fill = new SolidColorBrush(fillColor);
                star.Tag = Clone();
                return star;
            }
            if (type == FigureType.heart)
            {
                Path heart = new Path();
                int size = (int)height;
                string sData = "M 41,0 A 20,20 0 0 0 0,40 C 10,50 40,70 40,70 C 40,70 60,60 80,40 A 20,20 0 0 0 39,0";
                var converter = TypeDescriptor.GetConverter(typeof(Geometry));
                heart.Data = (Geometry)converter.ConvertFrom(sData);

                heart.Height = heart.Width = size;
                if (rotation != 0)
                {
                    heart.Height *= 0.75;
                    heart.Width *= 0.75;
                }
                heart.Stretch = Stretch.Uniform;
                heart.RenderTransformOrigin = new Point(0.5, 0.5);
                heart.LayoutTransform = new RotateTransform(rotation);
                heart.StrokeThickness = borderWidth;
                heart.Stroke = new SolidColorBrush(Colors.Black);
                heart.Fill = new SolidColorBrush(fillColor);
                heart.Tag = Clone();
                return heart;
            }
            if (type == FigureType.diamond)
            {
                Path diamond = new Path();
                int size = (int)height;
                string sData = "M 40,0L80,65 40,130 0,65z";
                var converter = TypeDescriptor.GetConverter(typeof(Geometry));
                diamond.Data = (Geometry)converter.ConvertFrom(sData);

                diamond.Height = diamond.Width = size;
                diamond.Stretch = Stretch.Uniform;
                diamond.RenderTransformOrigin = new Point(0.5, 0.5);
                diamond.LayoutTransform = new RotateTransform(rotation);
                diamond.StrokeThickness = borderWidth;
                diamond.Stroke = new SolidColorBrush(Colors.Black);
                diamond.Fill = new SolidColorBrush(fillColor);
                diamond.Tag = Clone();
                return diamond;
            }
            if (type == FigureType.spade)
            {
                Path spade = new Path();
                int size = (int)height;
                string sData = "M 27.090278,0C27.677578,2.4401169 32.568287,9.1172075 38.277794,13.395883 44.5214,18.075577 54.191917,21.270771 54.121616,32.299458 53.997116,51.173935 36.183281,55.14587 28.451152,41.878727L28.325452,41.6534 28.468214,42.54768C31.579301,60.439018,47.019001,64,47.019001,64L7.1050001,64C22.217525,60.009224,25.436666,48.475578,25.972797,41.656773L26.001989,41.245934 25.980531,41.28809C18.218597,55.885754 0.12199779,51.321625 0.00041448758,32.179657 -0.072461981,21.151072 9.4808918,17.687477 15.84166,13.275983 22.009571,8.9999886 26.400276,2.4922466 27.090278,0z";
                var converter = TypeDescriptor.GetConverter(typeof(Geometry));
                spade.Data = (Geometry)converter.ConvertFrom(sData);

                spade.Height = spade.Width = size;
                spade.Stretch = Stretch.Uniform;
                spade.RenderTransformOrigin = new Point(0.5, 0.5);
                spade.LayoutTransform = new RotateTransform(rotation);
                spade.StrokeThickness = borderWidth;
                spade.Stroke = new SolidColorBrush(Colors.Black);
                spade.Fill = new SolidColorBrush(fillColor);
                spade.Tag = Clone();
                return spade;
            }
            if (type == FigureType.club)
            {
                Path club = new Path();
                int size = (int)height;
                string sData = "M 31.951325,31.564323L31.841339,31.568395 31.903953,31.783258 31.915174,31.726174z M31.16775,0C40.119466,0 47.375998,7.0728493 47.375998,15.791499 47.375998,16.881823 47.262615,17.946307 47.046711,18.974377L47.01092,19.134573 47.373253,19.121157C47.512274,19.117725 47.65173,19.115999 47.791601,19.115998 56.743446,19.115999 64.000002,26.187519 64.000002,34.9087 64.000002,43.62718 56.743446,50.699999 47.791601,50.699999 41.427469,50.699999 35.921506,47.12517 33.27014,41.925653L33.258569,41.901785 33.344036,42.551062C35.952737,60.468645,51.257999,64.000998,51.257999,64.000998L13.026001,64.000998C27.830163,60.089678,30.718207,48.620466,31.128221,42.155346L31.143891,41.876163 31.025769,42.150654C28.503148,47.677303 22.819095,51.531 16.211101,51.531 7.2577801,51.531 0,44.460766 0,35.739548 0,27.29087 6.8112564,20.391506 15.376863,19.968547L15.536678,19.962632 15.471333,19.738436C15.138189,18.476935 14.961,17.154403 14.961,15.791499 14.961,7.0728493 22.218633,0 31.16775,0z";
                var converter = TypeDescriptor.GetConverter(typeof(Geometry));
                club.Data = (Geometry)converter.ConvertFrom(sData);

                club.Height = club.Width = size;
                club.Stretch = Stretch.Uniform;
                club.RenderTransformOrigin = new Point(0.5, 0.5);
                club.LayoutTransform = new RotateTransform(rotation);
                club.StrokeThickness = borderWidth;
                club.Stroke = new SolidColorBrush(Colors.Black);
                club.Fill = new SolidColorBrush(fillColor);
                club.Tag = Clone();
                return club;
            }
            if (type == FigureType.trapezoid)
            {
                Path trapezoid = new Path();
                int size = (int)height;
                string sData = "M15,0 L60,0 75,30 0,30 z";
                var converter = TypeDescriptor.GetConverter(typeof(Geometry));
                trapezoid.Data = (Geometry)converter.ConvertFrom(sData);

                trapezoid.Height = trapezoid.Width = size;
                trapezoid.Stretch = Stretch.Uniform;
                trapezoid.RenderTransformOrigin = new Point(0.5, 0.5);
                trapezoid.LayoutTransform = new RotateTransform(rotation);
                trapezoid.StrokeThickness = borderWidth;
                trapezoid.Stroke = new SolidColorBrush(Colors.Black);
                trapezoid.Fill = new SolidColorBrush(fillColor);
                trapezoid.Tag = Clone();
                return trapezoid;
            }

            return new Line();
        }

        public Figure Clone()
        {
            Figure clone = new Figure();
            clone.type = type;
            clone.width = width;
            clone.height = height;
            clone.rotation = rotation;
            clone.center = new Point(center.X, center.Y);
            clone.borderWidth = borderWidth;
            clone.fillColor = fillColor;

            return clone;
        }


        public override string ToString()
        {
            return (int)type + ";" + width + ";" + height + ";" + rotation + ";"
                + center.X + ";" + center.Y + ";" + borderWidth + ";" + (fillColor == Colors.Black ? 1 : (fillColor == Colors.Gray ? 0.5 : 0)) + ";";
        }


        public string ToStringTypeBinary()
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < Enum.GetNames(typeof(FigureType)).Length; i++)
                if ((int)type == i)
                    b.Append("1;");
                else
                    b.Append("0;");
            b.Append(width + ";" + height + ";" + rotation + ";" + center.X + ";" + center.Y + ";"
                + borderWidth + ";" + (fillColor == Colors.Black ? 1 : (fillColor == Colors.Gray ? 0.5 : 0)) + ";");

            return b.ToString();
        }


        public bool IsEquals(Figure f)
        {
            if (f.type != type || f.width != width || f.height != height || f.rotation != rotation || f.center.X != center.X || f.center.Y != center.Y
                || f.borderWidth != borderWidth || f.fillColor != fillColor)
                return false;
            else
                return true;
        }


        public string GetFigureType()
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < Enum.GetNames(typeof(FigureType)).Length; i++)
                if ((int)type == i)
                    b.Append("1;");
                else
                    b.Append("0;");

            return b.ToString().Remove(b.ToString().Length - 1);
        }

        public string GetRotation()
        {
            int n = 360 / 10;
            StringBuilder b = new StringBuilder();
            bool isAdded = false;
            for (int i = n; i <= 360; i += n)
                if (rotation < i && !isAdded)
                {
                    b.Append("1;");
                    isAdded = true;
                }
                else
                    b.Append("0;");

            return b.ToString().Remove(b.ToString().Length - 1);
        }

        public string GetFillColor()
        {
            return (fillColor == Colors.White ? "1" : "0") + ";" + (fillColor == Colors.Gray ? "1" : "0") + ";" + (fillColor == Colors.Black ? "1" : "0");
        }

        public string GetWidth()
        {
            int n = TestsGenerator.size / 10;
            StringBuilder b = new StringBuilder();
            bool isAdded = false;
            for (int i = n; i <= TestsGenerator.size; i += n)
                if (width < i && !isAdded)
                {
                    b.Append("1;");
                    isAdded = true;
                }
                else
                    b.Append("0;");

            return b.ToString().Remove(b.ToString().Length - 1);
        }

        public string GetHeight()
        {
            int n = TestsGenerator.size / 10;
            StringBuilder b = new StringBuilder();
            bool isAdded = false;
            for (int i = n; i <= TestsGenerator.size; i += n)
                if (height < i && !isAdded)
                {
                    b.Append("1;");
                    isAdded = true;
                }
                else
                    b.Append("0;");

            return b.ToString().Remove(b.ToString().Length - 1);
        }

        public string GetBorderWidth()
        {
            StringBuilder b = new StringBuilder();
            for (int i = 1; i <= 4; i++)
                if (borderWidth == i && fillColor != Colors.Black)
                    b.Append("1;");
                else
                    b.Append("0;");
            if (fillColor == Colors.Black)
                b.Append("1");
            else
                b.Append("0");

            return b.ToString();
        }
    }

    public static class Randomizer
    {
        public static Random rand = new Random(777);

    }

    public enum FigureType
    {
        t_letter = 0,
        ellipse = 1,
        rectangle = 2,
        circle = 3,
        square = 4,
        triangleEq = 5,
        star = 6,
        heart = 7,
        diamond = 8,
        spade = 9,
        club = 10,
        trapezoid = 11,
        triangle = 12
    }
}
