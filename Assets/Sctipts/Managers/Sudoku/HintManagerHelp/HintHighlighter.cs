using System.Linq;
using Game.Classes;
using Unity.VisualScripting;

namespace Game.Managers.Help
{
    public class HintHighlighter
    {
        #region CORE LOGIC

        public void SelectBlockHint(Hint hint)
        {
            CellManager cellManager = hint.CellManager;
            cellManager.CellHightlighter.UnselectAllLikeBlocker(cellManager.Cell.CellGroups);
            cellManager.CellHightlighter.SelectBlock(cellManager.Cell.CellGroups);
            DisableTextsCellManagersWhereHaveValues(cellManager.Cell.CellGroups.Block);
        }

        public void SelectInterferingCells(Hint hint)
        {
            CellManager cellManager = hint.CellManager;
            GridManager gridManager = GridManager.Instance;

            CellManager[] interferingCellManagersLineX = gridManager.GetInterferingCells(cellManager.Cell.Block, hint.Value, true);
            CellManager[] interferingCellManagersLineY = gridManager.GetInterferingCells(cellManager.Cell.Block, hint.Value, false);
            CellManager[] allInterferingCellManagers = interferingCellManagersLineX.Concat(interferingCellManagersLineY).ToArray();

            foreach (var interferingCellManager in allInterferingCellManagers)
            {
                if (interferingCellManagersLineX.Contains(interferingCellManager))
                {
                    DisableTextsCellManagersWhereHaveValues(interferingCellManager.Cell.CellGroups.LineX);
                    interferingCellManager.CellHightlighter.SelectByLineX(interferingCellManager.Cell.CellGroups);
                }
                else
                {
                    DisableTextsCellManagersWhereHaveValues(interferingCellManager.Cell.CellGroups.LineY);
                    interferingCellManager.CellHightlighter.SelectByLineY(interferingCellManager.Cell.CellGroups);
                }
                interferingCellManager.CellHightlighter.HighlightCell(interferingCellManager, CellHightlighter.CellHighlightType.MinorFocused);
            }

            DisableTextsCellManagersWhereHaveValues(interferingCellManagersLineX);
        }

        public void SelectByLineXAndY(Hint hint)
        {
            CellManager cellManager = hint.CellManager;
            if ((bool)hint.HighlightLineX)
            {
                cellManager.CellHightlighter.SelectByLineX(cellManager.Cell.CellGroups);
                DisableTextsCellManagersWhereHaveValues(cellManager.Cell.CellGroups.LineX);
            }

            if ((bool)hint.HighlightLineY)
            {
                cellManager.CellHightlighter.SelectByLineY(cellManager.Cell.CellGroups);
                DisableTextsCellManagersWhereHaveValues(cellManager.Cell.CellGroups.LineY);
            }
        }

        // Убирает значения из ячеек которые могут помешать отобразить подсказку
        private void DisableTextsCellManagersWhereHaveValues(CellManager[] cellManagers)
        {
            foreach (CellManager cellManager in cellManagers)
            {
                if (cellManager.Cell.Value == 0)
                {
                    cellManager.Text.gameObject.SetActive(false);
                }
            }
        }

        #endregion
    }
}