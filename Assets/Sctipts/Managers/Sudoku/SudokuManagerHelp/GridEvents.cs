using Game.Classes;
using Game.Panels;
using Help.Classes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Managers.Help
{
    public class GridEvents
    {
        #region GAME INPUT

        public void RegisterOnNumberButtonPressed(GridManager gridManager)
        {
            void OnNumberButtonPressed()
            {
                gridManager.GridBlocks.FocusedCellManager.SetTextDirectly("");
                gridManager.GridBlocks.SetValueFocusedCell(Input.inputString ?? string.Empty);
            }

            gridManager.SetOnNumberButtonPressed(OnNumberButtonPressed);
            GameInputManager.Instance.OnNumberPressed += OnNumberButtonPressed;
        }

        public void RegisterOnDeleteButtonPressed(GridManager gridManager)
        {
            void OnDeleteButtonPressed()
            {
                CellManager cellManager = gridManager.GridBlocks.FocusedCellManager;
                Cell cell = cellManager.Cell;

                if (cellManager == null)
                    return;

                cell.SetValue(0);
                cellManager.SetTextDirectly("");

                gridManager.Sudoku.SetValueRealGrid(cell);
                gridManager.GridAdd.AddDeletedMove(gridManager, cell);
            }

            gridManager.SetOnDeleteButtonPressed(OnDeleteButtonPressed);
            GameInputManager.Instance.OnDeletePressed += OnDeleteButtonPressed;
        }

        #endregion

        #region GAME INFO PANEL

        public void RegisterPauseGame(GridManager gridManager, GameInfoPanel gameInfoPanel)
        {
            void PauseGame()
            {
                CellManager[] cellManagers = gridManager.GridBlocks.AllCellManagers;

                gridManager.GridBlocks.SetIsPause(true);

                cellManagers.SetTextTransparency(0f);
                cellManagers[0].CellHightlighter.UnselectAll(cellManagers[0].Cell.CellGroups);



            }

            gridManager.SetPauseGame(PauseGame);
            gameInfoPanel.Paused += PauseGame;
        }

        public void RegisterUnpauseGame(GridManager gridManager, GameInfoPanel gameInfoPanel)
        {
            void UnpauseGame()
            {
                GridBlocks gridBlocks = gridManager.GridBlocks;

                gridBlocks.SetIsPause(true);
                gridBlocks.AllCellManagers.SetTextTransparency(1f);
                EventSystem.current.SetSelectedGameObject(gridManager.GridBlocks.FocusedCellManager?.gameObject);
            }

            gridManager.SetUnpauseGame(UnpauseGame);
            gameInfoPanel.Unpaused += UnpauseGame;
        }

        #endregion

        #region UPDATE COLORS

        public void RegisterUpdateColors(GridManager gridManager)
        {
            void UpdateColors()
            {
                CellManager focusedCellManager = gridManager.GridBlocks.FocusedCellManager ?? null;
                focusedCellManager?.CellHightlighter.Select(focusedCellManager);
            }

            gridManager.SetUpdateColors(UpdateColors);
            ColorThemeManager.Instance.ChangingColorTheme += UpdateColors;
        }

        #endregion
    }
}