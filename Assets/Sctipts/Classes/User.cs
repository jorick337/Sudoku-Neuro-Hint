using System.Collections.Generic;

namespace Game.Classes
{
    public class User
    {
        public string Username { get; set; }
        public Sudoku UnfinishedSudoku { get; set; }
        public List<Record> Records { get; set; } = new();

        public User(string username = "")
        {
            Username = username;
        }

        public User(User user)
        {
            Username = user.Username;
            Records = new List<Record>(user.Records);
            UnfinishedSudoku = user.UnfinishedSudoku;
        }
    }
}