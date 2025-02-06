using System.Linq;
using Game.AI;
using Game.Classes;
using Game.Managers;
using Help.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Panels
{
    public class NeuroHintPanel : MonoBehaviour
    {
        #region CONSTANTS

        private const int ACTIVE_CANVAS = 3;
        private const int INACTIVE_CANVAS = 0;

        #endregion

        #region CORE

        [Header("Core")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image[] probabilityImages;
        [SerializeField] private Text[] probabilityTexts;

        [Header("Managers")]
        [SerializeField] private AppSettingsManager appSettingsManager;
        [SerializeField] private GridManager gridManager;

        #endregion

        #region CORE LOGIC

        public void DisplayProbabilities(NeuroHint[] neuroHints)
        {
            canvas.SetSortingOrder(ACTIVE_CANVAS);
            UpdateProbabilitiesColor();
            UpdateProbabilities(neuroHints);

            foreach (CellManager cellManager in gridManager.GridBlocks.AllCellManagers)
            {
                cellManager.Text.gameObject.SetActive(true);
            }

            foreach (var neuroHint in neuroHints)
            {

            }
        }

        #endregion

        #region UPDATE UI

        private void UpdateProbabilitiesColor()
        {
            Color rightColor = appSettingsManager.SelectedColorTheme.RightTextGridCellColor;

            SetImagesColor(rightColor);
            SetImagesTransparency((float)0.9);
        }

        private void UpdateProbabilities(NeuroHint[] neuroHints)
        {
            neuroHints = neuroHints.OrderByDescending(neuroHint => neuroHint.Probability).ToArray();

            for (int i = 0; i < neuroHints.Length; i++)
            {
                probabilityTexts[i].text = $"{System.Math.Round(neuroHints[i].Probability * 100, 1)} %";
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
    }
}