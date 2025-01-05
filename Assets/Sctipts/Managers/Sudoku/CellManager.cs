using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Game.Classes;
using Help.UI;
using System;
using Game.Managers.Help;

namespace Game.Managers
{
    public class CellManager : MonoBehaviour, ISelectHandler
    {
        #region CONSTANTS

        private const string ANIM_VISIBILITY = "Visibility";

        #endregion

        #region EVENTS

        private Action UpdateColors;
        private UnityAction<string> OnValueChanged;

        #endregion

        #region CORE

        public Cell Cell { get; private set; }
        public CellAdd CellAdd { get; private set; }
        public CellHightlighter CellHightlighter { get; private set; }
        public CellUI CellUI { get; private set; }
        public CellEvents CellEvents { get; private set; }
        [Header("Core")]
        [SerializeField] private int block;
        [SerializeField] private int number; // Номер ячейки в блоке

        [Header("UI Objects")]
        [SerializeField] private InputField inputField;
        [SerializeField] private Text text;
        [SerializeField] private Image image;
        [SerializeField] private Animator animator;

        public InputField InputField => inputField;
        public Text Text => text;
        public Image Image => image;

        private ColorThemeManager _colorThemeManager;
        private GridManager _gridManager;
        private AppSettingsManager _appSettings;

        #endregion

        #region MONO

        private void Awake()
        {
            InitializeManagers();
        }

        private void Start()
        {
            InitializeValues();
            RegisterEvents(true);
            CellUI.UpdateText(this);
        }

        private void OnDisable()
        {
            RegisterEvents(false);
        }

        #endregion

        #region INITIALIZATON

        private void InitializeManagers()
        {
            _colorThemeManager = ColorThemeManager.Instance;
            _gridManager = GridManager.Instance;
            _appSettings = AppSettingsManager.Instance;
        }

        private void InitializeValues()
        {
            int value = _gridManager.Sudoku.RealGrid[block, number];
            ColorTheme colorTheme = _appSettings.SelectedColorTheme;
            GridBlocks gridBlocks = _gridManager.GridBlocks;

            Cell = new Cell(block, number, value, colorTheme, gridBlocks);
            CellAdd = new();
            CellHightlighter = new();
            CellUI = new();
            CellEvents = new();
        }

        private void RegisterEvents(bool register)
        {
            if (register)
            {
                CellEvents.RigisterUpdateColors(this);
                CellEvents.RegisterOnValueChangedForNormalMode(this);
            }
            else
            {
                _colorThemeManager.ChangingColorTheme -= UpdateColors;
                InputField.onValueChanged.RemoveListener(OnValueChanged);
            }
        }

        #endregion

        #region SELECTING

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            _gridManager.GridBlocks.SetFocusedCellManager(this);
            CellHightlighter.Select(this);
            Invoke("RemoveFocus", 0.1f);
        }

        private void RemoveFocus() => EventSystem.current.SetSelectedGameObject(null);

        #endregion

        #region SET

        public void SetTextWithValidate(string Value)
        {
            if (!inputField.readOnly)
            {
                if (_gridManager.GridBlocks.IsNotepadeMode && inputField.characterLimit == 1) // Если режим блокнота не включен
                {
                    CellUI.SwitchToNotepadMode(this);
                }
                else if (!_gridManager.GridBlocks.IsNotepadeMode && inputField.characterLimit == 0) // Если обычный режим не включен
                {
                    CellUI.SwitchToNormalMode(this);
                }

                SetText(Value);
            }
        }

        public void SetTextDirectly(string Value)
        {
            inputField.onValueChanged.RemoveAllListeners();
            if (!inputField.readOnly)
                SetText(Value);
            inputField.onValueChanged.AddListener(OnValueChanged);
        }

        public void SetOnValueChanged(UnityAction<string> unityAction)
        {
            if (OnValueChanged != null)
            {
                InputField.onValueChanged.RemoveListener(OnValueChanged);
            }
            
            OnValueChanged = unityAction;
            InputField.onValueChanged.AddListener(OnValueChanged);
        }

        public void SetActiveVisibilityAnimation(bool value) => animator.SetBool(ANIM_VISIBILITY, value);

        public void SetUpdateColors(Action action) => UpdateColors = action;
        public void SetCell(Cell cell) => Cell = cell;

        public void SetText(string value) => inputField.SetText(value == "0" ? "" : value);
        public void SetTransparencyColorText(float value) => Text.SetTransparency(value); // Для анимации Visibility

        #endregion
    }
}