using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect_3._1
{
    class Spot
    {
        public int row;
        public int col;
        public int score;

        public Spot()
        {
            row = -1;
            col = -1;
            score = 0;
        }
    }

    class AI_Player
    {
        public Spot GetBestMove(Game G, int difficulty)
        {
            Spot s = Minimax(G, 0, difficulty, G.getTurn());
            return s;
        }

        private Spot Minimax(Game G, int level, int maxLevel, char turn)
        {
            Spot BestSpot = new Spot();

            //if current AI player
            if (level % 2 == 0)
            {
                BestSpot.score = -1000000; // Negative infinity. Trying to maximize this score.
            }
            //if AI opponent
            else
            {
                BestSpot.score = 1000000; // Positive infinity. Trying to minimize this score.
            }

            //if there is a winner or we are as deep as we are allowed to go
            if (G.checkWinner().Item1 != ' ' || G.BoardIsFull() || level >= maxLevel)
            {
                BestSpot.score = scoreGame(G, level + 1, turn);
                //MessageBox.Show(string.Format("Score is {0}, level is {1}", BestSpot.score, level));
            }
            else
            {
                //note that consts in C# are static, meaning that to access 
                //them you should use the class name
                for (int i = 0; i < Game.ROWS; i++)
                {
                    for (int j = 0; j < Game.COLS; j++)
                    {
                        if (G.CellIsPlayable(i, j))
                        {
                            Game TmpGame = new Game(G); // Create virtual games and pick the best one.

                            TmpGame.MarkGameBoard(i, j);

                            Spot TmpSpot = new Spot();
                            TmpSpot.row = i;
                            TmpSpot.col = j;

                            //if (TmpGame.checkWinner().Item1 != ' ')
                            //{
                            TmpSpot.score = Minimax(TmpGame, level + 1, maxLevel, turn).score;

                            //MessageBox.Show(string.Format("Score is {0}, level is {1}", TmpSpot.score, level));
                            //if current AI player
                            if (level % 2 == 0)
                            {
                                if (TmpSpot.score > BestSpot.score)
                                {
                                    BestSpot = TmpSpot;
                                }
                            }
                            //if AI opponent
                            else
                            {
                                if (TmpSpot.score < BestSpot.score)
                                {
                                    BestSpot = TmpSpot;
                                }
                            }
                            //}
                        }
                    }
                }
            }
            //MessageBox.Show(string.Format("Score is {0}, level is {1}", BestSpot.score, level));
            return BestSpot;
        }
        private int scoreGame(Game G, int level, char turn)
        {
            int score = 0;

            score += ScoreCombinationL(G, turn) / (level * level);
            //score += ScoreTwoInRowOpen(G, turn) / (level * level);
            score += ScoreThreeInRow(G, turn) / (level * level);

            return score;
        }

        private int ScoreCombinationL(Game G, char turn)
        {
            int score = 0;
            char checkWinner = G.checkWinner().Item1;
            if (checkWinner != ' ')
            {
                if (checkWinner == turn)
                {
                    score += 10000; // Arbitrary numbers. Same for the one below.
                }
                else
                {
                    score -= 10000;
                }
            }
            return score;
        }

        private int ScoreThreeInRow(Game G, char turn)
        {
            int score = 0;

            //check horizontal
            for (int i = 0; i < Game.ROWS - 1; i++)
            {
                for (int j = 0; j < Game.COLS - 1; j++)
                {
                    // Check three in a row horizontally
                    if (j + 2 <= 6 && i + 1 <= 5)
                    {
                        if ((((G.getGameBoard()[i, j] == G.getGameBoard()[i, j + 1] && G.getGameBoard()[i, j] == G.getGameBoard()[i, j + 2]) && G.getGameBoard()[i + 1, j] == ' ')
                        || (G.getGameBoard()[i, j] == G.getGameBoard()[i, j + 1] && G.getGameBoard()[i, j] == G.getGameBoard()[i, j + 2]) && G.getGameBoard()[i + 1, j + 1] == ' ')
                        || (G.getGameBoard()[i, j] == G.getGameBoard()[i, j + 1] && G.getGameBoard()[i, j] == G.getGameBoard()[i, j + 2]) && G.getGameBoard()[i + 1, j + 2] == ' ')
                        {
                            if (G.getGameBoard()[i, j] == turn || G.getGameBoard()[i, j + 1] == turn || G.getGameBoard()[i, j + 2] == turn)
                            {
                                score += 1000;
                            }
                            else
                            {
                                score -= 1000;
                            }
                        }
                    }
                }
            }

            //check vertical
            for (int i = 0; i < Game.ROWS - 1; i++)
            {
                for (int j = 0; j < Game.COLS - 1; j++)
                {
                    // Check three in a row vertically
                    if (j - 1 >= 0 && j + 1 <= 6 && i + 2 <= 5)
                    {
                        if (((((((G.getGameBoard()[i, j] == G.getGameBoard()[i + 1, j] && G.getGameBoard()[i, j] == G.getGameBoard()[i + 2, j]) && G.getGameBoard()[i, j + 1] == ' ')
                          || (G.getGameBoard()[i, j] == G.getGameBoard()[i + 1, j] && G.getGameBoard()[i, j] == G.getGameBoard()[i + 2, j]) && G.getGameBoard()[i, j - 1] == ' ')
                          || (G.getGameBoard()[i, j] == G.getGameBoard()[i + 1, j] && G.getGameBoard()[i, j] == G.getGameBoard()[i + 2, j]) && G.getGameBoard()[i + 1, j + 1] == ' ')
                          || (G.getGameBoard()[i, j] == G.getGameBoard()[i + 1, j] && G.getGameBoard()[i, j] == G.getGameBoard()[i + 2, j]) && G.getGameBoard()[i + 1, j - 1] == ' ')
                          || (G.getGameBoard()[i, j] == G.getGameBoard()[i + 1, j] && G.getGameBoard()[i, j] == G.getGameBoard()[i + 2, j]) && G.getGameBoard()[i + 2, j + 1] == ' ')
                          || (G.getGameBoard()[i, j] == G.getGameBoard()[i + 1, j] && G.getGameBoard()[i, j] == G.getGameBoard()[i + 2, j]) && G.getGameBoard()[i + 2, j - 1] == ' ')
                        {
                            if (G.getGameBoard()[i, j] == turn || G.getGameBoard()[i + 1, j] == turn || G.getGameBoard()[i + 2, j] == turn)
                            {
                                score += 1000;
                            }
                            else
                            {
                                score -= 1000;
                            }
                        }
                    }
                }
            }

            return score;
        }

        //private int ScoreTwoInRowOpen(Game G, char turn)
        //{
        //    int score = 0;

        //    //check horizontal
        //    for (int i = 0; i < Game.ROWS - 1; i++)
        //    {
        //        for (int j = 0; j < Game.COLS - 1; j++)
        //        {
        //            // Check horizontal, if there are two coins next to each other with empty slot to the left or right
        //            if (j - 1 >= 0 && j + 2 <= 6)
        //            {
        //                if ((G.getGameBoard()[i, j] == G.getGameBoard()[i, j + 1] && G.getGameBoard()[i, j + 2] == ' ')
        //                || ((G.getGameBoard()[i, j] == G.getGameBoard()[i, j + 1] && G.getGameBoard()[i, j - 1] == ' ')))
        //                {
        //                    if (G.getGameBoard()[i, j] == turn || G.getGameBoard()[i, j + 1] == turn)
        //                    {
        //                        score += 1000;
        //                    }
        //                    else
        //                    {
        //                        score -= 1000;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    // Check vertical, if there are two coins next to each other with empty slot to the left or right
        //    for (int i = 0; i < Game.ROWS - 1; i++)
        //    {
        //        for (int j = 0; j < Game.COLS - 1; j++)
        //        {
        //            // Check vertical, if there are two coins on top of each other with empty slot above
        //            if (i - 1 >= 0 && i + 2 <= 5 && j - 1 >= 0 && j + 1 <= 6)
        //            {
        //                if ((G.getGameBoard()[i, j] == G.getGameBoard()[i + 1, j] && (G.getGameBoard()[i + 2, j] == ' '))
        //                 || (G.getGameBoard()[i, j] == G.getGameBoard()[i + 1, j] && (G.getGameBoard()[i, j + 1] == ' ')
        //                 || (G.getGameBoard()[i, j] == G.getGameBoard()[i + 1, j] && (G.getGameBoard()[i, j - 1] == ' ')
        //                 || (G.getGameBoard()[i, j] == G.getGameBoard()[i + 1, j] && (G.getGameBoard()[i + 1, j + 1] == ' ')
        //                 || (G.getGameBoard()[i, j] == G.getGameBoard()[i + 1, j] && (G.getGameBoard()[i + 1, j - 1] == ' '))))))
        //                {
        //                    if (G.getGameBoard()[i, j] == turn || G.getGameBoard()[i + 1, j] == turn)
        //                    {
        //                        score += 1000;
        //                    }
        //                    else
        //                    {
        //                        score -= 1000;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return score;
        //}
    }
}

