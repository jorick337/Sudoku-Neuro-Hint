using Unity.Mathematics;
using UnityEngine;

namespace Game.Classes
{
    [System.Serializable]
    public class Record
    {
        public int Level { get; private set; }
        public int NumberOfMistakes { get; private set; }
        public int NumberOfHints { get; private set; }
        public float TimeOfSolution { get; private set; }
        public int Score { get; private set; }

        public Record(int level, int mistakes, int hints, float time, int score)
        {
            Level = level;
            NumberOfMistakes = mistakes;
            NumberOfHints = hints;
            TimeOfSolution = time;
            Score = score;
        }

        public void SetNumberOfHints(int value) => NumberOfHints = value;

        public string GetTimeOfSolution()
        {
            int minutes = Mathf.FloorToInt(TimeOfSolution / 60);
            int seconds = Mathf.FloorToInt(TimeOfSolution % 60);

            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        public void AddMistake() => NumberOfMistakes += 1;
        public void AddTime(float value) => TimeOfSolution += value;
        public void AddScore(int value) => Score = Mathf.Max(0, Score + value);
    }
}