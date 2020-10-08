using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minesweeper.Models
{
    public class ElementViewModel
    {
        public ElementViewModel()
        {
            IsVisited = false;
        }

        public ElementViewModel(int figure) : this()
        {
            Figure = figure;
        }
        public ElementViewModel(bool isMine) : this()
        {
            IsMine = isMine;
        }

        public bool IsFlagged { get; set; }
        public bool IsOpen { get; set; }
        public int? Figure { get; set; } // set is because of ajax call
        public bool IsMine { get; set; } // set is because of ajax call
        public bool IsVisited { get; set; }
        public bool HasBeenCheckedForMines { get; set; }
        public bool IsEmpty
        {
            get
            {
                return !IsMine && !Figure.HasValue;
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
