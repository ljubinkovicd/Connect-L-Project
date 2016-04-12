/*
             Programmer: Djordje Ljubinkovic
             Instructor: Jordan Ringenberg
                   Date: 12/07/2015
    Program Description: Connect four game that looks for "L" shapes on the board to determine the winner,
                         instead of four tokens in a row. Includes an option to play an AI player.
 */

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

using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Threading;

namespace Connect_3._1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Game TTT_Game = new Game();
        int player1Wins = 0;
        int player2Wins = 0;
        int ties = 0;

        bool PlayVsAi = true;
        AI_Player AiPlayer = new AI_Player();

        public MainWindow()
        {
            InitializeComponent();
            DrawGridlines();
            InitOpponents();
        }

        void InitOpponents()
        {
            PlayVsAi = true;
            glidSlider.Visibility = System.Windows.Visibility.Visible;
            txtPlayer2.Text = "Computer";
            txtPlayer2.IsEnabled = false;
        }

        void DrawGridlines()
        {
            for (int i = 0; i <= 5; i++)
            {
                for (int j = 0; j <= 6; j++)
                {
                    Border b = new Border();
                    b.BorderBrush = Brushes.Black;
                    b.BorderThickness = new Thickness(1);

                    PlayGrid.Children.Add(b);
                    Grid.SetRow(b, i);
                    Grid.SetColumn(b, j);
                }
            }
        }

        Color GetPlayerColor(char player)
        {
            if (player == 'X')
            {
                return Color.FromRgb(203, 0, 0);  // Red
            }
            else
            {
                return Color.FromRgb(0, 0, 142);  // Blue
            }
        }

        void DrawO(int row, int col)
        {
            Draw(row, col, GetPlayerColor('O'));
        }

        void DrawX(int row, int col)
        {
            Draw(row, col, GetPlayerColor('X'));
        }

        void Draw(int row, int col, Color color)
        {
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            Ellipse ellipse = new Ellipse();
            mySolidColorBrush.Color = color;
            ellipse.Stroke = Brushes.Black;
            ellipse.Height = PlayGrid.ActualHeight / 6;
            ellipse.Width = PlayGrid.ActualWidth / 7;

            ellipse.StrokeThickness = 1;
            ellipse.Fill = mySolidColorBrush;
            PlayGrid.Children.Add(ellipse);
            Grid.SetRow(ellipse, row);
            Grid.SetColumn(ellipse, col);
        }

        private void PlayGrid_MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            var point = Mouse.GetPosition(PlayGrid);

            int row = 0;
            int col = 0;
            double accumulatedHeight = 0.0;
            double accumulatedWidth = 0.0;

            // calc row mouse was over
            foreach (var rowDefinition in PlayGrid.RowDefinitions)
            {
                accumulatedHeight += rowDefinition.ActualHeight;
                if (accumulatedHeight >= point.Y)
                    break;
                row++;
            }

            // calc col mouse was over
            foreach (var columnDefinition in PlayGrid.ColumnDefinitions)
            {
                accumulatedWidth += columnDefinition.ActualWidth;
                if (accumulatedWidth >= point.X)
                    break;
                col++;
            }

            int rowIndexEmpty = -1;

            for (int i = Game.ROWS - 1; i >= 0; i--)
            {
                if (TTT_Game.CellIsEmpty(Game.ROWS - i - 1, col))
                {
                    rowIndexEmpty = i;
                    break;
                }
            }

            if (rowIndexEmpty != -1)
            {
                if (TTT_Game.turn == 'X')
                {

                    DrawX(rowIndexEmpty, col);
                    TTT_Game.MarkGameBoard(Game.ROWS - rowIndexEmpty - 1, col);

                }
                else if (TTT_Game.turn == 'O')
                {
                    DrawO(rowIndexEmpty, col);
                    TTT_Game.MarkGameBoard(Game.ROWS - rowIndexEmpty - 1, col);
                }
            }
            else
            {
                MessageBox.Show(string.Format("CANNOT PUT A TOKEN IN THAT COLUMN! IT'S NOT EMPTY!"));
                return;
            }

            Tuple<char, List<Tuple<int, int>>> winResult = TTT_Game.checkWinner();

            char winner = winResult.Item1;

            if (txtPlayer1.Text == "")
            {
                txtPlayer1.Text = "Player 1";
            }
            if (txtPlayer2.Text == "")
            {
                txtPlayer2.Text = "Player 2";
            }
            if (winner != ' ')
            {
                List<Tuple<int, int>> winningSpots = winResult.Item2;

                string winnerName = "";
                if (winner == 'X')
                {
                    winnerName = txtPlayer1.Text;
                    player1Wins++;
                    lblPlayer1Wins.Content = player1Wins.ToString();
                }
                else
                {
                    winnerName = txtPlayer2.Text;
                    player2Wins++;
                    lblPlayer2Wins.Content = player2Wins.ToString();
                }
                string message = string.Format("Winner is {0}!", winnerName);
                EndGame(message, winResult);
            }
            else if (TTT_Game.BoardIsFull())
            {
                ties++;
                lblTie.Content = ties.ToString();
                string message = string.Format("It's a TIE!");
                EndGame(message, null);
            }
            else
            {
                if (PlayVsAi)
                {
                    int difficulty = (int)sliderDifficulty.Value;
                    Game G = new Game(TTT_Game);
                    Spot spot = AiPlayer.GetBestMove(G, difficulty);

                    int r = spot.row;
                    int c = spot.col;

                    TTT_Game.MarkGameBoard(r, c);
                    Draw(Game.ROWS - r - 1, c, GetPlayerColor('O'));

                    winResult = TTT_Game.checkWinner();

                    if (TTT_Game.checkWinner().Item1 != ' ')
                    {
                        string message = string.Format("Winner is {0}!", txtPlayer2.Text);
                        EndGame(message, winResult);
                    }
                    else if (TTT_Game.BoardIsFull())
                    {
                        string message = string.Format("It's a TIE!");
                        EndGame(message, null);
                    }
                }
            }
        }

        private void EndGame(string message, Tuple<char, List<Tuple<int, int>>> winresult)
        {
           
            if (winresult != null)
            {
                List<Tuple<int, int>> winningSpots = winresult.Item2;
                char player = winresult.Item1;

                foreach (var spot in winningSpots)
                {

                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                    Ellipse ellipse = new Ellipse();
                    mySolidColorBrush.Color = GetPlayerColor(player);
                    ellipse.Stroke = Brushes.Black;
                    ellipse.Height = PlayGrid.ActualHeight / 6;
                    ellipse.Width = PlayGrid.ActualWidth / 7;

                    ellipse.Stroke = Brushes.LightYellow;
                    ellipse.StrokeThickness = 7;
                    ellipse.Fill = mySolidColorBrush;
                    PlayGrid.Children.Add(ellipse);
                    Grid.SetRow(ellipse, Game.ROWS - spot.Item1 - 1);
                    Grid.SetColumn(ellipse, spot.Item2);
                }
            }
            MessageBox.Show(message);
            reset();
        }

        void reset()
        {
            PlayGrid.Children.Clear();
            DrawGridlines();
            if (PlayVsAi)
            {
                TTT_Game = new Game('X');
            }
            else
            {
                TTT_Game = new Game(TTT_Game.turn);
            }
        }

        void resetAll()
        {
            reset();
            lblPlayer1Wins.Content = "0";
            lblPlayer2Wins.Content = "0";
            lblTie.Content = "0";
            player1Wins = 0;
            player2Wins = 0;
            ties = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            resetAll();
        }

        private void ToggleButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (toggleBtn.IsChecked == true)
            {
                toggleBtn.Content = "Play vs AI(computer)";
                PlayVsAi = false;
                glidSlider.Visibility = System.Windows.Visibility.Hidden;
                txtPlayer2.Text = "Player 2";
                txtPlayer2.IsEnabled = true;
            }
            else
            {
                toggleBtn.Content = "Play vs human opponent";
                PlayVsAi = true;
                glidSlider.Visibility = System.Windows.Visibility.Visible;
                txtPlayer2.Text = "Computer";
                txtPlayer2.IsEnabled = false;
            }
        }
    }
}


