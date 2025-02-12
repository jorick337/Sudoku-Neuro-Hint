using System.Collections.Generic;

namespace Game.Classes
{
    public class Cell
    {
        #region CORE

        public int Block { get; private set; }
        public int Number { get; private set; }
        public int Value { get; private set; }

        public CellGroups CellGroups { get; private set; }
        public CellColors CellColors { get; private set; }

        public List<string> NotepadValues { get; private set; }

        public bool AddScoreForCorrectFilling { get; private set; }

        #endregion

        #region CONSTRUCTORS

        public Cell(int block, int number, int value, ColorTheme colorTheme, GridBlocks gridBlocks)
        {
            Block = block;
            Number = number;
            Value = value;

            CellColors = new(colorTheme);
            CellGroups = new(gridBlocks, this);

            NotepadValues = new();
            
            AddScoreForCorrectFilling = value != 0; // Если имеет значение с самого начала то не может получить очки за корректное получение
        }

        #endregion

        #region SET

        public void SetValue(int value) => Value = value;
        public void SetNotePadValues(List<string> values) => NotepadValues = values;
        
        public void SetCellColors(CellColors cellColors) => CellColors = cellColors;

        public void SetAddScoreForCorrectFilling(bool value) => AddScoreForCorrectFilling = value;

        #endregion
    }
}