using UnityEngine;
using UnityEngine.UI;
using Game.Managers;
using Help.UI;
using Game.Classes;
using UnityEngine.Events;

namespace Game.Panels
{
    public class FinishGamePanel : MonoBehaviour
    {
        #region CONSTANTS

        private const int DEFAULT_CANVAS_SORTING_ORDER = 0;
        private const int ACTIVE_CANVAS_SORTING_ORDER = 3;
        private const string VICTORY_TEXT = "Победа!";
        private const string DEFEAT_TEXT = "Поражение";
        private const string RESTART_BUTTON_TEXT = "Перезапустить";
        private const string RECORDS_BUTTON_TEXT = "Рекорды";
        private const string DEFEAT_MESSAGE = "Три ошибки. Не сдавайтесь, вы сможете победить!";
        private const string VICTORY_MESSAGE = "Вы справились за {0}!\nОшибки: {1}." +
        "\nПодсказки: {2}.\nВаш рекорд: {3}.\nСможете побить его в следующий раз?";

        #endregion

        #region EVENTS

        private UnityAction OnClickSecondBottomButton;

        #endregion

        #region CORE

        [Header("Core")]
        [SerializeField] private Canvas newGamePanelCanvas;
        [SerializeField] private Image[] blockers;
        [SerializeField] private NeuroHintPanel neuroHintPanel;

        [Header("Panel")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Text topText;
        [SerializeField] private Text middleText;
        [SerializeField] private Button firstBottomButton;
        [SerializeField] private Button secondBottomButton;
        [SerializeField] private Text secondBottomText;

        [Header("Managers")]
        [SerializeField] private GridManager gridManager;
        [SerializeField] private SceneController sceneController;

        #endregion

        #region MONO

        private void OnDisable()
        {
            UnregisterEvents();
        }

        #endregion

        #region INITIALIZATION

        private void UnregisterEvents()
        {
            firstBottomButton.onClick.RemoveListener(OpenNewGamePanel);

            if (OnClickSecondBottomButton != null)
            {
                secondBottomButton.onClick.RemoveListener(OnClickSecondBottomButton);
            }
        }

        #endregion

        #region CORE LOGIC

        public void FinishGame(bool isVictory)
        {
            ActivatePanel(true);

            if (isVictory)
            {
                DisplayVictory();
            }
            else
            {
                DisplayDefeat();
            }
        }

        private void DisplayVictory()
        {
            UnregisterEvents();
            SetOnClickSecondBottomButton(SaveAndShowRecord);

            Sudoku sudoku = gridManager.Sudoku;
            string victoryMessage = string.Format(
                VICTORY_MESSAGE,
                sudoku.Record.GetTimeOfSolution(),
                sudoku.Record.NumberOfMistakes,
                sudoku.Record.NumberOfHints,
                sudoku.Record.Score
            );
            ChangePanelText(VICTORY_TEXT, victoryMessage, RECORDS_BUTTON_TEXT);
        }

        private void DisplayDefeat()
        {
            UnregisterEvents();
            firstBottomButton.onClick.AddListener(OpenNewGamePanel);
            SetOnClickSecondBottomButton(RestartGame);

            ChangePanelText(DEFEAT_TEXT, DEFEAT_MESSAGE, RESTART_BUTTON_TEXT);
        }

        #endregion

        #region UI UPDATES

        public void ActivatePanel(bool isActive)
        {
            canvas.SetSortingOrder(isActive ? ACTIVE_CANVAS_SORTING_ORDER : DEFAULT_CANVAS_SORTING_ORDER);
            neuroHintPanel.Canvas.SetSortingOrder(DEFAULT_CANVAS_SORTING_ORDER);

            foreach (var image in blockers)
            {
                image.SetRaycastTarget(isActive);
            }
        }

        private void ChangePanelText(string top, string middle, string secondBottomButton)
        {
            topText.SetText(top);
            middleText.SetText(middle);
            secondBottomText.SetText(secondBottomButton);
        }

        #endregion

        #region SET

        private void SetOnClickSecondBottomButton(UnityAction unityAction)
        {
            OnClickSecondBottomButton = unityAction;
            secondBottomButton.onClick.AddListener(OnClickSecondBottomButton);
        }

        #endregion

        #region CALLBACKS

        private void OpenNewGamePanel()
        {
            newGamePanelCanvas.SetSortingOrder(4);
        }

        private void SaveAndShowRecord()
        {
            gridManager.SetSudoku(null);
            sceneController.LoadRecordsScene();
        }

        private void RestartGame()
        {
            gridManager.RestartGame();
            ActivatePanel(false);
        }

        #endregion
    }
}