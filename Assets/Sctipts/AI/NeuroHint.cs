namespace Game.AI
{
    public class NeuroHint
    {
        public int Value { get; private set; }
        public float Probability { get; private set; }
        public int Block { get; private set; }
        public int Number { get; private set; }

        public NeuroHint(int value, float probability, int block, int number)
        {
            Value = value;
            Probability = probability;
            Block = block;
            Number = number;
        }
    }
}