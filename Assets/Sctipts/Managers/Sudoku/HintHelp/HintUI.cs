
using System;
using Game.Classes;
using Help.UI;
using UnityEngine.UI;

namespace Game.Managers.Help
{
    public class HintUI
    {
        #region CORE LOGIC

        public void ActiveHintUI(HintManager hintManager, bool value)
        {
            hintManager.Canvas.SetSortingOrder(value ? 3 : 0);
            hintManager.ComeBackButton.gameObject.SetActive(false);

            SetRaycastTargetImages(hintManager.Blockers, value);
            SetTransparencyImages(hintManager.IndicatorImages, HintManager.TRANSPARENCY_INACTIVE);
        }

        #endregion

        #region EXPLANATIONS AND INDICATORS

        public void UpdateExplanationAndIndicators(HintManager hintManager)
        {
            string textExplanation = hintManager.TextExplanations[hintManager.ExplanationIndex];
            hintManager.SetTransparencyIndicator(HintManager.TRANSPARENCY_ACTIVE);
            hintManager.ExplanationText.SetText(textExplanation);

            switch (hintManager.Hint.View)
            {
                case HintManager.VIEW_ONE:
                    UpdateUIForSingleCandidate(hintManager);
                    break;
                case HintManager.VIEW_TWO:
                    UpdateUIForSinglePosition(hintManager);
                    break;
            }
        }

        private void UpdateUIForSingleCandidate(HintManager hintManager)
        {
            Hint hint = hintManager.Hint;
            
            void firstAction()
            {
                hintManager.HintHighlighter.SelectBlockHint(hint);
                hintManager.HintHighlighter.SelectInterferingCells(hint);
            }

            void secondAction()
            {
                hint.CellManager.CellHightlighter.HighlightCell(hint.CellManager, CellHightlighter.CellHighlightType.MainFocused);
            }

            ExecuteHintStep(hint, hintManager.ExplanationIndex, firstAction, secondAction);
        }

        private void UpdateUIForSinglePosition(HintManager hintManager)
        {
            Hint hint = hintManager.Hint;

            void firstAction()
            {
                hintManager.HintHighlighter.SelectBlockHint(hint);
                hintManager.HintHighlighter.SelectByLineXAndY(hint);
                hint.CellManager.CellHightlighter.HighlightCell(hint.CellManager, CellHightlighter.CellHighlightType.MainFocused);
            }

            void secondAction()
            {
                foreach (var cellManager in hint.NonRepeatingCellManagers)
                    cellManager.CellHightlighter.HighlightCell(cellManager, CellHightlighter.CellHighlightType.MinorFocused);
            }

            ExecuteHintStep(hint, hintManager.ExplanationIndex, firstAction, secondAction);
        }

        private void ExecuteHintStep(Hint hint, int index, Action firstAction, Action secondAction = null)
        {
            CellManager cellManager = hint.CellManager;
            switch (index)
            {
                case 0:
                    cellManager.CellUI.SwitchToNormalMode(hint.CellManager);
                    hint.GridBlock.SelectedBlockImage.SetEnabled(false);

                    firstAction.Invoke();
                    break;
                case 1:
                    firstAction.Invoke();
                    secondAction?.Invoke();

                    hint.GridBlock.SelectedBlockImage.SetEnabled(true);
                    cellManager.SetTextDirectly("");
                    cellManager.SetActiveVisibilityAnimation(false);
                    break;
                case 2:
                    cellManager.Text.gameObject.SetActive(true);
                    cellManager.SetTextDirectly(hint.Value.ToString());
                    cellManager.CellHightlighter.HighlightCell(cellManager, CellHightlighter.CellHighlightType.Right);
                    cellManager.SetActiveVisibilityAnimation(true);
                    break;
                default: break;
            }
        }

        #endregion

        #region SET

        private void SetRaycastTargetImages(Image[] images, bool value)
        {
            foreach (var image in images)
            {
                image.SetRaycastTarget(value);
            }
        }

        private void SetTransparencyImages(Image[] images, float value)
        {
            foreach (var image in images)
            {
                image.SetTransparency(value);
            }
        }

        #endregion
    }
}