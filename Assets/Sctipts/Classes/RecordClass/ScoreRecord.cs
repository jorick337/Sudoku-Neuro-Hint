using UnityEngine;

namespace Game.Classes
{
    [System.Serializable]
    public struct ScoreRecordPoints
    {
        [Header("Plus")]
        public int FillCorrectly;
        public int LevelFinished;
        public int QuickFinish;


        [Header("Minus")]
        public int WrongFill;
        public int HintTaken;
        public int RevertMove;
    }
}
