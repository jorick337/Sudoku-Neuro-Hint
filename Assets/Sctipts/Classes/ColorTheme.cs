using UnityEngine;

namespace Game.Classes
{
    [System.Serializable]
    public struct ColorTheme
    {
        #region CORE

        [Header("CORE")]
        public Color Text;
        public Color Background;

        [Header("Button")]
        public Color HighlightedButton;
        public Color PressedButton;
        public Color SelectedButton;
        public Color BackGroundButton;
        public Color TextButton;

        [Header("Outline")]
        public Color EffectColorOutline;

        #endregion

        #region SUDOKU

        [Header("SUDOKU")]
        public Color BackGroundImageGrid;

        [Header("GridBlock")]
        public Color SelectionImageGridBlock;

        [Header("GridCell")]
        public Color SelectedImageGridCell;
        public Color UnselectedImageGridCell;
        public Color MainFocusedImageGridCell;
        public Color MinorFocusedImageGridCell;
        public Color BlockingImageGridCell;

        [Header("GridCell - Text")]
        public Color RightTextGridCellColor;
        public Color WrongTextGridCellColor;

        #endregion
    }
}