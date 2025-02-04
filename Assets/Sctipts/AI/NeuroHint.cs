using UnityEngine;

namespace Game.AI
{
    public class NeuroHint
    {
        public int Value { get; private set; }
        public int Block { get; private set; }
        public int Number { get; private set; }
        public float Probability { get; private set; }

        public NeuroHint(int value, int block, int number, float probability)
        {
            Value = value;
            Block = block;
            Number = number;
            Probability = probability;
        }
    }
}