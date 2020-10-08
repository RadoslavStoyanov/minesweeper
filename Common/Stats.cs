//using Common.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;

//namespace Common
//{
//    /// <summary>
//    /// Helper class which analyzes boards in terms of:
//    /// 
//    /// - dimensions - ok
//    /// - mines count - ok
//    /// - empty cells count - ok
//    /// - figures count - ok
//    /// - sum of figures - ok
//    /// - sum figures - ok
//    /// - sum of figure and empty cells count sum - ok
//    /// 
//    /// and if something else comes around.
//    /// This class can come in handy when generating new playgrounds.
//    /// </summary>
//    public static class Stats
//    {
//        public static BoardStats AnalyzeBoard(char[,] board)
//        {
//            if (board == null)
//            {
//                throw new ArgumentException("Board");
//            }

//            //var rows = GetDimensions(board);
//            //var cols = rows;

//            var figuresSum = FiguresSum(board);
//            var figuresCount = FiguresCount(board);
//            var minesCount = GetMinesCount(board);
//            var emptyCellsCount = GetEmptyCellsCount(board);
//            var figuresAndEmptyCellsCount = GetFigureAndEmptyCellsCount(board);

//            var stats = GetUniqueDigitsCount(board);
//            stats.FiguresSum = figuresSum;
//            stats.FiguresCount = figuresCount;
//            stats.MinesCount = minesCount;
//            stats.EmptyCellsCount = emptyCellsCount;
//            stats.FigureAndEmptyCellsCount = figuresAndEmptyCellsCount;
//            //stats.Rows = rows;
//            //stats.Columns = cols;
//            stats.GapsBetweenMines = GetGapsBetweenMines(board);
//            stats.SumOfGaps = stats.GapsBetweenMines.Sum();

//            return stats;
//        }

//        private static int GetDimensions(char[,] board)
//        {
//            if (board == null)
//            {
//                throw new ArgumentException("Board");
//            }

//            return (int)Math.Sqrt(board.Length);
//        }

//        private static int FiguresSum(char[,] board)
//        {
//            if (board == null)
//            {
//                throw new ArgumentException("Board");
//            }

//            int sumOfFigures = 0;

//            foreach (var c in board)
//            {
//                if (char.IsDigit(c))
//                {
//                    sumOfFigures += (int)char.GetNumericValue(c);
//                }
//            }

//            return sumOfFigures;
//        }
//        private static int FiguresCount(char[,] board)
//        {
//            if (board == null)
//            {
//                throw new ArgumentException("Board");
//            }

//            int numberOfFigures = 0;

//            foreach (var c in board)
//            {
//                if (char.IsDigit(c))
//                {
//                    numberOfFigures++;
//                }
//            }

//            return numberOfFigures;
//        }

//        private static int GetMinesCount(char[,] board)
//        {
//            if (board == null)
//            {
//                throw new ArgumentException("Board");
//            }

//            int numberOfMines = 0;

//            foreach (var c in board)
//            {
//                if (c == 'm')
//                {
//                    numberOfMines++;
//                }
//            }

//            return numberOfMines;
//        }

//        private static int GetEmptyCellsCount(char[,] board)
//        {
//            if (board == null)
//            {
//                throw new ArgumentException("Board");
//            }

//            int numberOfEmptyCells = 0;

//            foreach (var c in board)
//            {
//                if (c == 'e')
//                {
//                    numberOfEmptyCells++;
//                }
//            }

//            return numberOfEmptyCells;
//        }

//        private static int GetFigureAndEmptyCellsCount(char[,] board)
//        {
//            if (board == null)
//            {
//                throw new ArgumentException("Board");
//            }

//            int numberOfFigureAndEmptyCellsCount = 0;

//            foreach (var c in board)
//            {
//                if (c == 'e' || char.IsDigit(c))
//                {
//                    numberOfFigureAndEmptyCellsCount++;
//                }
//            }

//            return numberOfFigureAndEmptyCellsCount;
//        }

//        private static BoardStats GetUniqueDigitsCount(char[,] board)
//        {
//            if (board == null)
//            {
//                throw new ArgumentException("Board");
//            }
//            var stats = new BoardStats();

//            foreach (var c in board)
//            {
//                switch (c)
//                {
//                    case '1':
//                        {
//                            stats.Ones++;
//                        }
//                        break;
//                    case '2':
//                        {
//                            stats.Twos++;
//                        }
//                        break;
//                    case '3':
//                        {
//                            stats.Threes++;
//                        }
//                        break;
//                    case '4':
//                        {
//                            stats.Fours++;
//                        }
//                        break;
//                    case '5':
//                        {
//                            stats.Fives++;
//                        }
//                        break;
//                    case '6':
//                        {
//                            stats.Six++;
//                        }
//                        break;
//                    case '7':
//                        {
//                            stats.Sevens++;
//                        }
//                        break;
//                    case '8':
//                        {
//                            stats.Eights++;
//                        }
//                        break;
//                    case '9':
//                        {
//                            stats.Nines++;
//                        }
//                        break;
//                    default:
//                        continue;
//                }
//            }

//            return stats;
//        }

//        private static IEnumerable<int> GetGapsBetweenMines(char[,] board)
//        {
//            if (board == null)
//            {
//                throw new ArgumentException("Board");
//            }

//            var result = new List<int>();
//            var gap = 0;

//            foreach (var c in board)
//            {
//                if (c != 'm')
//                {
//                    gap++;
//                }
//                else
//                {
//                    result.Add(gap);
//                    gap = 0;
//                }
//            }

//            return result;
//        }
//    }
//}
