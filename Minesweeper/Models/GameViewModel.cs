using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minesweeper.Models
{
    public class GameViewModel
    {
        private bool? isDead;
        private bool? allFiguresAndEmptyTilesAreOpened;

        private const int EASY_ROWS_COUNT = 9;
        private const int EASY_COLS_COUNT = 9;
        private const int EASY_MINES_COUNT = 10;

        private const int MEDIUM_ROWS_COUNT = 16;
        private const int MEDIUM_COLS_COUNT = 16;
        private const int MEDIUM_MINES_COUNT = 40;

        private const int HARD_ROWS_COUNT = 16;
        private const int HARD_COLS_COUNT = 30;
        private const int HARD_MINES_COUNT = 99;

        /// <summary>
        /// 1..9 - figure
        /// m - mine
        /// e - empty
        /// </summary>
        //public List<char[,]> PredefinedPlaygrounds { get; set; }
        public DifficultyEnum Difficulty { get; set; }
        public bool IsDead
        {
            get
            {
                if (!isDead.HasValue)
                {
                    for (int r = 0; r < this.Playground.Board.GetLength(0); r++)
                    {
                        for (int c = 0; c < this.Playground.Board.GetLength(1); c++)
                        {
                            var element = this.Playground.Board[r, c];

                            if (element.IsMine && element.IsOpen)
                            {
                                isDead = true;

                                return true;
                            }
                        }
                    }

                    isDead = false;

                    return false;
                }

                return isDead.Value;
            }
        }

        public bool AreAllMinesFlagged
        {
            get
            {
                foreach (var tile in Playground.Board)
                {
                    if (tile.IsMine)
                    {
                        if (tile.IsFlagged)
                        {
                            continue;
                        }

                        return false;
                    }
                }

                return true;
            }
        }

        public bool GameEnded
        {
            get
            {
                return IsDead || AreAllMinesFlagged || AllFiguresAndEmptyTilesAreOpened();
            }
        }

        public PlaygroundViewModel Playground { get; set; }

        public GameViewModel(DifficultyEnum difficulty)
        {
            Difficulty = difficulty;

            switch (Difficulty)
            {
                case DifficultyEnum.Easy:
                    {
                        Playground = new PlaygroundViewModel(EASY_ROWS_COUNT, EASY_COLS_COUNT, EASY_MINES_COUNT);
                    }
                    break;
                case DifficultyEnum.Medium:
                    {
                        Playground = new PlaygroundViewModel(MEDIUM_ROWS_COUNT, MEDIUM_COLS_COUNT, MEDIUM_MINES_COUNT);
                    }
                    break;
                case DifficultyEnum.Hard:
                    {
                        Playground = new PlaygroundViewModel(HARD_ROWS_COUNT, HARD_COLS_COUNT, HARD_MINES_COUNT);
                    }
                    break;
            }
        }

        public void ResetIsDeadStatus()
        {
            isDead = null;
        }

        private bool AllFiguresAndEmptyTilesAreOpened()
        {
            if (!allFiguresAndEmptyTilesAreOpened.HasValue)
            {
                foreach (var tile in Playground.Board)
                {
                    if (tile.IsEmpty || tile.Figure.HasValue)
                    {
                        if (tile.IsOpen)
                        {
                            continue;
                        }

                        allFiguresAndEmptyTilesAreOpened = false;

                        return false;
                    }
                }

                allFiguresAndEmptyTilesAreOpened = true;
            }

            return allFiguresAndEmptyTilesAreOpened.Value;
        }

        public void ResetAllFiguresAndEmptyTilesAreOpenedStatus()
        {
            allFiguresAndEmptyTilesAreOpened = null;
        }
    }
}
