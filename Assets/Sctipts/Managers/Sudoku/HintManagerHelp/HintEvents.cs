using Game.Classes;
using Help.UI;

namespace Game.Managers.Help
{
    public class HintEvents
    {
        #region BUTTONS

        public void RegisterOnClickForPreviousStep(HintManager hintManager)
        {
            void MoveToPreviousStep()
            {
                hintManager.SetTransparencyIndicator(HintManager.TRANSPARENCY_INACTIVE);
                hintManager.SetExplanationIndex(hintManager.ExplanationIndex - 1);

                if (hintManager.ExplanationIndex == 0)
                    hintManager.ComeBackButton.gameObject.SetActive(false);

                hintManager.HintUI.UpdateExplanationAndIndicators(hintManager);
            }

            hintManager.SetOnClickPreviousStep(MoveToPreviousStep);
            hintManager.ComeBackButton.onClick.AddListener(MoveToPreviousStep);
        }

        public void RegisterOnClickForNextStep(HintManager hintManager)
        {
            void MoveToNextStep()
            {
                hintManager.SetTransparencyIndicator(HintManager.TRANSPARENCY_INACTIVE);
                hintManager.SetExplanationIndex(hintManager.ExplanationIndex + 1);

                if (hintManager.ExplanationIndex == 1)
                    hintManager.ComeBackButton.gameObject.SetActive(true);

                if (hintManager.ExplanationIndex == 3)
                    ImproveToExitMode(hintManager);
                else
                    hintManager.HintUI.UpdateExplanationAndIndicators(hintManager);
            }

            hintManager.SetOnClickNextStep(MoveToNextStep);
            hintManager.NextButton.onClick.AddListener(MoveToNextStep);
        }

        public void ImproveToExitMode(HintManager hintManager)
        {
            GridBlocks gridBlocks = GridManager.Instance.GridBlocks;

            Hint hint = hintManager.Hint;
            foreach (CellManager cellManager in gridBlocks.AllCellManagers)
                cellManager.Text.gameObject.SetActive(true);

            bool isNotepadMode = gridBlocks.IsNotepadeMode;
            gridBlocks.SetIsNotepadMode(false);

            hint.CellManager.CellAdd.AddValueWithoutMoveAndChecks(hint.CellManager, hint.Value);

            hint.GridBlock.SelectedBlockImage.SetEnabled(false);
            hint.CellManager.SetActiveVisibilityAnimation(false);
            hintManager.HintUI.ActiveHintUI(hintManager, false);

            gridBlocks.SetIsNotepadMode(isNotepadMode);
        }

        #endregion

        #region Colors

        public void RegisterUpdateColors(HintManager hintManager, ColorThemeManager colorThemeManager)
        {
            void UpdateColors()
            {
                hintManager.HintAdd.AddColorsToExplanations(hintManager);
            }

            hintManager.SetUpdateColors(UpdateColors);
            colorThemeManager.ChangingColorTheme += UpdateColors;
        }

        #endregion
    }
}