using System.Windows.Input;
using JulMar.Windows.Mvvm;

namespace TicTacToe.ViewModels
{
    public class Cell : SimpleViewModel
    {
        private readonly GameBoard _board;
        private CellType _type;
        private bool _isAvailable;

        public CellType Type
        {
            get { return _type; }
            set 
            { 
                SetPropertyValue(ref _type, value);
                RaisePropertyChanged(() => Display);
                RaisePropertyChanged(() => DisplayColor);
                IsAvailable = _type == CellType.None;
            }
        }

        public string Display
        {
            get
            {
                switch (Type)
                {
                    case CellType.X:
                        return "$";
                    case CellType.O:
                        return "₵";
                    default:
                        return string.Empty;
                }
            }
        }

        public string DisplayColor
        {
            get
            {
                return Type == CellType.O ? "Red" : "Green";
            }
        }

        public bool IsAvailable
        {
            get { return _isAvailable; }
            private set { SetPropertyValue(ref _isAvailable, value); }
        }

        public ICommand Select { get; private set; }

        public Cell(GameBoard board)
        {
            Type = CellType.None;
            _board = board;
            Select = new DelegateCommand(OnSelectPiece, () => IsAvailable);
        }

        private void OnSelectPiece()
        {
            _board.SelectMove(this);
        }
    }
}