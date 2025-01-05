using System;
using UnityEngine;
using UnityEngine.UI;
using Help.UI;

namespace Game.Panels
{
    public class GameInfoPanel : MonoBehaviour
    {
        #region CONSTANTS

        private const string MISTAKES_TEMPLATE = "Ошибки: {0}/3";
        private const string SCORE_TEMPLATE = "Счет: {0}";
        private const string DIFFICULTY_LEVEL_TEMPLATE = "Уровень: {0}";

        #endregion

        #region EVENTS

        public event Action Paused;
        public event Action Unpaused;

        #endregion

        #region CORE

        [Header("Core")]
        [SerializeField] private Text difficultyLevelText;
        [SerializeField] private Text mistakesText;
        [SerializeField] private Text scoreRecordText;
        [SerializeField] private Text timeText;

        [Header("Pause")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button unpauseButton;

        #endregion

        #region MONO

        private void OnEnable()
        {
            RegisterEvents(true);
        }

        private void OnDisable()
        {
            RegisterEvents(false);
        }

        private void OnDestroy()
        {
            Paused = null;
            Unpaused = null;
        }

        #endregion

        #region INITIALIZATION

        private void RegisterEvents(bool register)
        {
            if (register)
            {
                pauseButton.onClick.AddListener(Pause);
                unpauseButton.onClick.AddListener(Unpause);
            }
            else
            {
                pauseButton.onClick.RemoveListener(Pause);
                unpauseButton.onClick.RemoveListener(Unpause);
            }
        }

        #endregion

        #region SET

        public void SetTimeText(string time) => timeText.SetText(time);
        public void SetMistakesText(int mistakes) =>
            mistakesText.SetText(string.Format(MISTAKES_TEMPLATE, mistakes));
        public void SetScoreRecordText(int score) =>
            scoreRecordText.SetText(string.Format(SCORE_TEMPLATE, score));
        public void SetDifficultyLevelText(string level) =>
            difficultyLevelText.SetText(string.Format(DIFFICULTY_LEVEL_TEMPLATE, level));

        #endregion

        #region CALLBACKS

        public void Pause() => Paused?.Invoke();
        public void Unpause() => Unpaused?.Invoke();

        #endregion
    }
}