using System.Collections.Generic;
using System.Linq;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;

namespace TicTacToe.ViewModels
{
    public class GameBoard : SimpleViewModel
    {
        private CellType _nextMove = CellType.X;
        private bool _isGameOver;

        public IList<Cell> Cells { get; private set; }

        public bool IsGameOver
        {
            get { return _isGameOver; }
            private set
            {
                if (SetPropertyValue(ref _isGameOver, value))
                {
                    ResetBoard.RaiseCanExecuteChanged();
                }
            }
        }

        public void SelectMove(Cell cell)
        {
            cell.Type = _nextMove;
            _nextMove = _nextMove == CellType.X ? CellType.O : CellType.X;
            IsGameOver = CheckForWinner();
        }

        public IDelegateCommand ResetBoard { get; private set; }

        public GameBoard()
        {
            Cells = new List<Cell>(Enumerable.Range(0,9).Select(_ => new Cell(this)));
            ResetBoard = new DelegateCommand(OnResetBoard, () => IsGameOver);
        }

        /// <summary>
        /// Resets the board
        /// </summary>
        private void OnResetBoard()
        {
            foreach (var cell in Cells)
            {
                cell.Type = CellType.None;
                IsGameOver = false;
            }
        }

        /// <summary>
        /// Check the combinations of possible wins.
        /// </summary>
        /// <returns>True if we have a winner.</returns>
        private bool CheckForWinner()
        {
            int count;

            // Check rows
            for (int i = 0; i < 3; i++)
            {
                count = (Cells.Skip(i*3).Take(3).Sum(c => (int)c.Type));
                if (count == 3 || count == 0)
                    return true;
            }

            // Check columns
            for (int i = 0; i < 3; i++)
            {
                count = (int)Cells[i].Type + (int)Cells[i + 3].Type + (int)Cells[i + 6].Type;
                if (count == 3 || count == 0)
                    return true;
            }

            // Check diagonals
            count = (int) Cells[0].Type + (int) Cells[4].Type + (int) Cells[8].Type;
            if (count == 3 || count == 0)
                return true;

            count = (int) Cells[2].Type + (int) Cells[4].Type + (int) Cells[6].Type;
            if (count == 3 || count == 0)
                return true;

            return false;
        }
    }
}
