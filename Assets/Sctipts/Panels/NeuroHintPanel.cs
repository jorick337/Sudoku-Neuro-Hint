using System.Linq;
using Game.AI;
using Game.Managers;
using Help.UI;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Game.Panels
{
    public class NeuroHintPanel : MonoBehaviour
    {
        #region CONSTANTS

        private const int ACTIVE_CANVAS = 3;
        private const int INACTIVE_CANVAS = 0;

        #endregion

        #region EVENTS

        private UnityAction[] probabilityButtonActions;
        private UnityAction closePanelButtonAction;

        #endregion

        #region CORE

        [Header("Core")]
        [SerializeField] private Canvas canvas;

        public Canvas Canvas => canvas;

        [Header("UI")]
        [SerializeField] private Image[] probabilityImages;
        [SerializeField] private Text[] probabilityTexts;
        [SerializeField] private Button[] probabilityButtons;
        [SerializeField] private Button closePanelButton;

        private CellManager[] _selectedCellManagers;

        [Header("Managers")]
        [SerializeField] private GridManager gridManager;

        private AppSettingsManager _appSettingsManager;

        #endregion

        #region MONO

        private void Awake()
        {
            InitializeManagers();
            InitializeValues();
            RegisterEvents(true);
        }

        private void OnDisable()
        {
            RegisterEvents(false);
        }

        #endregion

        #region INITIALIZATION

        private void InitializeManagers()
        {
            _appSettingsManager = AppSettingsManager.Instance;
        }

        private void InitializeValues()
        {
            probabilityButtonActions = new UnityAction[probabilityButtons.Length];
        }

        private void InitializeNeuroHints(NeuroHint[] neuroHints)
        {
            _selectedCellManagers = new CellManager[neuroHints.Length];

            for (int i = 0; i < neuroHints.Length; i++)
            {
                NeuroHint neuroHint = neuroHints[i];
                CellManager cellManager = gridManager.GridBlocks.Blocks[neuroHint.Block].CellManagers[neuroHint.Number];

                _selectedCellManagers[i] = cellManager;
            }
        }

        private void RegisterEvents(bool register)
        {
            if (register)
            {
                closePanelButtonAction = () => Canvas.SetSortingOrder(INACTIVE_CANVAS);
                closePanelButton.onClick.AddListener(closePanelButtonAction);
            }
            else
            {
                closePanelButton.onClick.RemoveListener(closePanelButtonAction);

                for (int i = 0; i < probabilityButtons.Length; i++)
                {
                    probabilityButtons[i].onClick.RemoveListener(probabilityButtonActions[i]);
                }
            }
        }

        private void RegisterProbabilityButtonsEvents(NeuroHint[] neuroHints = null)
        {
            for (int i = 0; i < probabilityButtons.Length; i++)
            {
                AddProbabilityButtonListener(i, neuroHints[i]);
            }
        }

        #endregion

        #region CORE LOGIC

        public void DisplayProbabilities(NeuroHint[] neuroHints)
        {
            Canvas.SetSortingOrder(ACTIVE_CANVAS);

            UpdateProbabilitiesColor();
            UpdateProbabilities(neuroHints);

            InitializeNeuroHints(neuroHints);
            RegisterProbabilityButtonsEvents(neuroHints);
        }

        #endregion

        #region UPDATE UI

        private void UpdateProbabilitiesColor()
        {
            Color rightColor = _appSettingsManager.SelectedColorTheme.MainFocusedImageGridCell;

            SetImagesColor(rightColor);
            SetImagesTransparency((float)0.9);
        }

        private void UpdateProbabilities(NeuroHint[] neuroHints)
        {
            neuroHints = neuroHints.OrderByDescending(neuroHint => neuroHint.Probability).ToArray();

            for (int i = 0; i < neuroHints.Length; i++)
            {
                probabilityTexts[i].text = $"{neuroHints[i].Value} - {System.Math.Round(neuroHints[i].Probability * 100, 1)} %";
            }
        }

        #endregion

        #region SET

        private void SetImagesColor(Color color)
        {
            foreach (var image in probabilityImages)
            {
                image.SetColor(color);
            }
        }

        private void SetImagesTransparency(float value)
        {
            foreach (var image in probabilityImages)
            {
                image.SetTransparency(value);
                value = (float)(value - 0.1);
            }
        }

        #endregion

        #region ADD

        private void AddProbabilityButtonListener(int number, NeuroHint neuroHint)
        {
            probabilityButtonActions[number] = () => FocusCellManager(_selectedCellManagers[number], neuroHint);
            probabilityButtons[number].onClick.AddListener(probabilityButtonActions[number]);
        }

        #endregion

        #region CALLBACKS

        private void FocusCellManager(CellManager cellManager, NeuroHint neuroHint)
        {
            EventSystem.current.SetSelectedGameObject(cellManager.gameObject);
            cellManager.CellHightlighter.SelectWithSameValues(cellManager.Cell.CellGroups, neuroHint.Value);
        }

        #endregion
    }
}