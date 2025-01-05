using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Game.Classes;
using Game.Managers.Help;

namespace Game.Managers
{
    public class SaveManager : MonoBehaviour
    {
        #region SINGLETON

        public static SaveManager Instance { get; private set; }

        #endregion

        #region CORE

        private SaveSerializator _saveSerializator;
        private SaveDeserializator _saveDeserializator;

        private string _savePathSettings;
        private string _savePathUsers;

        #endregion

        #region MONO

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);

                InitializeValues();
            }
            else
                Destroy(gameObject);
        }

        #endregion

        #region INITIALIZATION

        private void InitializeValues()
        {
            _saveSerializator = new();
            _saveDeserializator = new();

            _savePathSettings = Path.Combine(Application.persistentDataPath, "Settings.json");
            _savePathUsers = Path.Combine(Application.persistentDataPath, "Users.json");
        }

        #endregion

        #region SAVE

        public void SaveAppSettingsData(AppSettingData appSettingData)
        {
            JArray usersJArray = _saveSerializator.SerializeSettings(appSettingData);
            string json = usersJArray.ToString(Formatting.Indented);

            File.WriteAllText(_savePathSettings, json);
        }

        public void SaveUsers(List<User> users)
        {
            JArray usersJArray = _saveSerializator.SerializeUsers(users);
            string json = usersJArray.ToString(Formatting.Indented);

            File.WriteAllText(_savePathUsers, json);
        }

        #endregion

        #region LOAD

        public AppSettingData LoadAppSettingsData()
        {
            if (File.Exists(_savePathSettings))
            {
                string json = File.ReadAllText(_savePathSettings);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    JArray settingsJArray = JArray.Parse(json);
                    return _saveDeserializator.DeserializeSettings(settingsJArray);
                }
            }
            return null;
        }

        public List<User> LoadUsers()
        {
            if (File.Exists(_savePathUsers))
            {
                string json = File.ReadAllText(_savePathUsers);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    JArray usersJArray = JArray.Parse(json);
                    return _saveDeserializator.DeserializeUsers(usersJArray);
                }
            }
            return null;
        }

        #endregion
    }
}