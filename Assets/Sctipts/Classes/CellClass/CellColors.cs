using UnityEngine;

namespace Game.Classes
{
    public struct CellColors
    {
        public Color Selected { get; private set; }
        public Color Unselected { get; private set; }
        public Color MainFocused { get; private set; }
        public Color MinorFocused { get; private set; }
        public Color RightText { get; private set; }
        public Color WrongText { get; private set; }
        public Color Blocking { get; private set; }

        public CellColors(ColorTheme colorTheme)
        {
            Selected = colorTheme.SelectedImageGridCell;
            Unselected = colorTheme.UnselectedImageGridCell;
            MainFocused = colorTheme.MainFocusedImageGridCell;
            MinorFocused = colorTheme.MinorFocusedImageGridCell;
            RightText = colorTheme.RightTextGridCellColor;
            WrongText = colorTheme.WrongTextGridCellColor;
            Blocking = colorTheme.BlockingImageGridCell;
        }
    }
}