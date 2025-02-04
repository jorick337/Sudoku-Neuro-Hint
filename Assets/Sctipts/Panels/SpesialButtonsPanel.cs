using System.Collections.Generic;
using Game.Classes;
using Game.Managers;
using Game.AI;
using Help.Classes;
using Help.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Game.Managers.Help.GridAdd;

namespace Game.Panels
{
    public class SpesialButtonsPanel : MonoBehaviour
    {
        #region CONSTANTS

        private const float TRANSPARENCY_ACTIVE = 0.65f;
        private const float TRANSPARENCY_INACTIVE = 0.35f;

        #endregion

        #region EVENTS

        private UnityAction[] buttonActions;

        #endregion

        #region CORE

        [Header("Core")]
        [SerializeField] private AppSettingsPanel appSettingsPanel;
        [SerializeField] private Button[] cellValueButtons = new Button[9]; // Кнопок всего девять

        [Header("Come back")]
        [SerializeField] private Button comeBackButton;
        [SerializeField] private Image comeBackImage;

        [Header("Clear cell")]
        [SerializeField] private Button clearCellButton;
        [SerializeField] private Image clearCellImage;

        [Header("Hint")]
        [SerializeField] private Button hintButton;
        [SerializeField] private Image hintImage;

        [Header("Notepad")]
        [SerializeField] private Button quickNotesButton;
        [SerializeField] private Image quickNotesImage;
        [SerializeField] private Button notepadButton;
        [SerializeField] private Image notepadImage;

        [Header("Neuro hint")]
        [SerializeField] private Button neuroHintButton;
        [SerializeField] private Image neuroHintImage;

        [Header("Managers")]
        [SerializeField] private GridManager gridManager;
        [SerializeField] private HintManager hintManager;
        [SerializeField] private NeuroHintManager neuroHintManager;

        private AppSettingsManager _appSettingsManager;

        #endregion

        #region MONO

        private void Awake()
        {
            InitializeValues();
            InitializeManagers();
            InitializeUI();
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
            buttonActions = new UnityAction[9];
        }

        private void InitializeManagers()
        {
            _appSettingsManager = AppSettingsManager.Instance;
        }

        private void InitializeUI()
        {
            UpdateHintActivity();
            notepadImage.SetTransparency(TRANSPARENCY_INACTIVE);
            quickNotesImage.SetTransparency(TRANSPARENCY_ACTIVE);
            clearCellImage.SetTransparency(TRANSPARENCY_ACTIVE);
            comeBackImage.SetTransparency(TRANSPARENCY_ACTIVE);
            neuroHintImage.SetTransparency(TRANSPARENCY_ACTIVE);
        }

        private void RegisterEvents(bool register)
        {
            if (register)
            {
                RegisterCellValueButtonsListeners(true);

                comeBackButton.onClick.AddListener(UndoLastMove);
                clearCellButton.onClick.AddListener(ClearFocusedCell);
                hintButton.onClick.AddListener(GenerateHint);
                notepadButton.onClick.AddListener(ToggleNotepadMode);
                quickNotesButton.onClick.AddListener(PopulateQuickNotes);
                neuroHintButton.onClick.AddListener(ShowNeuroHints);

                appSettingsPanel.ChangingUseHints += UpdateHintActivity;
            }
            else
            {
                RegisterCellValueButtonsListeners(false);

                comeBackButton.onClick.RemoveListener(UndoLastMove);
                clearCellButton.onClick.RemoveListener(ClearFocusedCell);
                hintButton.onClick.RemoveListener(GenerateHint);
                notepadButton.onClick.RemoveListener(ToggleNotepadMode);
                quickNotesButton.onClick.RemoveListener(PopulateQuickNotes);
                neuroHintButton.onClick.RemoveListener(ShowNeuroHints);

                appSettingsPanel.ChangingUseHints -= UpdateHintActivity;
            }
        }

        #endregion

        #region COME BACK

        private void UndoLastMove()
        {
            Stack<Movement> movesHistory = gridManager.GridBlocks.MovesHistory;
            if (movesHistory.Count == 0)
                return;

            Movement lastMovement = movesHistory.Pop();
            Cell lastCell = gridManager.GridBlocks.Blocks[lastMovement.Block].CellManagers[lastMovement.Number].Cell;

            lastCell.SetValue(0);
            RestoreCellToGrid(gridManager, lastCell);

            if (movesHistory.Count > 0)
            {
                Movement previousMovement = movesHistory.Peek();
                Cell previousCell = gridManager.GridBlocks.Blocks[previousMovement.Block].CellManagers[previousMovement.Number].Cell;

                previousCell.SetValue(previousMovement.Value);
                RestoreCellToGrid(gridManager, previousCell);
            }
        }

