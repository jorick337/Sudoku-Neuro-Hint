using System.Collections.Generic;
using System.Linq;
using Game.Managers;
using UnityEngine;

namespace Game.Classes
{
    public class GridBlocks : MonoBehaviour
    {
        #region CORE

        [Header("Core")]
        [SerializeField] private GridBlock[] gridBlocks;

        public GridBlock[] Blocks => gridBlocks;
        public CellManager[] AllCellManagers => gridBlocks
            .SelectMany(gridBlock => gridBlock.CellManagers)
            .ToArray();

        public CellManager FocusedCellManager { get; private set; }
        public Stack<Movement> MovesHistory { get; private set; } // История всех действий с ячейками

        public bool IsNotepadeMode { get; private set; }
        public bool IsPaused { get; private set; }

        #endregion

        #region MONO

        private void OnEnable()
        {
            MovesHistory = new Stack<Movement>();
            IsNotepadeMode = false;
            IsPaused = false;
        }

        #endregion

        #region SET

        public void SetFocusedCellManager(CellManager cellManager) => FocusedCellManager = cellManager;
        public void SetValueFocusedCell(string value) => FocusedCellManager.SetTextWithValidate(value);

        public void SetIsNotepadMode(bool value) => IsNotepadeMode = value;
        public void SetIsPause(bool value) => IsPaused = value;

        #endregion
    }
}