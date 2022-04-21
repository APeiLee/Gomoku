using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using FiveInARow.Model;
using FiveInARow.ViewModel;

namespace FiveInARow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();

            DrawLine();
            SetPoint();
        }

        private const int LenghtOfSide = 560;
        private const int PointNum = 15;
        private const int StartLocation = 0;
        private const int EndLocation = 560;
        private readonly List<Point> _pointList = new List<Point>();
        private int[,] _recordChessPieces = new int[PointNum, PointNum];
        private readonly List<Ellipse> _chessBoardRecord = new List<Ellipse>();
        private Line _winLine = new Line();
        private Ellipse _newEllipseFlag = new Ellipse();
        private Rectangle _lastRectangle;
        private bool _isBlack = true;
        private bool _gameOver;

        private void DrawLine()
        {
            int distance = LenghtOfSide / (PointNum - 1);

            //画竖线
            for (int i = 1; i < PointNum - 1; i++)
            {
                Line setLine = new Line()
                {
                    X1 = StartLocation + distance * i,
                    Y1 = StartLocation,
                    X2 = StartLocation + distance * i,
                    Y2 = EndLocation,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };

                ChessboardCanvas.Children.Add(setLine);
            }

            //画横线
            for (int i = 1; i < PointNum - 1; i++)
            {
                Line setLine = new Line()
                {
                    X1 = StartLocation,
                    Y1 = StartLocation + distance * i,
                    X2 = EndLocation,
                    Y2 = StartLocation + distance * i,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };

                ChessboardCanvas.Children.Add(setLine);
            }
        }

        private void SetPoint()
        {
            int distance = LenghtOfSide / (PointNum - 1);

            for (int i = 0; i < PointNum; i++)
            {
                for (int j = 0; j < PointNum; j++)
                {
                    Point point = new Point(StartLocation + j * distance, StartLocation + i * distance);
                    _pointList.Add(point);
                }
            }

            Ellipse ellipse1 = new Ellipse()
            {
                Margin = new Thickness(_pointList[PointNum * 3 + 3].X - 5, _pointList[PointNum * 3 + 3].Y - 5, 0, 0),
                Width = 10,
                Height = 10,
                Fill = Brushes.Black
            };
            ChessboardCanvas.Children.Add(ellipse1);

            Ellipse ellipse2 = new Ellipse()
            {
                Margin = new Thickness(_pointList[PointNum * 11 + 3].X - 5, _pointList[PointNum * 11 + 3].Y - 5, 0, 0),
                Width = 10,
                Height = 10,
                Fill = Brushes.Black
            };
            ChessboardCanvas.Children.Add(ellipse2);

            Ellipse ellipse3 = new Ellipse()
            {
                Margin = new Thickness(_pointList[PointNum * PointNum / 2].X - 5, _pointList[PointNum * PointNum / 2].Y - 5, 0, 0),
                Width = 10,
                Height = 10,
                Fill = Brushes.Black
            };
            ChessboardCanvas.Children.Add(ellipse3);

            Ellipse ellipse4 = new Ellipse()
            {
                Margin = new Thickness(_pointList[PointNum * 3 + 11].X - 5, _pointList[PointNum * 3 + 11].Y - 5, 0, 0),
                Width = 10,
                Height = 10,
                Fill = Brushes.Black
            };
            ChessboardCanvas.Children.Add(ellipse4);

            Ellipse ellipse5 = new Ellipse()
            {
                Margin = new Thickness(_pointList[PointNum * 11 + 11].X - 5, _pointList[PointNum * 11 + 11].Y - 5, 0, 0),
                Width = 10,
                Height = 10,
                Fill = Brushes.Black
            };
            ChessboardCanvas.Children.Add(ellipse5);
        }

        private PointAndColorNumberModel GetPointOnMouseMove(double x, double y, double differLong = 20)
        {
            int number = 0;
            foreach (var onePoint in _pointList)
            {
                if (onePoint.X - differLong < x && x < onePoint.X + differLong && onePoint.Y - differLong < y && y < onePoint.Y + differLong)
                {
                    PointAndColorNumberModel pointAndColorNumber = new PointAndColorNumberModel()
                    {
                        Point = onePoint,
                        Number = number
                    };
                    return pointAndColorNumber;
                }
                number++;
            }
            return new PointAndColorNumberModel { Point = new Point(-1, -1), Number = -1 };
        }

        private void ChessboardCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_lastRectangle != null)
            {
                ChessboardCanvas.Children.Remove(_lastRectangle);
            }

            _lastRectangle = null;

            Point resultPoint = GetPointOnMouseMove(e.GetPosition(sender as Canvas).X, e.GetPosition(sender as Canvas).Y).Point;
            if (resultPoint != new Point(-1, -1))
            {
                Rectangle rectangle = new Rectangle()
                {
                    Margin = new Thickness(resultPoint.X - 20, resultPoint.Y - 20, 0, 0),
                    Width = 40,
                    Height = 40,
                    Stroke = Brushes.DarkGreen,
                    StrokeThickness = 2,
                };

                _lastRectangle = rectangle;
                ChessboardCanvas.Children.Add(rectangle);
            }
        }


        private void ChessboardCanvas_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_gameOver)
            {
                MessageBoxResult mbr = MessageBox.Show("游戏已结束，是否重新开始？", "系统提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (mbr != MessageBoxResult.Yes)
                {
                    return;
                }

                ClearChessboard();
            }

            PointAndColorNumberModel pointAndColorNumber = GetPointOnMouseMove(e.GetPosition(sender as Canvas).X, e.GetPosition(sender as Canvas).Y);

            if (pointAndColorNumber.Point != new Point(-1, -1))
            {
                if (_recordChessPieces[pointAndColorNumber.Number / PointNum, pointAndColorNumber.Number % PointNum] != 0)
                {
                    MessageBox.Show("这个位置已经有棋子了啊！\n\n认真点好不好ORZ？", "系统提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                AddAChessPieces(pointAndColorNumber.Point.X, pointAndColorNumber.Point.Y);

                RecordChessPieces(pointAndColorNumber.Number, _isBlack);

                if (Judgment())
                {
                    MessageBox.Show(!_isBlack ? "真厉害，你赢了！" : "不好意思，你输了！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    _gameOver = true;
                    return;
                }

                PlayChess();

                if (Judgment())
                {
                    MessageBox.Show(!_isBlack ? "真厉害，你赢了！" : "不好意思，你输了！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    _gameOver = true;
                }
            }
        }

        private void ClearChessboard()
        {
            _gameOver = false;

            ChessboardCanvas.Children.Remove(_winLine);
            ChessboardCanvas.Children.Remove(_newEllipseFlag);

            foreach (var ellipse in _chessBoardRecord)
            {
                ChessboardCanvas.Children.Remove(ellipse);
            }

            _recordChessPieces = new int[PointNum, PointNum];
            _isBlack = true;
        }

        private void AddAChessPieces(double x, double y)
        {
            ChessboardCanvas.Children.Remove(_newEllipseFlag);

            Ellipse ellipse = new Ellipse()
            {
                Width = 40,
                Height = 40,
                Margin = new Thickness(x - 20, y - 20, 0, 0),
                StrokeThickness = 2
            };

            if (_isBlack)
            {
                ellipse.Stroke = Brushes.Black;
                ellipse.Fill = Brushes.Black;
                _isBlack = false;
            }
            else
            {
                ellipse.Stroke = Brushes.CadetBlue;
                ellipse.Fill = Brushes.CadetBlue;
                _isBlack = true;
            }

            ChessboardCanvas.Children.Add(ellipse);
            _chessBoardRecord.Add(ellipse);

            #region 用圆圈提示
            if (_isBlack)
            {
                _newEllipseFlag = new Ellipse()
                {
                    Width = 30,
                    Height = 30,
                    Margin = new Thickness(x - 15, y - 15, 0, 0),
                    StrokeThickness = 2,
                    Stroke = Brushes.Red
                };

                ChessboardCanvas.Children.Add(_newEllipseFlag);
            }
                #endregion
        }

        /// <summary>
        /// 记录棋盘棋子位置
        /// </summary>
        /// <param name="number">一位数组中的位置</param>
        /// <param name="isBlack">颜色。此处一定要注意，现在的颜色是反的！！！</param>
        private void RecordChessPieces(int number, bool isBlack)
        {
            if (number == -1)
            {
                return;
            }

            _recordChessPieces[number / PointNum, number % PointNum] = !isBlack ? 1 : 2;
        }

        private List<IntelligenceModel> Intelligence(int colorNumber)
        {
            List<IntelligenceModel> intelligenceList = new List<IntelligenceModel>();

            for (int i = 0; i < PointNum; i++)
            {
                for (int j = 0; j < PointNum; j++)
                {
                    if (_recordChessPieces[i, j] == 0)
                    {
                        int criticality = CalculateCriticality(i, j, colorNumber);

                        if (criticality > 0)
                        {
                            intelligenceList.Add(new IntelligenceModel()
                            {
                                Criticality = criticality,
                                PointNumber = i * PointNum + j
                            });
                        }
                    }
                }
            }

            if (intelligenceList.Count > 0)
            {
                return intelligenceList;
            }

            return null;
        }

        private int CalculateCriticality(int i, int j, int colorNumber, bool threeFlag = false)
        {
            int totalSameColorNumber = 0;
            int tempSameColorNumber = 0;
            int criticality = 0;
            bool headIsEnd = true;
            bool tailIsEnd = true;
            int twoEmptyPoint = 1;

            #region 纵向判断

            for (int k = i - 1; k >= 0; k--)
            {
                if (_recordChessPieces[k, j] != colorNumber)
                {
                    if (threeFlag)
                    {
                        if (_recordChessPieces[k, j] == 0)
                        {
                            headIsEnd = false;

                            if (twoEmptyPoint > 0)
                            {
                                twoEmptyPoint--;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (_recordChessPieces[k, j] == 0)
                        {
                            headIsEnd = false;
                        }

                        break;
                    }
                }
                else
                {
                    tempSameColorNumber = i - k - (1 - twoEmptyPoint);
                }
            }
            totalSameColorNumber += tempSameColorNumber;
            tempSameColorNumber = 0;
            twoEmptyPoint = 1;

            for (int k = i + 1; k < PointNum; k++)
            {
                if (_recordChessPieces[k, j] != colorNumber)
                {
                    if (threeFlag)
                    {
                        if (_recordChessPieces[k, j] == 0)
                        {
                            tailIsEnd = false;

                            if (twoEmptyPoint > 0)
                            {
                                twoEmptyPoint--;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (_recordChessPieces[k, j] == 0)
                        {
                            tailIsEnd = false;
                        }

                        break;
                    }
                }
                else
                {
                    tempSameColorNumber = k - i - (1 - twoEmptyPoint);
                }
            }
            totalSameColorNumber += tempSameColorNumber;

            int space = 0;

            for (int k = i - 1; k >= 0; k--)
            {
                if (_recordChessPieces[k, j] != colorNumber)
                {
                    space++;

                    if (space > 3)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            for (int k = i + 1; k < PointNum; k++)
            {
                if (_recordChessPieces[k, j] != colorNumber)
                {
                    space++;

                    if (space > 3)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            criticality = CalculateCriticality(totalSameColorNumber, criticality, space, headIsEnd, tailIsEnd);

            #endregion

            #region 左上到右下判断

            totalSameColorNumber = 0;
            tempSameColorNumber = 0;
            headIsEnd = true;
            tailIsEnd = true;
            twoEmptyPoint = 1;

            for (int k = 1; k < PointNum; k++)
            {
                if (i - k < 0 || j - k < 0)
                {
                    break;
                }

                if (_recordChessPieces[i - k, j - k] != colorNumber)
                {
                    if (threeFlag)
                    {
                        if (_recordChessPieces[i - k, j - k] == 0)
                        {
                            headIsEnd = false;

                            if (twoEmptyPoint > 0)
                            {
                                twoEmptyPoint--;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (_recordChessPieces[i - k, j - k] == 0)
                        {
                            headIsEnd = false;
                        }

                        break;
                    }
                }
                else
                {
                    tempSameColorNumber = k - (1 - twoEmptyPoint);
                }
            }
            totalSameColorNumber += tempSameColorNumber;
            tempSameColorNumber = 0;
            twoEmptyPoint = 1;

            for (int k = 1; k < PointNum; k++)
            {
                if (i + k >= PointNum || j + k >= PointNum)
                {
                    break;
                }

                if (_recordChessPieces[i + k, j + k] != colorNumber)
                {
                    if (threeFlag)
                    {
                        if (_recordChessPieces[i + k, j + k] == 0)
                        {
                            tailIsEnd = false;

                            if (twoEmptyPoint > 0)
                            {
                                twoEmptyPoint--;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (_recordChessPieces[i + k, j + k] == 0)
                        {
                            tailIsEnd = false;
                        }

                        break;
                    }
                }
                else
                {
                    tempSameColorNumber = k - (1 - twoEmptyPoint);
                }
            }
            totalSameColorNumber += tempSameColorNumber;

            space = 0;
            for (int k = 1; k < PointNum; k++)
            {
                if (i - k < 0 || j - k < 0)
                {
                    break;
                }

                if (_recordChessPieces[i - k, j - k] != colorNumber)
                {
                    space++;
                    if (space > 3)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            for (int k = 1; k < PointNum; k++)
            {
                if (i + k >= PointNum || j + k >= PointNum)
                {
                    break;
                }

                if (_recordChessPieces[i + k, j + k] != colorNumber)
                {
                    space++;
                    if (space > 3)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            criticality = CalculateCriticality(totalSameColorNumber, criticality, space, headIsEnd, tailIsEnd);

            #endregion

            #region 横向判断

            totalSameColorNumber = 0;
            tempSameColorNumber = 0;
            headIsEnd = true;
            tailIsEnd = true;
            twoEmptyPoint = 1;

            for (int k = j - 1; k >= 0; k--)
            {
                if (_recordChessPieces[i, k] != colorNumber)
                {
                    if (threeFlag)
                    {
                        if (_recordChessPieces[i, k] == 0)
                        {
                            headIsEnd = false;

                            if (twoEmptyPoint > 0)
                            {
                                twoEmptyPoint--;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (_recordChessPieces[i, k] == 0)
                        {
                            headIsEnd = false;
                        }

                        break;
                    }

                }
                else
                {
                    tempSameColorNumber = j - k - (1 - twoEmptyPoint);
                }
            }
            totalSameColorNumber += tempSameColorNumber;
            tempSameColorNumber = 0;
            twoEmptyPoint = 1;

            for (int k = j + 1; k < PointNum; k++)
            {
                if (_recordChessPieces[i, k] != colorNumber)
                {
                    if (threeFlag)
                    {
                        if (_recordChessPieces[i, k] == 0)
                        {
                            tailIsEnd = false;

                            if (twoEmptyPoint > 0)
                            {
                                twoEmptyPoint--;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (_recordChessPieces[i, k] == 0)
                        {
                            tailIsEnd = false;
                        }

                        break;
                    }
                }
                else
                {
                    tempSameColorNumber = k - j - (1 - twoEmptyPoint);
                }
            }
            totalSameColorNumber += tempSameColorNumber;

            space = 0;
            for (int k = j - 1; k >= 0; k--)
            {
                if (_recordChessPieces[i, k] != colorNumber)
                {
                    space++;
                    if (space > 3)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            for (int k = j + 1; k < PointNum; k++)
            {
                if (_recordChessPieces[i, k] != colorNumber)
                {
                    space++;
                    if (space > 3)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            criticality = CalculateCriticality(totalSameColorNumber, criticality, space, headIsEnd, tailIsEnd);

            #endregion

            #region 右上到左下判断

            totalSameColorNumber = 0;
            tempSameColorNumber = 0;
            headIsEnd = true;
            tailIsEnd = true;
            twoEmptyPoint = 1;

            for (int k = 1; k < PointNum; k++)
            {
                if (i - k < 0 || j + k >= PointNum)
                {
                    break;
                }

                if (_recordChessPieces[i - k, j + k] != colorNumber)
                {
                    if (threeFlag)
                    {
                        if (_recordChessPieces[i - k, j + k] == 0)
                        {
                            headIsEnd = false;

                            if (twoEmptyPoint > 0)
                            {
                                twoEmptyPoint--;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (_recordChessPieces[i - k, j + k] == 0)
                        {
                            headIsEnd = false;
                        }

                        break;
                    }
                }
                else
                {
                    tempSameColorNumber = k - (1 - twoEmptyPoint);
                }
            }
            totalSameColorNumber += tempSameColorNumber;
            tempSameColorNumber = 0;
            twoEmptyPoint = 1;

            for (int k = 1; k < PointNum; k++)
            {
                if (i + k >= PointNum || j - k < 0)
                {
                    break;
                }

                if (_recordChessPieces[i + k, j - k] != colorNumber)
                {
                    if (threeFlag)
                    {
                        if (_recordChessPieces[i + k, j - k] == 0)
                        {
                            tailIsEnd = false;

                            if (twoEmptyPoint > 0)
                            {
                                twoEmptyPoint--;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (_recordChessPieces[i + k, j - k] == 0)
                        {
                            tailIsEnd = false;
                        }

                        break;
                    }
                }
                else
                {
                    tempSameColorNumber = k - (1 - twoEmptyPoint);
                }
            }
            totalSameColorNumber += tempSameColorNumber;

            space = 0;
            for (int k = 1; k < PointNum; k++)
            {
                if (i - k < 0 || j + k >= PointNum)
                {
                    break;
                }

                if (_recordChessPieces[i - k, j + k] != colorNumber)
                {
                    space++;
                    if (space > 3)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            for (int k = 1; k < PointNum; k++)
            {
                if (i + k >= PointNum || j - k < 0)
                {
                    break;
                }

                if (_recordChessPieces[i + k, j - k] != colorNumber)
                {
                    space++;
                    if (space > 3)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            criticality = CalculateCriticality(totalSameColorNumber, criticality, space, headIsEnd, tailIsEnd);

            #endregion

            return criticality;
        }

        private int CalculateCriticality(int totalSameColorNumber, int criticality, int space, bool headIsEnd, bool tailIsEnd)
        {
            if (totalSameColorNumber >= 4)
            {
                criticality += 5461;
            }
            else if (totalSameColorNumber == 3)
            {
                if (!headIsEnd && !tailIsEnd)
                {
                    criticality += 1365;
                }
                else if (!headIsEnd || !tailIsEnd)
                {
                    criticality += 341;
                }
            }
            else if (totalSameColorNumber == 2)
            {
                if (!headIsEnd && !tailIsEnd)
                {
                    criticality += 85;
                }
                else if (!headIsEnd || !tailIsEnd)
                {
                    if (space > 3)
                    {
                        criticality += 21;
                    }
                }
            }
            else if (totalSameColorNumber == 1)
            {
                if (space > 3)
                {
                    if (!headIsEnd && !tailIsEnd)
                    {
                        criticality += 5;
                    }
                    else if (!headIsEnd || !tailIsEnd)
                    {
                        criticality += 1;
                    }
                }
            }

            return criticality;
        }

        private void PlayChess()
        {
            List<IntelligenceModel> proactiveIntelligenceList = Intelligence(2);
            if (proactiveIntelligenceList != null)
            {
                #region 如果能一步致胜，那么就下这步棋

                int maxCriticality = proactiveIntelligenceList.Select(p => p.Criticality).Max();

                if (maxCriticality >= 5461)
                {
                    var victoryPointList = (from p in proactiveIntelligenceList
                                            where p.Criticality >= maxCriticality
                                            select p).ToList();

                    if (victoryPointList.Count > 0)
                    {
                        Random ran = new Random();
                        var victoryPoint = victoryPointList[ran.Next(0, victoryPointList.Count)];

                        AddAChessPieces(_pointList[victoryPoint.PointNumber].X, _pointList[victoryPoint.PointNumber].Y);
                        RecordChessPieces(victoryPoint.PointNumber, _isBlack);

                        return;
                    }
                }

                #endregion

                #region 不能一步致胜，那么就要优先主动防守

                List<IntelligenceModel> intelligenceList = Intelligence(1);

                if (intelligenceList != null)
                {
                    maxCriticality = intelligenceList.Select(p => p.Criticality).Max();

                    if (maxCriticality >= 5461)
                    {
                        var veryDengerousPointList = (from p in intelligenceList
                                                      where p.Criticality >= maxCriticality
                                                      select p).ToList();

                        if (veryDengerousPointList.Count > 0)
                        {
                            List<int> recordCriticalityList = new List<int>();
                            foreach (var model in veryDengerousPointList)
                            {
                                recordCriticalityList.Add(CalculateCriticality(model.PointNumber / PointNum, model.PointNumber % PointNum, 2));
                            }
                            int bestNumber = recordCriticalityList.IndexOf(recordCriticalityList.Max());

                            var veryDengerousPoint = veryDengerousPointList[bestNumber];

                            AddAChessPieces(_pointList[veryDengerousPoint.PointNumber].X, _pointList[veryDengerousPoint.PointNumber].Y);
                            RecordChessPieces(veryDengerousPoint.PointNumber, _isBlack);

                            return;
                        }
                    }

                    //同级的应该优先自己走的那步棋
                    maxCriticality = proactiveIntelligenceList.Select(p => p.Criticality).Max();
                    if (maxCriticality >= 341)
                    {
                        var myDengerousPointList = (from p in proactiveIntelligenceList
                                                    where p.Criticality >= maxCriticality
                                                    select p).ToList();

                        if (myDengerousPointList.Count > 0)
                        {
                            Random ran = new Random();
                            var myDengerousPoint =
                                myDengerousPointList[ran.Next(0, myDengerousPointList.Count)];

                            AddAChessPieces(_pointList[myDengerousPoint.PointNumber].X, _pointList[myDengerousPoint.PointNumber].Y);
                            RecordChessPieces(myDengerousPoint.PointNumber, _isBlack);

                            return;
                        }
                    }


                    //自己没有这么优先的棋，那么就堵他的
                    maxCriticality = intelligenceList.Select(p => p.Criticality).Max();

                    if (maxCriticality >= 1365)
                    {
                        var dengerousPointList = (from p in intelligenceList
                                                  where p.Criticality >= maxCriticality
                                                  select p).ToList();

                        if (dengerousPointList.Count > 0)
                        {
                            List<int> recordCriticalityList = new List<int>();
                            foreach (var model in dengerousPointList)
                            {
                                recordCriticalityList.Add(CalculateCriticality(model.PointNumber / PointNum, model.PointNumber % PointNum, 2));
                            }
                            int bestNumber = recordCriticalityList.IndexOf(recordCriticalityList.Max());

                            var dengerousPoint = dengerousPointList[bestNumber];

                            AddAChessPieces(_pointList[dengerousPoint.PointNumber].X, _pointList[dengerousPoint.PointNumber].Y);
                            RecordChessPieces(dengerousPoint.PointNumber, _isBlack);

                            return;
                        }
                    }

                }

                #endregion

                #region 选择当前最优的Point
                maxCriticality = proactiveIntelligenceList.Select(p => p.Criticality).Max();

                var optimalPointList = (from p in proactiveIntelligenceList
                                        where p.Criticality >= maxCriticality
                                        select p).ToList();

                if (maxCriticality >= 85)
                {
                    List<int> recordCriticalityList = new List<int>();
                    foreach (var model in optimalPointList)
                    {
                        recordCriticalityList.Add(CalculateCriticality(model.PointNumber / PointNum, model.PointNumber % PointNum, 2, true));
                    }
                    int bestNumber = recordCriticalityList.IndexOf(recordCriticalityList.Max());

                    var optimalPoint = optimalPointList[bestNumber];

                    AddAChessPieces(_pointList[optimalPoint.PointNumber].X, _pointList[optimalPoint.PointNumber].Y);
                    RecordChessPieces(optimalPoint.PointNumber, _isBlack);
                }
                else
                {
                    if (optimalPointList.Count > 0)
                    {
                        Random ran = new Random();
                        var optimalPoint = optimalPointList[ran.Next(0, optimalPointList.Count)];

                        AddAChessPieces(_pointList[optimalPoint.PointNumber].X, _pointList[optimalPoint.PointNumber].Y);
                        RecordChessPieces(optimalPoint.PointNumber, _isBlack);
                    }
                }
                #endregion
            }
            else
            {
                #region 不能一步致胜，那么就要优先主动防守

                List<IntelligenceModel> intelligenceList = Intelligence(1);

                if (intelligenceList != null)
                {
                    int maxCriticality = intelligenceList.Select(p => p.Criticality).Max();

                    if (maxCriticality >= 5461)
                    {
                        var veryDengerousPointList = (from p in intelligenceList
                                                      where p.Criticality >= maxCriticality
                                                      select p).ToList();

                        if (veryDengerousPointList.Count > 0)
                        {
                            List<int> recordCriticalityList = new List<int>();
                            foreach (var model in veryDengerousPointList)
                            {
                                recordCriticalityList.Add(CalculateCriticality(model.PointNumber / PointNum, model.PointNumber % PointNum, 2));
                            }
                            int bestNumber = recordCriticalityList.IndexOf(recordCriticalityList.Max());

                            var veryDengerousPoint = veryDengerousPointList[bestNumber];

                            AddAChessPieces(_pointList[veryDengerousPoint.PointNumber].X, _pointList[veryDengerousPoint.PointNumber].Y);
                            RecordChessPieces(veryDengerousPoint.PointNumber, _isBlack);

                            return;
                        }
                    }

                    //自己没有这么优先的棋，那么就堵他的
                    maxCriticality = intelligenceList.Select(p => p.Criticality).Max();

                    if (maxCriticality >= 1365)
                    {
                        var dengerousPointList = (from p in intelligenceList
                                                  where p.Criticality >= maxCriticality
                                                  select p).ToList();

                        if (dengerousPointList.Count > 0)
                        {
                            List<int> recordCriticalityList = new List<int>();
                            foreach (var model in dengerousPointList)
                            {
                                recordCriticalityList.Add(CalculateCriticality(model.PointNumber / PointNum, model.PointNumber % PointNum, 2));
                            }
                            int bestNumber = recordCriticalityList.IndexOf(recordCriticalityList.Max());

                            var dengerousPoint = dengerousPointList[bestNumber];

                            AddAChessPieces(_pointList[dengerousPoint.PointNumber].X, _pointList[dengerousPoint.PointNumber].Y);
                            RecordChessPieces(dengerousPoint.PointNumber, _isBlack);

                            return;
                        }
                    }

                }

                #endregion

                #region 初始的一步棋

                if (_recordChessPieces[(PointNum * PointNum / 2) / PointNum, (PointNum * PointNum / 2) % PointNum] == 0)
                {
                    AddAChessPieces(_pointList[PointNum * PointNum / 2].X, _pointList[PointNum * PointNum / 2].Y);
                    RecordChessPieces(PointNum * PointNum / 2, _isBlack);
                }
                else
                {
                    while (true)
                    {
                        Random random = new Random();
                        int i = random.Next(6, 9);
                        random = new Random();
                        int j = random.Next(6, 9);

                        if (_recordChessPieces[i, j] == 0)
                        {
                            AddAChessPieces(_pointList[i * PointNum + j].X, _pointList[i * PointNum + j].Y);
                            RecordChessPieces(i * PointNum + j, _isBlack);

                            break;
                        }
                    }
                }

                #endregion
            }
        }

        private void DrawWinLine(int startNum, int endNum)
        {
            Line line = new Line()
            {
                X1 = _pointList[startNum].X,
                Y1 = _pointList[startNum].Y,
                X2 = _pointList[endNum].X,
                Y2 = _pointList[endNum].Y,
                Stroke = Brushes.Red,
                StrokeThickness = 3
            };
            ChessboardCanvas.Children.Add(line);
            _winLine = line;
        }

        private bool Judgment()
        {
            for (int i = 0; i < PointNum; i++)
            {
                for (int j = 0; j < PointNum; j++)
                {
                    if (_recordChessPieces[i, j] != 0)
                    {
                        int colorNumber = _recordChessPieces[i, j];

                        #region 向右判断

                        for (int tempJ = j + 1; tempJ < PointNum; tempJ++)
                        {
                            if (tempJ - j > 4)
                            {
                                DrawWinLine(i * PointNum + j, i * PointNum + --tempJ);
                                return true;
                            }

                            if (_recordChessPieces[i, tempJ] != colorNumber)
                            {
                                break;
                            }
                        }

                        #endregion

                        #region 向左下判断

                        for (int k = 1; k < PointNum; k++)
                        {
                            if (k >= 5)
                            {
                                DrawWinLine(i * PointNum + j, (i + k - 1) * PointNum + j - k + 1);
                                return true;
                            }

                            if (i + k >= PointNum || j - k < 0)
                            {
                                break;
                            }

                            if (_recordChessPieces[i + k, j - k] != colorNumber)
                            {
                                break;
                            }
                        }

                        #endregion

                        #region 向下判断

                        for (int tempI = i + 1; tempI < PointNum; tempI++)
                        {
                            if (tempI - i > 4)
                            {
                                DrawWinLine(i * PointNum + j, --tempI * PointNum + j);
                                return true;
                            }

                            if (_recordChessPieces[tempI, j] != colorNumber)
                            {
                                break;
                            }
                        }

                        #endregion

                        #region 向右下判断

                        for (int k = 1; k < PointNum; k++)
                        {
                            if (k >= 5)
                            {
                                DrawWinLine(i * PointNum + j, (i + k - 1) * PointNum + j + k - 1);
                                return true;
                            }

                            if (i + k >= PointNum || j + k >= PointNum)
                            {
                                break;
                            }

                            if (_recordChessPieces[i + k, j + k] != colorNumber)
                            {
                                break;
                            }
                        }

                        #endregion
                    }
                }
            }

            return false;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("确定要重新开始吗？", "系统提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbr != MessageBoxResult.Yes)
            {
                return;
            }

            ClearChessboard();
        }
    }//End public class
}