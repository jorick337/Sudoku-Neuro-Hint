namespace Game.Classes
{
    public struct Movement
    {
        public int Block { get; private set; }
        public int Number { get; private set; }
        public int Value { get; private set; }

        public Movement(Cell cell)
        {
            Block = cell.Block;
            Number = cell.Number;
            Value = cell.Value;
        }
    }
}