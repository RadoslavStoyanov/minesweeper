using Common;
using Common.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Minesweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minesweeper.Controllers
{
    public class HomeController : Controller
    {
        private IMemoryCache _cache;
        private readonly Random _randomNumberGenerator;

        public HomeController(IMemoryCache memoryCache)
        {
            _randomNumberGenerator = new Random(DateTime.Now.Millisecond / 23);
            _cache = memoryCache;
        }

        public IActionResult Index(DifficultyEnum difficulty)
        {
            var game = CreateGame(difficulty);
            CacheGame(game);
            ConsoleTrace.LogSuccess("Playground generated.");

            return View(game);
        }

        [HttpPost]
        public IActionResult CheckElement(int row, int column, bool hasBeenFlagged)
        {
            var game = GetCachedGame();

            if (game.IsDead)
            {
                return View("Index", game);
            }

            game.ResetAllFiguresAndEmptyTilesAreOpenedStatus();

            var element = game.Playground.Board[row, column];

            if (hasBeenFlagged) // add/remove flag to the current element
            {
                element.IsFlagged = !element.IsFlagged;
                CacheGame(game);

                return View("Index", game);
            }

            if (element.IsFlagged) // if the element was flagged we can't open it
            {
                return View("Index", game);
            }

            if (element.IsMine)
            {
                OpenMines(game.Playground.Board);
                game.ResetIsDeadStatus();
            }
            else if (element.Figure.HasValue && !element.IsFlagged) // if it is a digit and we haven't flagged it
            {
                OpenDigit(game.Playground.Board, row, column);
            }
            else if (element.IsEmpty && !element.IsFlagged)
            {
                OpenEmptySpacesRegion(game.Playground.Board, row, column);
            }

            CacheGame(game);

            return View("Index", game);
        }

        private void CacheGame(GameViewModel game)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));

            _cache.Set(Constants.CacheKeys.Game, game, cacheEntryOptions);
        }

        private GameViewModel GetCachedGame()
        {
            GameViewModel game = null;

            if (!_cache.TryGetValue(Constants.CacheKeys.Game, out game))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));

                _cache.Set(Constants.CacheKeys.Game, game, cacheEntryOptions);
            }

            return game;
        }

        private GameViewModel CreateGame(DifficultyEnum difficulty)
        {
            var game = new GameViewModel(difficulty);
            game.Playground.Board = GenerateBoard(game.Playground.Row, game.Playground.Column, game.Playground.NumberOfMines);

            return game;
        }

        private void OpenMines(ElementViewModel[,] playgroundArray)
        {
            for (int r = 0; r < playgroundArray.GetLength(0); r++)
            {
                for (int c = 0; c < playgroundArray.GetLength(1); c++)
                {
                    if (playgroundArray[r, c].IsMine)
                    {
                        playgroundArray[r, c].IsOpen = true;
                    }
                }
            }
        }

        private void OpenDigit(ElementViewModel[,] playgroundArray, int row, int column)
        {
            playgroundArray[row, column].IsOpen = true;
        }

        private void OpenEmptySpacesRegion(ElementViewModel[,] playgroundArray, int row, int col)
        {
            if ((col < 0) || (row < 0) ||
            (col >= playgroundArray.GetLength(1)) || (row >= playgroundArray.GetLength(0)))
            {
                // We are out of the labyrinth
                return;
            }

            if (playgroundArray[row, col].IsVisited)
            {
                return;
            }

            if (!playgroundArray[row, col].IsEmpty)
            {
                if (playgroundArray[row, col].Figure.HasValue)
                {
                    playgroundArray[row, col].IsVisited = true;
                    playgroundArray[row, col].IsOpen = true;
                }

                return;
            }

            playgroundArray[row, col].IsVisited = true;
            playgroundArray[row, col].IsOpen = true;

            OpenEmptySpacesRegion(playgroundArray, row, col - 1); // left
            OpenEmptySpacesRegion(playgroundArray, row - 1, col); // up
            OpenEmptySpacesRegion(playgroundArray, row, col + 1); // right
            OpenEmptySpacesRegion(playgroundArray, row + 1, col); // down

            OpenEmptySpacesRegion(playgroundArray, row - 1, col - 1); // top left
            OpenEmptySpacesRegion(playgroundArray, row - 1, col + 1); // top right
            OpenEmptySpacesRegion(playgroundArray, row + 1, col - 1); // bottom left
            OpenEmptySpacesRegion(playgroundArray, row + 1, col + 1); // bottom right
        }

        private ElementViewModel[,] GenerateBoard(int rows, int cols, int mines)
        {
            var boardBoundary = rows > cols ? cols : rows;
            var coordinates = GenerateMinesCoordinates(boardBoundary, mines);
            var board = InitializeBoard(rows, cols);
            
            board = PlaceMines(rows, cols, coordinates);

            try
            {
                FillBoardWithFigures(board, 0, 0);

                for (int r = 0; r < board.GetLength(0); r++)
                {
                    for (int c = 0; c < board.GetLength(1); c++)
                    {
                        board[r, c].IsVisited = false;
                    }
                }
            }
            catch (StackOverflowException soe)
            {
                Console.WriteLine(soe.Message);
            }

            return board;
        }

        private HashSet<Tuple<int, int>> GenerateMinesCoordinates(int boardBoundary, int mines)
        {
            var next = _randomNumberGenerator.Next(0, boardBoundary);
            var result = new HashSet<Tuple<int, int>>();

            while (mines > 0)
            {
                try
                {
                    var temp = next;
                    next = _randomNumberGenerator.Next(0, boardBoundary);

                    while (temp == next)
                    {
                        next = _randomNumberGenerator.Next(0, boardBoundary);
                    }

                    var coordinates = new Tuple<int, int>(temp, next);
                    var existingCoordinates = result.Any(c => c.Item1 == coordinates.Item1 && c.Item2 == coordinates.Item2);

                    try
                    {
                        if (existingCoordinates)
                        {
                            throw new ArgumentException("existing coordinates");
                        }
                    }
                    catch (ArgumentException ae)
                    {
                        ConsoleTrace.LogWarning(ae.Message);

                        continue;
                    }

                    result.Add(coordinates);

                    mines--;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }

        private ElementViewModel[,] InitializeBoard(int rows, int cols)
        {
            return new ElementViewModel[rows, cols];
        }

        private ElementViewModel[,] PlaceMines(int rows, int cols, HashSet<Tuple<int, int>> coordinates)
        {
            var board = new ElementViewModel[rows, cols];

            for (int r = 0; r < board.GetLength(0); r++)
            {
                for (int c = 0; c < board.GetLength(1); c++)
                {
                    board[r, c] = new ElementViewModel();

                    var pair = new Tuple<int, int>(r, c);

                    if (coordinates.Contains(pair))
                    {
                        board[r, c].IsMine = true;
                        coordinates.Remove(pair);
                    }
                }
            }

            return board;
        }

        private void FillBoardWithFigures(ElementViewModel[,] board, int row, int col)
        {
            if (row < 0 || row >= board.GetLength(0))
            {
                return;
            }

            if (col < 0 || col >= board.GetLength(1))
            {
                return;
            }

            if (board[row, col].IsVisited)
            {
                return;
            }

            if (!board[row, col].IsMine)
            {
                var numberOfEntries = new List<int>();
                var coordinatesCheckedForMines = new HashSet<Tuple<int, int>>();
                var mines = 0;

                if ((row == 0 || row == board.GetLength(0) - 1) && (col == 0 || col == board.GetLength(1) - 1))
                {
                    mines = CountMines(board, row, col, row, col, 3, 0, coordinatesCheckedForMines);
                }
                else if ((row == 0 || row == board.GetLength(0) - 1) && (col > 0 || col < board.GetLength(1) - 1))
                {
                    mines = CountMines(board, row, col, row, col, 5, 0, coordinatesCheckedForMines);
                }
                else if ((col == 0 || col == board.GetLength(1) - 1) && (row > 0 || row < board.GetLength(0) - 1))
                {
                    mines = CountMines(board, row, col, row, col, 5, 0, coordinatesCheckedForMines);
                }
                else if ((row > 0 && row < board.GetLength(0) - 1) && (col > 0 && col < board.GetLength(1) - 1))
                {
                    mines = CountMines(board, row, col, row, col, 8, 0, coordinatesCheckedForMines);
                }

                foreach (var coordinates in coordinatesCheckedForMines)
                {
                    board[coordinates.Item1, coordinates.Item2].HasBeenCheckedForMines = false;
                }

                if (mines > 0)
                {
                    board[row, col].Figure = mines;
                }
            }

            board[row, col].IsVisited = true;

            FillBoardWithFigures(board, row - 1, col); // up
            FillBoardWithFigures(board, row + 1, col); // down
            FillBoardWithFigures(board, row, col - 1); // left
            FillBoardWithFigures(board, row, col + 1); // right
        }

        private int CountMines(ElementViewModel[,] board,
                               int row,
                               int col,
                               int startRow,
                               int startCol,
                               int numberOfCellsToCheck,
                               int mines,
                               HashSet<Tuple<int, int>> coordinatesCheckedForMines)
        {
            while (numberOfCellsToCheck > 0)
            {
                // Check if we are inside the board
                if (row < 0 || row >= board.GetLength(0))
                {
                    return mines;
                }

                // Check if we are inside the board
                if (col < 0 || col >= board.GetLength(1))
                {
                    return mines;
                }

                // Making sure we are checking only tiles surrounding the current one.
                if (Math.Abs(row - startRow) > 1)
                {
                    return mines;
                }

                // Making sure we are checking only tiles surrounding the current one.
                if (Math.Abs(col - startCol) > 1)
                {
                    return mines;
                }

                if (board[row, col].HasBeenCheckedForMines)
                {
                    return mines;
                }

                if (row != startRow || col != startCol)
                {
                    if (board[row, col].IsMine)
                    {
                        mines++;
                    }

                    board[row, col].HasBeenCheckedForMines = true;
                    numberOfCellsToCheck--;
                    coordinatesCheckedForMines.Add(new Tuple<int, int>(row, col));
                }

                mines = CountMines(board, row - 1, col, startRow, startCol, numberOfCellsToCheck, mines, coordinatesCheckedForMines); // up
                mines = CountMines(board, row + 1, col, startRow, startCol, numberOfCellsToCheck, mines, coordinatesCheckedForMines); // down
                mines = CountMines(board, row, col - 1, startRow, startCol, numberOfCellsToCheck, mines, coordinatesCheckedForMines); // left
                mines = CountMines(board, row, col + 1, startRow, startCol, numberOfCellsToCheck, mines, coordinatesCheckedForMines); // right

                mines = CountMines(board, row - 1, col - 1, startRow, startCol, numberOfCellsToCheck, mines, coordinatesCheckedForMines); // top left
                mines = CountMines(board, row - 1, col + 1, startRow, startCol, numberOfCellsToCheck, mines, coordinatesCheckedForMines); // top right
                mines = CountMines(board, row + 1, col - 1, startRow, startCol, numberOfCellsToCheck, mines, coordinatesCheckedForMines); // bottom left
                mines = CountMines(board, row + 1, col + 1, startRow, startCol, numberOfCellsToCheck, mines, coordinatesCheckedForMines); // bottom right

                numberOfCellsToCheck--;
            }

            return mines;
        }
    }
}
