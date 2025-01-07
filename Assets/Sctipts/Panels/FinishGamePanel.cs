using UnityEngine;
using UnityEngine.UI;
using Game.Managers;
using Help.UI;
using Game.Classes;
using System;
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

        private UnityAction OnClickFirstBottomButton;
        private UnityAction OnClickSecondBottomButton;

        #endregion

        #region CORE

        [Header("Core")]
        [SerializeField] private Canvas newGamePanelCanvas;
        [SerializeField] private Image[] blockers;

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
            if (OnClickFirstBottomButton != null)
            {
                firstBottomButton.onClick.RemoveListener(OnClickFirstBottomButton);
            }

            if (OnClickSecondBottomButton != null)
            {
                secondBottomButton.onClick.RemoveListener(OnClickFirstBottomButton);
            }
        }

        #endregion

        #region CORE LOGIC

        public void FinishGame(bool isVictory)
        {
            gridManager.GridBlocks.SetIsPause(true);
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

            Sudoku sudoku = gridManager.Sudoku;
            string victoryMessage = string.Format(
                VICTORY_MESSAGE,
                sudoku.Record.GetTimeOfSolution(),
                sudoku.Record.NumberOfMistakes,
                sudoku.Record.NumberOfHints,
                sudoku.Record.Score
            );

            ChangePanelText(VICTORY_TEXT, victoryMessage, RECORDS_BUTTON_TEXT);
            SetOnClickSecondBottomButton(sceneController.LoadRecordsScene);
        }

        private void DisplayDefeat()
        {
            UnregisterEvents();

            ChangePanelText(DEFEAT_TEXT, DEFEAT_MESSAGE, RESTART_BUTTON_TEXT);
            SetOnClickFirstBottomButton(() => newGamePanelCanvas.SetSortingOrder(4));
            SetOnClickSecondBottomButton(RestartGame);
        }

        private void RestartGame()
        {
            gridManager.RestartGame();
            ActivatePanel(false);
        }

        #endregion

        #region UI UPDATES

        public void ActivatePanel(bool isActive)
        {
            canvas.SetSortingOrder(isActive ? ACTIVE_CANVAS_SORTING_ORDER : DEFAULT_CANVAS_SORTING_ORDER);

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

        private void SetOnClickFirstBottomButton(UnityAction unityAction)
        {
            OnClickFirstBottomButton = unityAction;
            firstBottomButton.onClick.AddListener(OnClickFirstBottomButton);
        }

        private void SetOnClickSecondBottomButton(UnityAction unityAction)
        {
            OnClickSecondBottomButton = unityAction;
            secondBottomButton.onClick.AddListener(OnClickSecondBottomButton);
        }

        #endregion
    }
}