using Game.Classes;
using Game.Panels;

namespace Game.Managers.Help
{
    public class GridUI
    {
        #region GAME INFO PANEL

        public void UpdateGameInfoPanel(GridManager gridManager)
        {
            Record record = gridManager.Sudoku.Record;
            string levelStr = AppSettingsManager.Instance.GetLevelStr(record.Level);

            GameInfoPanel gameInfoPanel = gridManager.GameInfoPanel;

            gameInfoPanel.SetDifficultyLevelText(levelStr);
            gameInfoPanel.SetMistakesText(record.NumberOfMistakes);
            gameInfoPanel.SetScoreRecordText(record.Score);
            gameInfoPanel.SetTimeText(record.GetTimeOfSolution());
        }

        #endregion
    }
}