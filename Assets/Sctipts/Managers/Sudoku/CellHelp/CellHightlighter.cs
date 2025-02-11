using Game.Classes;
using Help.UI;
using UnityEngine.UI;

namespace Game.Managers.Help
{
    public class CellHightlighter
    {
        #region CORE

        public enum CellHighlightType
        {
            Unselected,
            Highlighted,
            MainFocused,
            MinorFocused,
            Blocking,
            Wrong,
            Right
        }

        #endregion

        #region CORE LOGIC

        public void HighlightCell(CellManager cellManager, CellHighlightType highlightType)
        {
            CellColors cellColors = cellManager.Cell.CellColors;
            Image image = cellManager.Image;
            Text text = cellManager.Text;

            switch (highlightType)
            {
                case CellHighlightType.Unselected:
                    image.SetColor(cellColors.Unselected);
                    break;
                case CellHighlightType.Highlighted:
                    image.SetColor(cellColors.Selected);
                    break;
                case CellHighlightType.MainFocused:
                    image.SetColor(cellColors.MainFocused);
                    break;
                case CellHighlightType.MinorFocused:
                    image.SetColor(cellColors.MinorFocused);
                    break;
                case CellHighlightType.Blocking:
                    image.SetColor(cellColors.Blocking);
                    break;
                case CellHighlightType.Right:
                    text.SetColor(cellColors.RightText);
                    break;
                case CellHighlightType.Wrong:
                    text.SetColor(cellColors.WrongText);
                    break;
            }
        }

        #endregion

        #region SELECT

        public void Select(CellManager cellManager)
        {
            Cell cell = cellManager.Cell;
            CellGroups cellGroups = cell.CellGroups;

            UnselectAll(cellGroups);
            SelectBlock(cellGroups);
            SelectByLineX(cellGroups);
            SelectByLineY(cellGroups);
            SelectWithSameValues(cellGroups, cell.Value);

            HighlightCell(cellManager, CellHighlightType.MainFocused);
        }

        public void UnselectAll(CellGroups cellGroups) => HighlightCells(cellGroups.All, CellHighlightType.Unselected);
        public void UnselectAllLikeBlocker(CellGroups cellGroups) => HighlightCells(cellGroups.All, CellHighlightType.Blocking);
        public void SelectBlock(CellGroups cellGroups) => HighlightCells(cellGroups.Block, CellHighlightType.Highlighted);
        public void SelectByLineX(CellGroups cellGroups) => HighlightCells(cellGroups.LineX, CellHighlightType.Highlighted);
        public void SelectByLineY(CellGroups cellGroups) => HighlightCells(cellGroups.LineY, CellHighlightType.Highlighted);

        public void SelectWithSameValues(CellGroups cellGroups, int value)
        {
            if (value != 0)
            {
                foreach (var cellManager in cellGroups.All)
                {
                    Cell cell = cellManager.Cell;
                    if (cell.Value == value)
                        HighlightCell(cellManager, CellHighlightType.MinorFocused);
                }
            }
        }

        private void HighlightCells(CellManager[] cellManagers, CellHighlightType cellHighlightType)
        {
            foreach (var cellManager in cellManagers)
                HighlightCell(cellManager, cellHighlightType);
        }

        #endregion
    }
}