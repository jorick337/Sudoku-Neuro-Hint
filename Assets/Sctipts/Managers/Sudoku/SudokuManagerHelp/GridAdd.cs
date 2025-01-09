using System.Collections.Generic;
using Game.Classes;
using Game.Panels;
using UnityEngine;

namespace Game.Managers.Help
{
    public class GridAdd
    {
        #region MOVE MANAGEMENT

        public void AddMove(GridManager gridManager, Cell cell)
        {
            Debug.Log(cell.Value);
            if (gridManager.GridBlocks.IsNotepadeMode)
            {
                return;
            }

            Stack<Movement> movesHistory = gridManager.GridBlocks.MovesHistory;

            Movement movement = new(cell);
            if (movesHistory.Count == 0)
            {
                movesHistory.Push(movement);
                return;
            }

            Movement lastMovement = movesHistory.Peek();
            if (!IsCellsEqual(movement, lastMovement)) // Если не равны
                if (IsBlockAndNumberCellsEqual(movement, lastMovement)) // Если одна и та же ячейка
                {
                    movesHistory.Push(movement);
                }
                else
                {
                    RemoveDeletedMoves(movesHistory);
                    movesHistory.Push(movement);
                }
        }

        public void AddDeletedMove(GridManager gridManager, Cell cell)
        {
            RemoveDeletedMoves(gridManager.GridBlocks.MovesHistory);

            if (cell.Value != 0)
            {
                cell.SetValue(0);

                Movement movement = new(cell);
                gridManager.GridBlocks.MovesHistory.Push(movement);
            }
        }

        private void RemoveDeletedMoves(Stack<Movement> movesHistory)
        {
            while (movesHistory.Count > 0 && movesHistory.Peek().Value == 0)
                movesHistory.Pop();
        }

        #endregion

        #region TIME MANAGEMENT

        public void IncrementTime(GridManager gridManager)
        {
            if (!gridManager.GridBlocks.IsPaused)
            {
                Record record = gridManager.Sudoku.Record;

                record.AddTime(Time.deltaTime);
                gridManager.GameInfoPanel.SetTimeText(record.GetTimeOfSolution());
            }
        }

        #endregion

        #region SCORE MANAGEMENT

        public enum ScoreType
        {
            FillCorrectly,
            LevelFinished,
            QuickFinish,
            WrongFill,
            HintTaken,
            RevertMove
        }

        public void AddScoreByScoreType(GridManager gridManager, ScoreType scoreType)
        {
            ScoreRecordPoints scoreRecordPoints = AppSettingsManager.Instance.SelectedScoreRecordPoints;

            Record record = gridManager.Sudoku.Record;

            float timeRatio = (float)record.Level * 5 * 60; // Базовое время: 5 минут на уровень
            bool giveTimeBonus = record.TimeOfSolution < timeRatio;

            Debug.Log(record.TimeOfSolution + " " + timeRatio);
            int score = scoreType switch
            {
                ScoreType.FillCorrectly => scoreRecordPoints.FillCorrectly,
                ScoreType.LevelFinished => scoreRecordPoints.LevelFinished,
                ScoreType.QuickFinish => giveTimeBonus ? scoreRecordPoints.QuickFinish : 0,
                ScoreType.WrongFill => scoreRecordPoints.WrongFill,
                ScoreType.HintTaken => scoreRecordPoints.HintTaken,
                ScoreType.RevertMove => scoreRecordPoints.RevertMove,
                _ => 0
            };

            record.AddScore(score);
            gridManager.GameInfoPanel.SetScoreRecordText(record.Score);
        }

        #endregion

        #region MISTAKE MANAGEMENT

        public void AddMistake(GridManager gridManager)
        {
            GameInfoPanel gameInfoPanel = gridManager.GameInfoPanel;
            Record record = gridManager.Sudoku.Record;

            record.AddMistake();
            AddScoreByScoreType(gridManager, ScoreType.WrongFill);

            gameInfoPanel.SetScoreRecordText(record.Score);
            gameInfoPanel.SetMistakesText(record.NumberOfMistakes);

            gridManager.CheckGameCompletion();
        }

        #endregion

        #region BOOl

        private bool IsCellsEqual(Movement firstMovement, Movement secondMovement) =>
            firstMovement.Block == secondMovement.Block &&
            firstMovement.Number == secondMovement.Number &&
            firstMovement.Value == secondMovement.Value;

        private bool IsBlockAndNumberCellsEqual(Movement firstMovement, Movement secondMovement) =>
            firstMovement.Block == secondMovement.Block &&
            firstMovement.Number == secondMovement.Number;


        #endregion
    }
}