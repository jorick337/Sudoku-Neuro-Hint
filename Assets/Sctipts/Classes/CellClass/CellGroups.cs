using Game.Managers;
using Help.Classes;

namespace Game.Classes
{
    public struct CellGroups
    {
        public CellManager[] All { get; private set; }
        public CellManager[] Block { get; private set; }
        public CellManager[] LineX { get; private set; }
        public CellManager[] LineY { get; private set; }

        public CellGroups(GridBlocks gridBlocks, Cell cell)
        {
            All = gridBlocks.AllCellManagers;
            Block = gridBlocks.Blocks[cell.Block].CellManagers;
            LineX = gridBlocks.Blocks.GetCellManagers(cell.Block / 3 * 3, 3, 1, cell.Number / 3 * 3, 3, 1);
            LineY = gridBlocks.Blocks.GetCellManagers(cell.Block % 3, 9, 3, cell.Number % 3, 9, 3);
        }
    }
}