using UnityEngine;
using UnityEngine.UI;
using Game.Managers;
using Help.UI;
using Game.Classes;

namespace Game.Panels
{
    public class FinishGamePanel : MonoBehaviour
    {
        #region CONSTANTS

        private const int DEFAULT_CANVAS_SORTING_ORDER = 0;
        private const int ACTIVE_CANVAS_SORTING_ORDER = 4;
        private const string VICTORY_TEXT = "Победа!";
        private const string DEFEAT_TEXT = "Поражение";
        private const string RESTART_BUTTON_TEXT = "Перезапустить";
        private const string RECORDS_BUTTON_TEXT = "Рекорды";
        private const string DEFEAT_MESSAGE = "Три ошибки. Не сдавайтесь, вы сможете победить!";
        private const string VICTORY_MESSAGE = "Вы справились за {0}!\nОшибки: {1}." +
        "\nПодсказки: {2}.\nВаш рекорд: {3}.\nСможете побить его в следующий раз?";

        #endregion

        #region CORE

        [Header("Core")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image blocker;
        [SerializeField] private Text topText;
        [SerializeField] private Text middleText;
        [SerializeField] private Button secondBottomButton;
        [SerializeField] private Text secondBottomText;

        [Header("Managers")]
        [SerializeField] private GridManager sudokuManager;
        [SerializeField] private SceneController sceneController;

        #endregion

        #region MONO

        private void OnDisable()
        {
            ResetButtonListeners();
        }

        #endregion

        #region CORE LOGIC

        public void FinishGame(bool isVictory)
        {
            SetActivePanel(true);
            if (isVictory)
                DisplayVictory();
            else
                DisplayDefeat();
        }

        private void DisplayVictory()
        {
            ResetButtonListeners();

            Sudoku sudoku = sudokuManager.Sudoku;
            string victoryMessage = string.Format(
                VICTORY_MESSAGE,
                sudoku.Record.GetTimeOfSolution(),
                sudoku.Record.NumberOfMistakes,
                sudoku.Record.NumberOfHints,
                sudoku.Record.Score
            );

            SetPanelText(VICTORY_TEXT, victoryMessage, RECORDS_BUTTON_TEXT);
            secondBottomButton.onClick.AddListener(sceneController.LoadRecordsScene);
        }

        private void DisplayDefeat()
        {   
            ResetButtonListeners();

            SetPanelText(DEFEAT_TEXT, DEFEAT_MESSAGE, RESTART_BUTTON_TEXT);
            secondBottomButton.onClick.AddListener(RestartGame);
        }

        private void RestartGame()
        {
            sudokuManager.RestartGame();
            SetActivePanel(false);
        }

        private void ResetButtonListeners() => secondBottomButton.onClick.RemoveAllListeners();

        #endregion

        #region SET

        private void SetActivePanel(bool isActive)
        {
            canvas.SetSortingOrder(isActive 
                ? ACTIVE_CANVAS_SORTING_ORDER
                : DEFAULT_CANVAS_SORTING_ORDER);
            blocker.SetRaycastTarget(isActive);
        }

        private void SetPanelText(string top, string middle, string bottomButton)
        {
            topText.SetText(top);
            middleText.SetText(middle);
            secondBottomText.SetText(bottomButton);
        }

        #endregion
    }
}