        private void RestoreCellToGrid(GridManager gridManager, Cell cell)
        {
            CellManager cellManager = gridManager.GridBlocks.Blocks[cell.Block].CellManagers[cell.Number];

            cellManager.CellAdd.AddValueWithoutMoveAndChecks(cellManager, cell.Value);
            gridManager.GridAdd.AddScoreByScoreType(gridManager, ScoreType.RevertMove);
        }

        #endregion

        #region CLEAR CELL

        private void ClearFocusedCell()
        {
            CellManager focusedCellManager = gridManager.GridBlocks.FocusedCellManager;
            if (focusedCellManager.Cell.Value != 0 && !focusedCellManager.Cell.AddScoreForCorrectFilling)
            {
                focusedCellManager.CellAdd.AddValueWithoutMoveAndChecks(focusedCellManager, 0);
                gridManager.GridAdd.AddScoreByScoreType(gridManager, ScoreType.RevertMove);
            }
        }

        #endregion

        #region USE HINT

        private void GenerateHint()
        {
            Record record = gridManager.Sudoku.Record;
            record.SetNumberOfHints(record.NumberOfHints + 1);

            hintManager.GenerateHint();
            gridManager.GridAdd.AddScoreByScoreType(gridManager, ScoreType.HintTaken);
        }

        #endregion

        #region NOTEPAD MODE

        private void ToggleNotepadMode()
        {
            bool isNotepadMode = !gridManager.GridBlocks.IsNotepadeMode;
            float transparencyImage = isNotepadMode ? TRANSPARENCY_ACTIVE : TRANSPARENCY_INACTIVE;

            gridManager.GridBlocks.SetIsNotepadMode(isNotepadMode);
            notepadImage.SetTransparency(transparencyImage);
        }

        #endregion

        #region QUICK NOTES

        private void PopulateQuickNotes()
        {
            gridManager.GridBlocks.SetIsNotepadMode(true);

            foreach (var cellManager in gridManager.GridBlocks.AllCellManagers)
            {
                // Если ячейка не решенная и в ней нет значений
                if (!cellManager.InputField.readOnly && cellManager.Cell.Value == 0)
                {
                    cellManager.CellUI.SwitchToNotepadMode(cellManager);

                    Cell cell = cellManager.Cell;
                    for (int i = 1; i <= 9; i++) // Перебор подходящих значений
                    {
                        if (gridManager.Sudoku.RealGrid.IsSafe(cell.Block, cell.Number, i))
                        {
                            cellManager.SetText(i.ToString());
                        }
                    }
                }
            }

            gridManager.GridBlocks.SetIsNotepadMode(false);
        }

        #endregion

        #region FOCUS CELL BUTTONS

        private void RegisterCellValueButtonsListeners(bool canRegister)
        {
            for (int i = 0; i < cellValueButtons.Length; i++)
            {
                if (canRegister)
                {
                    int value = i + 1;
                    buttonActions[i] = () => gridManager.GridBlocks.SetValueFocusedCell(value.ToString());
                    cellValueButtons[i].onClick.AddListener(buttonActions[i]);
                }
                else
                    cellValueButtons[i].onClick.RemoveListener(buttonActions[i]);
            }
        }

        #endregion

        #region NEURO HINT

        private void ShowNeuroHints()
        {
            NeuroHint[] neuroHints = neuroHintManager.GenerateHints();

            foreach (var neuroHint in neuroHints)
            {
                Debug.Log($"{neuroHint.Value} {neuroHint.Block} {neuroHint.Number} {neuroHint.Probability}");
            }

            
        }

        #endregion

        #region CALLBACKS

        private void UpdateHintActivity()
        {
            bool isActive = _appSettingsManager.AppSettingData.UseHints;

            hintButton.SetInteractable(isActive);
            hintImage.SetTransparency(isActive ? TRANSPARENCY_ACTIVE : TRANSPARENCY_INACTIVE);
        }

        #endregion
    }
}