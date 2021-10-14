using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IQTestsGenerator
{
    public class TestsGenerator
    {
        public const int mainSize = 50;

        public static int elements = 3; //test size -> 3x3
        public static int size = mainSize; //size on single matrice
        public static int answersCount = 5; //number of answers

        public void GenerateTest(string testType, out Grid test, out Grid correctAnswer, out Grid answers)
        {
            size = mainSize;

            int isBlack = Randomizer.rand.Next(3);
            bool mayBeTheSame = Randomizer.rand.Next(2) == 0;

            test = new Grid();
            test.Height = elements * size;
            test.Width = elements * size;
            for (int i = 0; i < elements; i++)
            {
                ColumnDefinition c = new ColumnDefinition();
                c.Width = new GridLength(1, GridUnitType.Star);
                test.ColumnDefinitions.Add(c);

                RowDefinition r = new RowDefinition();
                r.Height = new GridLength(1, GridUnitType.Star);
                test.RowDefinitions.Add(r);
            }

            correctAnswer = new Grid();
            correctAnswer.Height = size;
            correctAnswer.Width = size;

            answers = new Grid();
            answers.Height = size;
            answers.Width = size * answersCount;
            for (int i = 0; i < answersCount; i++)
            {
                ColumnDefinition c = new ColumnDefinition();
                c.Width = new GridLength(1, GridUnitType.Star);
                answers.ColumnDefinitions.Add(c);
            }

            //----------------------------------- 00 ----------------------------------
            if (int.Parse(testType.Split('_')[0]) == 0)
            {
                mayBeTheSame = false;

                int correctAnswerFigure = Randomizer.rand.Next(answersCount);
                List<int> figureTypes = new List<int>();
                for (int i = 0; i < Enum.GetNames(typeof(FigureType)).Length; i++)
                    figureTypes.Add(i);

                int figureType = figureTypes[Randomizer.rand.Next(figureTypes.Count)];
                Figure f = new Figure();
                f.type = (FigureType)figureType;
                f.height = size / 2 + Randomizer.rand.Next(size / 2);
                f.width = (figureType == 0 ? Randomizer.rand.Next(5) : (size / 2 + Randomizer.rand.Next(size / 2))) + 1;
                if (Randomizer.rand.Next(2) == 0)
                    f.fillColor = Colors.Black;
                else if (isBlack == 1)
                {
                    f.fillColor = Colors.Gray;
                    f.borderWidth = 1;
                }
                else
                {
                    f.fillColor = Colors.White;
                    f.borderWidth = 1;
                }

                for (int row = 0; row < elements; row++)
                    for (int column = 0; column < elements; column++)
                    {
                        if (testType == "00_02" || testType == "00_04")
                        {
                            f.rotation = Randomizer.rand.Next(360);
                            //f.height /= 1.2;
                            //f.width /= 1.2;
                        }
                        if (testType == "00_03" || testType == "00_04")
                        {
                            int newSize = Randomizer.rand.Next(size / 2);
                            f.height = size / 2 + newSize;
                            f.width = size / 2 + newSize;
                        }

                        Shape s = f.GenerateShape();
                        s.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        s.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        s.Margin = new Thickness(f.center.X, f.center.Y, 0, 0);

                        if (row == elements - 1 && column == elements - 1)
                            correctAnswer.Children.Add(s);
                        else
                        {
                            Grid.SetRow(s, row);
                            Grid.SetColumn(s, column);
                            test.Children.Add(s);
                        }

                        if (row == elements - 1 && column == elements - 1)
                        {
                            Shape sCopy = f.GenerateShape();
                            sCopy.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            sCopy.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            s.Margin = new Thickness(f.center.X, f.center.Y, 0, 0);
                            Grid.SetColumn(sCopy, correctAnswerFigure);
                            answers.Children.Add(sCopy);
                        }
                    }
                figureTypes.Remove(figureType);

                for (int i = 0; i < answersCount - 1; i++)
                {
                    f = new Figure();
                    figureType = figureTypes[Randomizer.rand.Next(figureTypes.Count)];
                    f.type = (FigureType)figureType;
                    figureTypes.Remove(figureType);
                    f.height = size / 2 + Randomizer.rand.Next(size / 2);
                    f.width = (figureType == 0 ? Randomizer.rand.Next(5) : (size / 2 + Randomizer.rand.Next(size / 2))) + 1;
                    if ((Randomizer.rand.Next(2) == 0 && int.Parse(testType.Split('_')[1]) < 5) || (isBlack == 0 && (testType == "01_05" || testType == "01_06" || testType == "01_07")))
                        f.fillColor = Colors.Black;
                    else if (isBlack == 1)
                    {
                        f.fillColor = Colors.Gray;
                        f.borderWidth = 1;
                    }
                    else
                    {
                        f.fillColor = Colors.White;
                        f.borderWidth = 1;
                    }

                    Shape s = f.GenerateShape();
                    s.HorizontalAlignment = HorizontalAlignment.Center;
                    s.VerticalAlignment = VerticalAlignment.Center;
                    s.Margin = new Thickness(f.center.X, f.center.Y, 0, 0);
                    if (i < correctAnswerFigure)
                        Grid.SetColumn(s, i);
                    else
                        Grid.SetColumn(s, i + 1);
                    answers.Children.Add(s);
                }
            }


            //----------------------------------- 01 ----------------------------------
            if (int.Parse(testType.Split('_')[0]) == 1)
            {
                if (int.Parse(testType.Split('_')[1]) < 5)
                    mayBeTheSame = false;

                int correctAnswerFigure = 0;
                List<int> figureTypes = new List<int>();
                for (int i = 0; i < Enum.GetNames(typeof(FigureType)).Length; i++)
                    figureTypes.Add(i);

                while (figureTypes.Count > elements)
                    figureTypes.RemoveAt(Randomizer.rand.Next(figureTypes.Count));

                bool[,] isFigureAdded = new bool[elements, elements];

                for (int i = 0; i < figureTypes.Count; i++)
                {
                    Figure f = new Figure();
                    f.type = (FigureType)figureTypes[i];
                    f.height = size / 2 + Randomizer.rand.Next(size / 2);
                    f.width = (figureTypes[i] == 0 ? Randomizer.rand.Next(5) : (size / 2 + Randomizer.rand.Next(size / 2))) + 1;
                    if ((Randomizer.rand.Next(2) == 0 && int.Parse(testType.Split('_')[1]) < 5) || (isBlack == 0 && (testType == "01_05" || testType == "01_06" || testType == "01_07")))
                        f.fillColor = Colors.Black;
                    else if (isBlack == 1)
                    {
                        f.fillColor = Colors.Gray;
                        f.borderWidth = 1;
                    }
                    else
                    {
                        f.fillColor = Colors.White;
                        f.borderWidth = 1;
                    }

                    List<int> availableColumns = Enumerable.Range(0, elements).ToList();
                    for (int row = 0; row < elements; row++)
                    {
                        if (testType == "01_02" || testType == "01_04" || testType == "01_06" || testType == "01_07")
                        {
                            f.rotation = Randomizer.rand.Next(360);
                            f.height /= 1.2;
                            f.width /= 1.2;
                        }
                        if (testType == "01_03" || testType == "01_04" || testType == "01_07")
                        {
                            int newSize = Randomizer.rand.Next(size / 2);
                            f.height = size / 2 + newSize;
                            f.width = size / 2 + newSize;
                        }

                        int column = availableColumns[Randomizer.rand.Next(availableColumns.Count)];

                        while (isFigureAdded[row, column] || (i < figureTypes.Count - 1 && row != elements - 1 && isFigureAdded[row + 1, availableColumns[(availableColumns.FindIndex(x => x == column) + 1) % availableColumns.Count]]))
                            column = availableColumns[(availableColumns.FindIndex(x => x == column) + 1) % availableColumns.Count];

                        availableColumns.Remove(column);
                        isFigureAdded[row, column] = true;

                        Shape s = f.GenerateShape();
                        s.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        s.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        s.Margin = new Thickness(f.center.X, f.center.Y, 0, 0);

                        if (row == elements - 1 && column == elements - 1)
                        {
                            correctAnswer.Children.Add(s);
                            correctAnswerFigure = figureTypes[i];
                        }
                        else
                        {
                            Grid.SetRow(s, row);
                            Grid.SetColumn(s, column);
                            test.Children.Add(s);
                        }

                        if (row == elements - 1)
                        {
                            Shape sCopy = f.GenerateShape();
                            sCopy.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            sCopy.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            s.Margin = new Thickness(f.center.X, f.center.Y, 0, 0);
                            Grid.SetColumn(sCopy, i);
                            answers.Children.Add(sCopy);
                        }
                    }

                }

                for (int i = 0; i < answersCount - figureTypes.Count; i++)
                {
                    Figure f = new Figure();
                    f.type = (figureTypes[i] != correctAnswerFigure || mayBeTheSame) ? (FigureType)figureTypes[i] : (FigureType)figureTypes[(i + 1) % figureTypes.Count];
                    f.height = size / 2 + Randomizer.rand.Next(size / 2);
                    f.width = (figureTypes[i] == 0 ? Randomizer.rand.Next(5) : (size / 2 + Randomizer.rand.Next(size / 2))) + 1;
                    if ((Randomizer.rand.Next(2) == 0 && int.Parse(testType.Split('_')[1]) < 5) || (isBlack == 0 && (testType == "01_05" || testType == "01_06" || testType == "01_07")))
                        f.fillColor = Colors.Black;
                    else if (isBlack == 1)
                    {
                        f.fillColor = Colors.Gray;
                        f.borderWidth = 1;
                    }
                    else
                    {
                        f.fillColor = Colors.White;
                        f.borderWidth = 1;
                    }
                    if (testType == "01_02" || testType == "01_03" || testType == "01_04" || testType == "01_06" || testType == "01_07")
                        f.rotation = Randomizer.rand.Next(360);
                    if (testType == "01_04")
                        f.center = new Point(size / 4 - Randomizer.rand.Next(size / 2), size / 4 - Randomizer.rand.Next(size / 2));

                    Shape s = f.GenerateShape();
                    s.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    s.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    s.Margin = new Thickness(f.center.X, f.center.Y, 0, 0);
                    Grid.SetColumn(s, figureTypes.Count + i);
                    answers.Children.Add(s);
                }

            }

            if (testType == "01_05" || testType == "01_06" || testType == "01_07")
            {
                size /= 3;
                int correctAnswerFigure = 0;
                List<int> figureTypes = new List<int>();
                for (int i = 1; i < Enum.GetNames(typeof(FigureType)).Length; i++)
                    figureTypes.Add(i);

                while (figureTypes.Count > elements)
                    figureTypes.RemoveAt(Randomizer.rand.Next(figureTypes.Count));

                bool[,] isFigureAdded = new bool[elements, elements];

                for (int i = 0; i < figureTypes.Count; i++)
                {
                    Figure f = new Figure();
                    f.type = (FigureType)figureTypes[i];
                    f.height = size / 2 + Randomizer.rand.Next(size / 2);
                    f.width = (figureTypes[i] == 0 ? Randomizer.rand.Next(5) : (size / 2 + Randomizer.rand.Next(size / 2))) + 1;
                    if (isBlack == 0)
                        f.fillColor = Colors.Black;
                    else if (isBlack == 1)
                        f.fillColor = Colors.Gray;
                    else
                        f.fillColor = Colors.White;

                    List<int> availableColumns = Enumerable.Range(0, elements).ToList();
                    for (int row = 0; row < elements; row++)
                    {
                        if (testType == "01_06" || testType == "01_07")
                            f.rotation = Randomizer.rand.Next(360);
                        if (testType == "01_07")
                        {
                            f.height = size / 2 + Randomizer.rand.Next(size / 2);
                            f.width = (figureTypes[i] == 0 ? Randomizer.rand.Next(5) : (size / 2 + Randomizer.rand.Next(size / 2))) + 1;
                        }

                        int column = availableColumns[Randomizer.rand.Next(availableColumns.Count)];

                        while (isFigureAdded[row, column] || (i < figureTypes.Count - 1 && row != elements - 1 && isFigureAdded[row + 1, availableColumns[(availableColumns.FindIndex(x => x == column) + 1) % availableColumns.Count]]))
                            column = availableColumns[(availableColumns.FindIndex(x => x == column) + 1) % availableColumns.Count];

                        availableColumns.Remove(column);
                        isFigureAdded[row, column] = true;

                        Shape s = f.GenerateShape();
                        s.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        s.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        s.Margin = new Thickness(f.center.X, f.center.Y, 0, 0);

                        if (row == elements - 1 && column == elements - 1)
                        {
                            correctAnswer.Children.Add(s);
                            correctAnswerFigure = figureTypes[i];
                        }
                        else
                        {
                            Grid.SetRow(s, row);
                            Grid.SetColumn(s, column);
                            test.Children.Add(s);
                        }

                        if (row == elements - 1)
                        {
                            Shape sCopy = f.GenerateShape();
                            sCopy.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            sCopy.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            s.Margin = new Thickness(f.center.X, f.center.Y, 0, 0);
                            Grid.SetColumn(sCopy, i);
                            answers.Children.Add(sCopy);
                        }
                    }

                }

                for (int i = 0; i < answersCount - figureTypes.Count; i++)
                {
                    Figure f = new Figure();
                    f.type = (figureTypes[i] != correctAnswerFigure || !mayBeTheSame) ? (FigureType)figureTypes[i] : (FigureType)figureTypes[(i + 1) % figureTypes.Count];
                    f.height = size / 2 + Randomizer.rand.Next(size / 2);
                    f.width = (figureTypes[i] == 0 ? Randomizer.rand.Next(5) : (size / 2 + Randomizer.rand.Next(size / 2))) + 1;
                    if (isBlack == 0)
                        f.fillColor = Colors.Black;
                    else if (isBlack == 1)
                    {
                        f.fillColor = Colors.Gray;
                        f.borderWidth = 1;
                    }
                    else
                        f.fillColor = Colors.White;

                    if (testType == "01_06" || testType == "01_07")
                        f.rotation = Randomizer.rand.Next(360);

                    Shape s = f.GenerateShape();
                    s.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    s.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    s.Margin = new Thickness(f.center.X, f.center.Y, 0, 0);
                    Grid.SetColumn(s, figureTypes.Count + i);
                    answers.Children.Add(s);
                }

            }


            //----------------------------------- 02 -----------------------------------

            if (int.Parse(testType.Split('_')[0]) == 2)
            {
                List<int> possibleFigures = new List<int>() { 0, 2, 4, 5, 7, 9, 10, 11 };
                List<int> figureTypes = new List<int>();
                for (int i = 1; i < Enum.GetNames(typeof(FigureType)).Length; i++)
                    figureTypes.Add(i);
                possibleFigures = possibleFigures.Where(x => x < figureTypes.Count).ToList();
                int figureType = possibleFigures[Randomizer.rand.Next(possibleFigures.Count)];

                Figure f = new Figure();
                f.type = (FigureType)figureType;
                f.height = size / 2 + Randomizer.rand.Next(size / 3);
                f.width = (figureType == 0 ? Randomizer.rand.Next(5) : (size / 6 + Randomizer.rand.Next(size / 4))) + 1;
                if (isBlack == 0)
                    f.fillColor = Colors.Black;
                else if (isBlack == 1)
                {
                    f.fillColor = Colors.Gray;
                    f.borderWidth = 1;
                }
                else
                {
                    f.fillColor = Colors.White;
                    f.borderWidth = 1;
                }

                List<int> possibleAngles = new List<int>() { -135, -90, -45, -30, 30, 45, 90, 135 };
                int rotationAngle = possibleAngles[Randomizer.rand.Next(possibleAngles.Count)];
                int startAngle = Randomizer.rand.Next(4) * 90;

                int figureType2 = possibleFigures[Randomizer.rand.Next(possibleFigures.Count - 1)] + 1;
                size /= 2;
                Figure f2 = new Figure();
                f2.type = (FigureType)figureType2;
                f2.height = size / 2 + Randomizer.rand.Next(size / 3);
                f2.width = (figureType == 0 ? Randomizer.rand.Next(5) : (size / 6 + Randomizer.rand.Next(size / 4))) + 1;
                if (isBlack == 0)
                    f2.fillColor = Colors.Black;
                else if (isBlack == 1)
                {
                    f.fillColor = Colors.Gray;
                    f.borderWidth = 1;
                }
                else
                {
                    f2.fillColor = Colors.White;
                    f2.borderWidth = 1;
                }

                int rotationAngle2 = possibleAngles[Randomizer.rand.Next(possibleAngles.Count)];
                int startAngle2 = Randomizer.rand.Next(4) * 90;

                int option1 = Randomizer.rand.Next(3);
                int option2 = Randomizer.rand.Next(3);

                for (int row = 0; row < elements; row++)
                    for (int column = 0; column < elements; column++)
                    {
                        int currentAngle = 0, currentAngle2 = 0;
                        if (testType == "02_01")
                            currentAngle = startAngle + (row * elements + column) * rotationAngle;
                        if (testType == "02_02")
                            currentAngle = startAngle + (column * elements + row) * rotationAngle;
                        if (testType == "02_03" || testType == "02_07")
                            currentAngle = startAngle + (row + column) * rotationAngle;
                        if (testType == "02_04")
                        {
                            currentAngle = startAngle + ((option1 + option2) % 2 == 0 ? (row * elements + column) : (row + column * elements)) * rotationAngle;
                            currentAngle2 = startAngle2 + ((option1 + option2) % 2 == 1 ? (row * elements + column) : (row + column * elements)) * rotationAngle;
                        }
                        if (testType == "02_05")
                        {
                            currentAngle = startAngle + (row + column) * rotationAngle;
                            currentAngle2 = startAngle2 + (column + row) * rotationAngle2;
                        }
                        if (testType == "02_06")
                        {
                            currentAngle = startAngle + (option1 == 0 ? (row + column) : (option1 == 1 ? (row * elements + column) : (row + column * elements))) * rotationAngle;
                            currentAngle2 = startAngle2 + (option2 == 0 ? (row + column) : (option2 == 1 ? (row * elements + column) : (row + column * elements))) * rotationAngle2;
                        }

                        f.rotation = currentAngle;
                        f2.rotation = currentAngle2;


                        Shape s = f.GenerateShape();
                        s.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        s.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                        Shape s2 = f2.GenerateShape();
                        s2.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        s2.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                        if (row == elements - 1 && column == elements - 1)
                        {
                            correctAnswer.Children.Add(s);
                            if (testType == "02_04" || testType == "02_05" || testType == "02_06")
                                correctAnswer.Children.Add(s2);

                            int correctAnswerPosition = Randomizer.rand.Next(answersCount);
                            Shape sCopy = f.GenerateShape();
                            sCopy.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            sCopy.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            Grid.SetColumn(sCopy, correctAnswerPosition);
                            answers.Children.Add(sCopy);

                            if (testType == "02_04" || testType == "02_05" || testType == "02_06")
                            {
                                Shape s2Copy = f2.GenerateShape();
                                s2Copy.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                s2Copy.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                                Grid.SetColumn(s2Copy, correctAnswerPosition);
                                answers.Children.Add(s2Copy);
                            }

                            for (int i = 0; i < answersCount; i++)
                            {
                                if (i == correctAnswerPosition) continue;

                                int r = Randomizer.rand.Next(possibleAngles.Count);
                                f.rotation = currentAngle + possibleAngles[r];
                                int r2 = Randomizer.rand.Next(possibleAngles.Count);
                                f2.rotation = currentAngle2 + possibleAngles[r2];
                                possibleAngles.RemoveAt(r);

                                Shape sWrongAnswer = f.GenerateShape();
                                sWrongAnswer.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                sWrongAnswer.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                                Grid.SetColumn(sWrongAnswer, i);
                                answers.Children.Add(sWrongAnswer);

                                if (testType == "02_04" || testType == "02_05" || testType == "02_06")
                                {
                                    Shape s2WrongAnswer = f2.GenerateShape();
                                    s2WrongAnswer.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                    s2WrongAnswer.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                                    Grid.SetColumn(s2WrongAnswer, i);
                                    answers.Children.Add(s2WrongAnswer);
                                }
                            }
                        }
                        else
                        {
                            Grid.SetRow(s, row);
                            Grid.SetColumn(s, column);
                            test.Children.Add(s);

                            if (testType == "02_04" || testType == "02_05" || testType == "02_06")
                            {
                                Grid.SetRow(s2, row);
                                Grid.SetColumn(s2, column);
                                test.Children.Add(s2);
                            }
                        }
                    }
            }


            //----------------------------------- 03 -----------------------------------

            if (int.Parse(testType.Split('_')[0]) == 3)
            {
                List<int> possibleFigures = new List<int>();
                for (int i = 0; i < Enum.GetNames(typeof(FigureType)).Length; i++)
                    possibleFigures.Add(i);
                int figureType = possibleFigures[Randomizer.rand.Next(possibleFigures.Count)];

                List<double> possibleRatios = new List<double>() { 0.9, 1.1 };
                double scaleRatio = possibleRatios[Randomizer.rand.Next(possibleRatios.Count)];

                size = (int)(size * (scaleRatio > 1 ? 0.35 : 1.1));

                Figure f = new Figure();
                f.type = (FigureType)figureType;
                f.height = size + (scaleRatio > 1 ? 1 : -1) * Randomizer.rand.Next(size / 4);
                f.width = size + (scaleRatio > 1 ? 1 : -1) * Randomizer.rand.Next(size / 4);
                f.rotation = Randomizer.rand.Next(360);
                if (isBlack == 0)
                    f.fillColor = Colors.Black;
                else if (isBlack == 1)
                {
                    f.fillColor = Colors.Gray;
                    f.borderWidth = 1;
                }
                else
                {
                    f.fillColor = Colors.White;
                    f.borderWidth = 1;
                }

                double startWidth = f.width;
                double startHeight = f.height;

                List<int> powers = new List<int>();
                for (int i = 0; i < elements; i++)
                    for (int j = 0; j < elements; j++)
                        powers.Add(2 * j);

                for (int row = 0; row < elements; row++)
                    for (int column = 0; column < elements; column++)
                    {

                        if (testType == "03_01")
                        {
                            f.width = startWidth * Math.Pow(scaleRatio, row * elements + column);
                            f.height = startHeight * Math.Pow(scaleRatio, row * elements + column);
                        }
                        if (testType == "03_02")
                        {
                            f.width = startWidth * Math.Pow(scaleRatio, column * elements + row);
                            f.height = startHeight * Math.Pow(scaleRatio, column * elements + row);
                        }
                        if (testType == "03_03" || testType == "03_04")
                        {
                            f.width = startWidth * Math.Pow(scaleRatio, row + column);
                            f.height = startHeight * Math.Pow(scaleRatio, row + column);
                        }

                        if (testType == "03_05" || testType == "03_06")
                        {
                            int i = Randomizer.rand.Next(powers.Count);
                            f.width = startWidth * Math.Pow(scaleRatio, powers[i]);
                            f.height = startHeight * Math.Pow(scaleRatio, powers[i]);
                            powers.RemoveAt(i);
                        }

                        if (testType == "03_04" || testType == "03_06")
                        {
                            f.rotation = Randomizer.rand.Next(360);
                        }

                        if (figureType == 0)
                            f.width = startWidth;

                        Shape s = f.GenerateShape();
                        s.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        s.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                        if (row == elements - 1 && column == elements - 1)
                        {
                            correctAnswer.Children.Add(s);

                            int correctAnswerPosition = Randomizer.rand.Next(answersCount);
                            Shape sCopy = f.GenerateShape();
                            sCopy.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            sCopy.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            Grid.SetColumn(sCopy, correctAnswerPosition);
                            answers.Children.Add(sCopy);

                            List<double> ratios = new List<double>();
                            if (scaleRatio < 1)
                            {
                                ratios.Add(0.8);
                                for (int i = 0; i < answersCount; i++)
                                    ratios.Add(1.2 + i * 0.2);
                            }
                            else
                            {
                                ratios.Add(1.2);
                                for (int i = 0; i < answersCount; i++)
                                    ratios.Add(0.8 - i * 0.2);
                            }

                            double answerHeight = f.height;
                            double answerWidth = f.width;

                            for (int i = 0; i < answersCount; i++)
                            {
                                if (i == correctAnswerPosition) continue;

                                f.height = answerHeight * ratios[i];
                                if (figureType != 0)
                                    f.width = answerWidth * ratios[i];

                                Shape sWrongAnswer = f.GenerateShape();
                                sWrongAnswer.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                sWrongAnswer.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                                Grid.SetColumn(sWrongAnswer, i);
                                answers.Children.Add(sWrongAnswer);
                            }
                        }
                        else
                        {
                            Grid.SetRow(s, row);
                            Grid.SetColumn(s, column);
                            test.Children.Add(s);
                        }
                    }
            }


            //----------------------------------- 05 -----------------------------------

            if (int.Parse(testType.Split('_')[0]) == 5)
            {
                int sizeSmall = size / 3;

                List<int> possibleFigures = new List<int>();
                for (int i = 0; i < Enum.GetNames(typeof(FigureType)).Length; i++)
                    possibleFigures.Add(i);

                Figure[] fig = new Figure[elements];

                for (int i = 0; i < fig.Length; i++)
                {
                    int figureType = possibleFigures[Randomizer.rand.Next(possibleFigures.Count)];
                    Figure f = new Figure();
                    f.type = (FigureType)figureType;
                    f.height = sizeSmall / 2 + Randomizer.rand.Next(sizeSmall / 2);
                    f.width = (figureType == 0 ? Randomizer.rand.Next(5) : (sizeSmall / 2 + Randomizer.rand.Next(sizeSmall / 3))) + 1;
                    f.rotation = Randomizer.rand.Next(360);
                    if (Randomizer.rand.Next(2) == 0)
                        f.fillColor = Colors.Black;
                    else
                    {
                        f.fillColor = Colors.White;
                        f.borderWidth = 1;
                    }
                    fig[i] = f;
                }

                bool isIncreasing = Randomizer.rand.Next(2) == 0;
                int option = Randomizer.rand.Next(3);

                List<int> possibleNumbers = new List<int>();
                for (int i = 1; i <= elements * elements; i++)
                    possibleNumbers.Add(i);
                List<int> numbers = new List<int>();
                for (int i = 0; i < elements; i++)
                {
                    int number = possibleNumbers[Randomizer.rand.Next(possibleNumbers.Count)];
                    numbers.Add(number);
                    possibleNumbers.Remove(number);
                }

                for (int row = 0; row < elements; row++)
                    for (int column = 0; column < elements; column++)
                    {
                        List<Figure> figuresToAdd = new List<Figure>();
                        if (testType == "05_01" || testType == "05_06")
                        {
                            int figuresCount = (option == 0 ? row * elements + column : (option == 1 ? row + column * elements : row + column)) + 1;
                            if (!isIncreasing)
                                figuresCount = (option == 2 ? elements + elements : elements * elements) - figuresCount + 1;

                            for (int i = 0; i < figuresCount; i++)
                                figuresToAdd.Add(fig[0]);
                        }
                        if (testType == "05_02" || testType == "05_07")
                        {
                            for (int i = 0; i <= (isIncreasing ? column : elements - column - 1); i++)
                                for (int j = 0; j < elements; j++)
                                    if ((isIncreasing && j <= row) || (!isIncreasing && j <= elements - row - 1))
                                        figuresToAdd.Add(fig[i]);
                                    else
                                        figuresToAdd.Add(null);
                        }
                        if (testType == "05_03")
                        {
                            for (int i = 0; i <= (isIncreasing ? row : elements - row - 1); i++)
                                for (int j = 0; j < elements; j++)
                                    if ((isIncreasing && j <= column) || (!isIncreasing && j <= elements - column - 1))
                                        figuresToAdd.Add(fig[i]);
                                    else
                                        figuresToAdd.Add(null);
                        }
                        if (testType == "05_04" || testType == "05_08")
                        {
                            for (int i = 0; i < numbers[column]; i++)
                                figuresToAdd.Add(fig[row]);
                        }
                        if (testType == "05_05")
                        {
                            for (int i = 0; i < numbers[row]; i++)
                                figuresToAdd.Add(fig[column]);
                        }

                        Grid gridToAdd = AddFiguresToGrid(figuresToAdd, int.Parse(testType.Split('_')[1]) > 5);

                        if (row == elements - 1 && column == elements - 1)
                        {
                            correctAnswer.Children.Add(gridToAdd);

                            int correctAnswerPosition = Randomizer.rand.Next(answersCount);
                            Grid gridToAddCopy = AddFiguresToGrid(figuresToAdd, int.Parse(testType.Split('_')[1]) > 5);
                            gridToAddCopy.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            gridToAddCopy.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            Grid.SetColumn(gridToAddCopy, correctAnswerPosition);
                            answers.Children.Add(gridToAddCopy);

                            List<int> possibleNumberModification = new List<int>();
                            for (int i = 1; i < answersCount; i++)
                            {
                                if (figuresToAdd.Count(x => x != null) - i >= 0)
                                    possibleNumberModification.Add(-i);
                                if (figuresToAdd.Count(x => x != null) + i <= elements * elements)
                                    possibleNumberModification.Add(i);
                            }


                            for (int i = 0; i < answersCount; i++)
                            {
                                if (i == correctAnswerPosition) continue;

                                List<Figure> figuresToAddCopy = new List<Figure>(figuresToAdd);

                                int modificationNumber = possibleNumberModification[Randomizer.rand.Next(possibleNumberModification.Count)];
                                possibleNumberModification.Remove(modificationNumber);

                                if (modificationNumber > 0)
                                {
                                    for (int j = 0; j < modificationNumber; j++)
                                    {
                                        int pos = Randomizer.rand.Next(figuresToAdd.Count);
                                        while (figuresToAdd[pos] == null)
                                            pos = Randomizer.rand.Next(figuresToAdd.Count);
                                        figuresToAddCopy.Add(figuresToAdd[pos]);
                                    }
                                }
                                else
                                {
                                    for (int j = 0; j < -modificationNumber; j++)
                                    {
                                        while (figuresToAddCopy.Last() == null)
                                            figuresToAddCopy.RemoveAt(figuresToAddCopy.Count - 1);
                                        figuresToAddCopy.RemoveAt(figuresToAddCopy.Count - 1);
                                    }
                                }

                                Grid sWrongAnswer = AddFiguresToGrid(figuresToAddCopy, int.Parse(testType.Split('_')[1]) > 5);
                                sWrongAnswer.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                sWrongAnswer.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                                Grid.SetColumn(sWrongAnswer, i);
                                answers.Children.Add(sWrongAnswer);
                            }
                        }
                        else
                        {
                            Grid.SetRow(gridToAdd, row);
                            Grid.SetColumn(gridToAdd, column);
                            test.Children.Add(gridToAdd);
                        }
                    }

            }

            test.Measure(new Size(test.Width, test.Height));
            test.Arrange(new Rect(new Size(test.Width, test.Height)));
            correctAnswer.Measure(new Size(correctAnswer.Width, correctAnswer.Height));
            correctAnswer.Arrange(new Rect(new Size(correctAnswer.Width, correctAnswer.Height)));
            answers.Measure(new Size(answers.Width, answers.Height));
            answers.Arrange(new Rect(new Size(answers.Width, answers.Height)));
        }


        public Figure GenerateRandomFigure(int figureType = -1)
        {
            Figure f = new Figure();
            if (figureType == -1)
                f.type = (FigureType)Randomizer.rand.Next(Enum.GetNames(typeof(FigureType)).Length - 1) + 1;
            else
                f.type = (FigureType)figureType;
            f.height = size / 3 + size / 2 / 2 + 3;
            f.width = f.type == 0 ? 3 : size / 3 + size / 2 / 2 + 3;
            f.width *= 1.5;
            f.rotation = Randomizer.rand.Next(360);
            f.center = new System.Windows.Point(size / 8 - size / 4 / 2, size / 8 - size / 4 / 2);
            int fillColor = 5;
            f.fillColor = Color.FromArgb(255, (byte)(255 * fillColor / 5), (byte)(255 * fillColor / 5), (byte)(255 * fillColor / 5));
            f.borderWidth = 2;

            return f;
        }


        Grid AddFiguresToGrid(List<Figure> figures, bool isRotation)
        {
            Grid g = new Grid();
            g.Height = size * 0.9;
            g.Width = size * 0.9;
            for (int i = 0; i < elements; i++)
            {
                ColumnDefinition c = new ColumnDefinition();
                c.Width = new GridLength(1, GridUnitType.Star);
                g.ColumnDefinitions.Add(c);

                RowDefinition r = new RowDefinition();
                r.Height = new GridLength(1, GridUnitType.Star);
                g.RowDefinitions.Add(r);
            }

            for (int i = 0; i < figures.Count; i++)
            {
                if (figures[i] == null)
                    continue;
                if (isRotation)
                    figures[i].rotation = Randomizer.rand.Next(360);
                Shape s = figures[i].GenerateShape();
                s.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                s.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetColumn(s, i % elements);
                Grid.SetRow(s, i / elements);
                g.Children.Add(s);
            }

            return g;
        }
    }
}