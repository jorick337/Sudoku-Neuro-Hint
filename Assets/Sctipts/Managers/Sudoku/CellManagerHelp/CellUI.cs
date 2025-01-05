using Game.Classes;
using Help.UI;
using UnityEngine.UI;

namespace Game.Managers.Help
{
    public class CellUI
    {
        public void ConfigureInputField(CellManager cellManager, int characterLimit, int fontSize, string text)
        {
            InputField inputField = cellManager.InputField;

            inputField.SetCharacteLimit(characterLimit);
            inputField.SetFontSize(fontSize);
            cellManager.SetTextDirectly(text);
        }

        public void SwitchToNormalMode(CellManager cellManager)
        {
            ConfigureInputField(cellManager, 1, 22, cellManager.Cell.Value.ToString());
            cellManager.CellEvents.RegisterOnValueChangedForNormalMode(cellManager);
        }

        public void SwitchToNotepadMode(CellManager cellManager)
        {
            ConfigureInputField(cellManager, 0, 7, "");

            cellManager.Cell.SetValue(0);
            cellManager.Cell.SetNotePadValues(new());

            cellManager.CellEvents.RegisterOnValueChangedForNotepadMode(cellManager);

            CellManager focusedCellManager = GridManager.Instance.GridBlocks.FocusedCellManager;
            if (focusedCellManager != null)
            {
                cellManager.CellHightlighter.Select(focusedCellManager);
            }
        }

        public void UpdateText(CellManager cellManager)
        {
            Cell cell = cellManager.Cell;
            bool isReadOnly = cell.AddScoreForCorrectFilling;
            string text = isReadOnly ? cell.Value.ToString() : "";

            cellManager.SetTextDirectly(text);
            cellManager.InputField.SetReadOnly(isReadOnly);
            cellManager.CellHightlighter.HighlightCell(cellManager, CellHightlighter.CellHighlightType.Unselected);
        }
    }
}