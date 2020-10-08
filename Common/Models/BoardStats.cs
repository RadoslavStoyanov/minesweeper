using System.Collections.Generic;

namespace Common.Models
{
    public class BoardStats
    {
        public BoardStats()
        {
            GapsBetweenMines = new List<int>();
        }

        public BoardStats(int figuresSum, int figuresCount, int minesCount, int emptyCellsCount, int figureAndEmptyCellsCount, int rows, int columns)
            : this()
        {
            FiguresSum = figuresSum;
            FiguresCount = figuresCount;
            MinesCount = minesCount;
            EmptyCellsCount = emptyCellsCount;
            FigureAndEmptyCellsCount = figureAndEmptyCellsCount;
            //Rows = rows;
            //Columns = columns;
        }

        public BoardStats(int ones, int twos, int threes, int fours, int fives, int six, int sevens, int eights, int nines)
            : this()
        {
            Ones = ones;
            Twos = twos;
            Threes = threes;
            Fours = fours;
            Fives = fives;
            Six = six;
            Sevens = sevens;
            Eights = eights;
            Nines = nines;
        }

        public int FiguresSum { get; set; }
        public int FiguresCount { get; set; }
        public int MinesCount { get; set; }
        public int EmptyCellsCount { get; set; }
        public int FigureAndEmptyCellsCount { get; set; }
        //public int Rows { get; set; }
        //public int Columns { get; set; }
        public int Ones { get; set; }
        public int Twos { get; set; }
        public int Threes { get; set; }
        public int Fours { get; set; }
        public int Fives { get; set; }
        public int Six { get; set; }
        public int Sevens { get; set; }
        public int Eights { get; set; }
        public int Nines { get; set; }
        public IEnumerable<int> GapsBetweenMines { get; set; }
        public int SumOfGaps { get; set; }
    }
}
