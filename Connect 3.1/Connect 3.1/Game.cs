using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect_3._1
{
    class Game
    {
        public const int ROWS = 6;
        public const int COLS = 7;
        private int numFilledCells;
        private char[,] GameBoard;

        public char turn { get; set; }

        // Initialize all possible winning combinations.
        char[][,] Combinations = new char[8][,];

        char[,] Combination1 = new char[2, 3] { {'*', '*', '*'},
                                                {'*', ' ', ' '} };

        char[,] Combination2 = new char[2, 3] { { '*', '*', '*' },
                                                { ' ', ' ', '*' } };

        char[,] Combination3 = new char[2, 3] { { ' ', ' ', '*' },
                                                { '*', '*', '*' } };

        char[,] Combination4 = new char[2, 3] { { '*', ' ', ' ' },
                                                { '*', '*', '*' } };

        char[,] Combination5 = new char[3, 2] { { '*', ' ' },
                                                { '*', ' ' },
                                                { '*', '*' } };

        char[,] Combination6 = new char[3, 2] { { '*', '*' },
                                                { '*', ' ' },
                                                { '*', ' ' } };

        char[,] Combination7 = new char[3, 2] { { ' ', '*' },
                                                { ' ', '*' },
                                                { '*', '*' } };

        char[,] Combination8 = new char[3, 2] { { '*', '*' },
                                                { ' ', '*' },
                                                { ' ', '*' } };

        public Game(char t = 'X')
        {
            GameBoard = new char[ROWS, COLS];

            turn = t;
            numFilledCells = 0;

            //initialize each cell to empty
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLS; j++)
                {
                    GameBoard[i, j] = ' ';
                }
            }

            InitCombinations();
        }

        public Game(Game G)
        {
            GameBoard = new char[ROWS, COLS];

            turn = G.turn;
            numFilledCells = G.numFilledCells;

            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLS; j++)
                {
                    GameBoard[i, j] = G.GameBoard[i, j];
                }
            }
            InitCombinations();
        }

        public char getTurn()
        {
            return turn;
        }

        private void InitCombinations()
        {
            Combinations[0] = Combination1;
            Combinations[1] = Combination2;
            Combinations[2] = Combination3;
            Combinations[3] = Combination4;
            Combinations[4] = Combination5;
            Combinations[5] = Combination6;
            Combinations[6] = Combination7;
            Combinations[7] = Combination8;
        }

        public bool CellIsEmpty(int row, int col)
        {
            return (GameBoard[row, col] == ' ');
        }

        // Function that determines which cells are playable,
        // so that AI only makes valid moves.
        public bool CellIsPlayable(int row, int col)
        {
            bool isCellPlayable = false;

            if (row == 0)
            {
                if (GameBoard[row, col] == ' ')
                {
                    isCellPlayable = true;
                }
            }
            else
            {
                if (GameBoard[row, col] == ' ' && GameBoard[row - 1, col] != ' ')
                {
                    isCellPlayable = true;
                }
            }

            return isCellPlayable;
        }

        public void MarkGameBoard(int row, int col)
        {

            if (!CellIsEmpty(row, col))
            {
                return;
            }
            GameBoard[row, col] = turn;
            if (turn == 'X')
            {
                turn = 'O';
            }
            else
            {
                turn = 'X';
            }
            numFilledCells++;
        }

        public bool BoardIsFull()
        {
            return numFilledCells >= ROWS * COLS;
        }

        public char[,] getGameBoard()
        {
            return GameBoard;
        }

        public Tuple<char, List<Tuple<int, int>>> checkWinner()
        {
            List<Tuple<int, int>> winningSpots = new List<Tuple<int, int>>();

            char lastTurn;
            if (turn == 'X')
            {
                lastTurn = 'O';
            }
            else
            {
                lastTurn = 'X';
            }

            for (int ii = 0; ii < Combinations.GetLength(0); ii++)
            {
                char[,] combination = Combinations[ii];

                int combinationRows = combination.GetLength(0);
                int combinationColumns = combination.GetLength(1);

                for (int y = 0; y < ROWS - combinationRows + 1; y++)
                {
                    for (int x = 0; x < COLS - combinationColumns + 1; x++)
                    {
                        winningSpots.Clear();

                        for (int i = 0; i < combinationRows; i++)
                        {
                            for (int j = 0; j < combinationColumns; j++)
                            {
                                if (GameBoard[y + i, x + j] == lastTurn && combination[i, j] == '*')
                                {
                                    winningSpots.Add(Tuple.Create(y + i, x + j));
                                }
                            }
                        }

                        if (winningSpots.Count == 4)
                        {
                            return Tuple.Create(lastTurn, winningSpots);
                        }
                    }
                }
            }
            return Tuple.Create(' ', winningSpots);
        }
    }
}
