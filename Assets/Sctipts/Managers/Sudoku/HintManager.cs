using System;
using System.Linq;
using Game.Classes;
using Game.Managers.Help;
using Help.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Managers
{
    public class HintManager : MonoBehaviour
    {
        #region CONSTANTS

        public const string VIEW_ONE = "SingleCandidate";
        public const string VIEW_TWO = "SinglePosition";

        private readonly string[] TEXT_EXPLANATIONS_SINGLE_CANDIDATE =
        {
            "Обратите внимание на эти ячейки и выделенные области",
            "В этом блоке есть только одна ячейка, которая может содержать цифру 0",
            "Так как это единственно возможный вариант, в этой ячейке должна быть цифра 0"
        };
        private readonly string[] TEXT_EXPLANATIONS_SINGLE_POSITION =
        {
            "Обратите внимание на эту ячейку",
            "В строке, столбце и блоке уже присутствуют цифры 0",
            "Так как это единственно возможный вариант, в этой ячейке должна быть цифра 0"
        };

        public const float TRANSPARENCY_ACTIVE = 1f;
        public const float TRANSPARENCY_INACTIVE = 0.7f;

        #endregion

        #region EVENTS

        private Action UpdateColors;
        private UnityAction OnClickNextStep;
        private UnityAction OnClickPreviousStep;

        #endregion

        #region CORE

        public Hint Hint { get; private set; }
        public HintAdd HintAdd { get; private set; }
        public HintHighlighter HintHighlighter { get; private set; }
        public HintUI HintUI { get; private set; }
        public HintEvents HintEvents { get; private set; }

        public int ExplanationIndex { get; private set; }
        public string[] TextExplanations { get; private set; }

        [Header("Core")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image[] blockers;
        [SerializeField] private Text explanationText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button comeBackButton;
        [SerializeField] private Image[] indicatorImages;

        public Canvas Canvas => canvas;
        public Image[] Blockers => blockers;
        public Text ExplanationText => explanationText;
        public Button NextButton => nextButton;
        public Button ComeBackButton => comeBackButton;
        public Image[] IndicatorImages => indicatorImages;

        [Header("Managers")]
        [SerializeField] private ColorThemeManager colorThemeManager;

        #endregion

        #region MONO

        private void Awake()
        {
            InitializeValues();
        }

        private void OnEnable()
        {
            RegisterEvents(true);
        }

        private void OnDisable()
        {
            RegisterEvents(false);
        }

        #endregion

        #region INITIALIZATION

        private void InitializeValues()
        {
            HintHighlighter = new();
            HintAdd = new();
            HintUI = new();
            HintEvents = new();
        }

        private void RegisterEvents(bool register)
        {
            if (register)
            {
                HintEvents.RegisterOnClickForNextStep(this);
                HintEvents.RegisterOnClickForPreviousStep(this);
                HintEvents.RegisterUpdateColors(this, colorThemeManager);
            }
            else
            {
                nextButton.onClick.RemoveListener(OnClickNextStep);
                comeBackButton.onClick.RemoveListener(OnClickPreviousStep);
                colorThemeManager.ChangingColorTheme -= UpdateColors;
            }
        }

        #endregion

        #region CORE LOGIC

        public void GenerateHint()
        {
            SetExplanationIndex(0);
            Hint = new();

            TextExplanations = Hint.View == VIEW_ONE
               ? TEXT_EXPLANATIONS_SINGLE_CANDIDATE.ToArray()
               : TEXT_EXPLANATIONS_SINGLE_POSITION.ToArray();

            HintAdd.AddValuesToExplanations(this);
            HintAdd.AddColorsToExplanations(this);

            HintUI.ActiveHintUI(this, true);
            HintUI.UpdateExplanationAndIndicators(this);
        }

        #endregion

        #region SET

        public void SetExplanationIndex(int value) => ExplanationIndex = value;
        public void SetTextExplanation(int index, string oldValue, string newValue) => TextExplanations[index] = TextExplanations[index]?.Replace(oldValue, newValue);

        public void SetTransparencyIndicator(float value) => IndicatorImages[ExplanationIndex].SetTransparency(value);

        public void SetUpdateColors(Action action) => UpdateColors = action;
        public void SetOnClickPreviousStep(UnityAction unityAction) => OnClickPreviousStep = unityAction;
        public void SetOnClickNextStep(UnityAction unityAction) => OnClickNextStep = unityAction;

        #endregion
    }
}