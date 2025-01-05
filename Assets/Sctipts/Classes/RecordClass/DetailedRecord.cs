using UnityEngine;

namespace Game.Classes
{
    public class DetailedRecord
    {
        public string Username;
        public Record Record;
        public GameObject RecordRow;

        public DetailedRecord(string username, Record record)
        {
            Username = username;
            Record = record;
        }

        public string[] GetStringValues() => new[]
        {
            Username,
            Record.Level.ToString(),
            Record.NumberOfMistakes.ToString(),
            Record.NumberOfHints.ToString(),
            Record.GetTimeOfSolution(),
            Record.Score.ToString()
        };
    }
}