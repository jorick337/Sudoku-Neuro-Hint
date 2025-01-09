using System;
using System.Linq;
using Game.Classes;
using Game.Managers.Help;
using Game.Panels;
using Help.Classes;
using Help.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Managers
{
    public class GridManager : MonoBehaviour
    {
        #region SINGLETON

        public static GridManager Instance { get; private set; }

        #endregion

        #region EVENTS

        public Action UpdateColors;

        private Action OnNumberButtonPressed;
        private Action OnDeleteButtonPressed;

        private Action PauseGame;
        private Action UnpauseGame;

        #endregion

        #region CORE

        public GridBlocks GridBlocks { get; private set; }
        public GridAdd GridAdd { get; private set; }
        public GridUI GridUI { get; private set; }
        public GridEvents GridEvents { get; private set; }

        public Sudoku Sudoku { get; private set; }

        [Header("Core")]
        [SerializeField] private GameObject grid;
        [SerializeField] private GameObject grid3x3Prefab;

        private GameObject _selectedGrid;

        [Header("Managers")]
        [SerializeField] private GameInfoPanel gameInfoPanel;
        [SerializeField] private FinishGamePanel finishGamePanel;
        [SerializeField] private ColorThemeManager colorThemeManager;

        public GameInfoPanel GameInfoPanel => gameInfoPanel;

        private UserManager _userManager;
        private AppSettingsManager _appSettingsManager;
        private GameInputManager _gameInputManager;

        #endregion

        #region MONO

        private void Awake()
        {
            Instance = this;

            InitializeManagers();
            InitializeValues();
        }

        private void Start()
        {
            InstantiateGridBySize();
            GridUI.UpdateGameInfoPanel(this);
        }

        private void Update()
        {
            GridAdd.IncrementTime(this);
        }

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
            if (_appSettingsManager.AppSettingData.AutosaveSudoku && !GridBlocks.AllCellManagers.All(cell => cell.InputField.readOnly))
            {
                _userManager.SaveSudoku(Sudoku);
            }
            else
            {
                Sudoku = null;
            }
        }

        #endregion

        #region INITIALIZATION

        private void InitializeManagers()
        {
            _userManager = UserManager.Instance;
            _appSettingsManager = AppSettingsManager.Instance;
            _gameInputManager = GameInputManager.Instance;
        }

        private void InitializeValues()
        {
            GridAdd = new();
            GridUI = new();
            GridEvents = new();

            if (_appSettingsManager.IsNewGame)
            {
                Sudoku = new Sudoku(_appSettingsManager.SelectedLevel);
                _appSettingsManager.SetIsNewGame(false);
            }
            else
            {
                Sudoku = new(_userManager.User.UnfinishedSudoku);
            }

            _appSettingsManager.SetSelectedScoreRecordPoints(Sudoku.Record.Level - 1);
        }

        private void RegisterEvents(bool register)
        {
            if (register)
            {
                GridEvents.RegisterUpdateColors(this);

                GridEvents.RegisterOnNumberButtonPressed(this);
                GridEvents.RegisterOnDeleteButtonPressed(this);

                GridEvents.RegisterPauseGame(this, gameInfoPanel);
                GridEvents.RegisterUnpauseGame(this, gameInfoPanel);
            }
            else
            {
                colorThemeManager.ChangingColorTheme -= UpdateColors;

                _gameInputManager.OnNumberPressed -= OnNumberButtonPressed;
                _gameInputManager.OnDeletePressed -= OnDeleteButtonPressed;

                gameInfoPanel.Paused -= PauseGame;
                gameInfoPanel.Unpaused -= UnpauseGame;
            }
        }

        #endregion

        #region CORE LOGIC

        private void InstantiateGridBySize()
        {
            switch (Sudoku.MainGrid.GetLength(0))
            {
                case 9:
                    CreateGridInstance(grid3x3Prefab);
                    break;
            }
        }

        private void CreateGridInstance(GameObject gridPrefab)
        {
            _selectedGrid = Instantiate(gridPrefab, grid.transform);
            GridBlocks = _selectedGrid.GetComponent<GridBlocks>();

            colorThemeManager.UpdateUIElementsAndColorTheme();
        }

        #endregion

        #region NEW GAME

        public void StartNewGame()
        {
            InitializeValues();
            ResetGame();
        }

        public void RestartGame()
        {
            Sudoku.SetRealGrid(Sudoku.InitialGrid);
            Sudoku.Record = new(Sudoku.Record.Level, 0, 0, 0, 0);

            GridBlocks.MovesHistory.Clear();
            GridBlocks.SetIsNotepadMode(false);

            ResetGame();
        }

        private void ResetGame()
        {
            DestroyImmediate(_selectedGrid);
            InstantiateGridBySize();
            GridUI.UpdateGameInfoPanel(this);
        }

        #endregion

        #region FINISH

        public void CheckGameCompletion()
        {
            if (Sudoku.Record.NumberOfMistakes >= 3)
            {
                FinishGame(false);
                return;
            }
            else if (GridBlocks.AllCellManagers.All(cell => cell.InputField.readOnly))
            {
                FinishGame(true);
            }
        }

        private void FinishGame(bool winGame)
        {
            GridBlocks.FocusedCellManager.InputField.SetReadOnly(true);
            GridBlocks.SetIsPause(true);

            if (winGame)
            {
                GridAdd.AddScoreByScoreType(this, GridAdd.ScoreType.LevelFinished);

                if (_appSettingsManager.AppSettingData.AutosaveRecord)
                {
                    _userManager.AddRecord(Sudoku.Record);
                }

                _userManager.SaveSudoku(null);
            }
            else if (_appSettingsManager.AppSettingData.AutosaveSudoku)// Сохранение перезапущенной версии в случае поражения
            {
                Sudoku.SetRealGrid(Sudoku.InitialGrid);
                _userManager.SaveSudoku(Sudoku);
            }

            finishGamePanel.FinishGame(winGame);
        }

        #endregion

        #region SET

        public void SetSudoku(Sudoku sudoku) => Sudoku = sudoku;

        public void SetUpdateColors(Action action) => UpdateColors += action;

        public void SetOnNumberButtonPressed(Action action) => OnNumberButtonPressed = action;
        public void SetOnDeleteButtonPressed(Action action) => OnDeleteButtonPressed = action;

        public void SetPauseGame(Action action) => PauseGame = action;
        public void SetUnpauseGame(Action action) => UnpauseGame = action;

        #endregion

        #region GET

        // Ищет ячейки которые мешают поставить такое же значение в других местах в блоке
        public CellManager[] GetInterferingCells(int block, int value, bool isHorizontal)
        {
            int startBlock = isHorizontal ? block / 3 * 3 : block % 3;
            int durationBlock = isHorizontal ? 3 : 9;
            int blockStep = isHorizontal ? 1 : 3;

            return GridBlocks.Blocks.GetCellManagers(startBlock, durationBlock, blockStep, 0, 9, 1)
            .Where(cellManager => cellManager.Cell.Value == value).
            ToArray();
        }

        #endregion
    }
}