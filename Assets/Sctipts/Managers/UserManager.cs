using System.Collections.Generic;
using System.Linq;
using Game.Classes;
using UnityEngine;

namespace Game.Managers
{
    public class UserManager : MonoBehaviour
    {
        #region SINGLETON

        public static UserManager Instance { get; private set; }

        #endregion

        #region CORE

        [Header("Managers")]
        [SerializeField] private SaveManager saveManager;
        [SerializeField] private AppSettingsManager appSettings;

        public User User { get; private set; }
        public List<User> Users { get; private set; }

        #endregion

        #region MONO

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            InitializeValues();
            Load();
        }

        #endregion

        #region INITIALIZATION

        private void InitializeValues()
        {
            string defaultUserName = appSettings.AppSettingData.DefaultUsername;
            User = new(defaultUserName);
            Users = new();
        }

        #endregion

        #region CORE LOGIC

        public void SaveGameProgress(Sudoku sudoku)
        {
            SetSudoku(sudoku);
            AddRecord(sudoku.Record);
            Debug.Log(123);
            saveManager.SaveUsers(Users);
        }

        private void Load()
        {
            List<User> users = saveManager.LoadUsers();
            if (users != null)
            {
                foreach (var user in users)
                {
                    if (!IsUsernameRepetition(user.Username))
                    {
                        AddUser(user);

                        if (User.Username == user.Username) // Если это "Sudoku"
                        {
                            User = user;
                        }
                    }
                }
            }
        }

        #endregion

        #region SET

        public void SetSudoku(Sudoku sudoku) => User.UnfinishedSudoku = sudoku;
        public void SetUser(User user) => User = user;

        #endregion

        #region BOOL

        public bool IsUsernameRepetition(string UserName) => Users != null && Users.Any(user => user.Username == UserName);
        public bool IsUnfinishedSudokuNull() => User.UnfinishedSudoku == null;

        #endregion

        #region ADD

        public void AddUser(User user) => Users.Add(user);
        public void AddRecord(Record Record) => User.Records.Add(Record);

        #endregion
    }
}