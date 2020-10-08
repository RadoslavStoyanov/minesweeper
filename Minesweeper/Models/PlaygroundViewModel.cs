using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minesweeper.Models
{
    public class PlaygroundViewModel
    {
        public int Row { get; set; }// height -> row
        public int Column { get; set; }  // width -> column 
        public int NumberOfMines { get; set; }
        public ElementViewModel[,] Board { get; set; }

        public PlaygroundViewModel(int rows, int cols, int numberOfMines)
        {
            Board = new ElementViewModel[rows, cols];
            NumberOfMines = numberOfMines;
            Row = rows;
            Column = cols;
        }
    }
}
