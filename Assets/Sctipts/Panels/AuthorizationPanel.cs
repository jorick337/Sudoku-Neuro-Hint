using UnityEngine;
using UnityEngine.UI;
using Game.Managers;
using Help.UI;
using Game.Classes;

namespace Game.Panels
{
    public class AuthorizationPanel : MonoBehaviour
    {
        #region CONSTANTS

        private const string CONTINUE_TEXT = "Продолжить";
        private const string CHANGE_TEXT = "Сменить";
        private const string USERNAME_LABEL = "Имя пользователя:\n {0}";

        #endregion

        #region CORE

        [Header("Core")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private InputField inputField;
        [SerializeField] private Button loginButton;
        [SerializeField] private Text loginText;
        [SerializeField] private GameObject duplicateUsernameWarning;

        private bool _isWarningEnabled;

        [Header("Linked Objects")]
        [SerializeField] private Text userInfoText;
        [SerializeField] private Image sceneBlocker;
        [SerializeField] private NewGamePanel newGamePanel;

        private UserManager _userManager;

        #endregion

        #region MONO

        private void Start()
        {
            User user = _userManager.User;
            UpdateUI(user.Username);
        }

        private void OnEnable()
        {
            InitializeManagers();
            InitializeValues();
            RegisterEvents(true);
        }

        private void OnDisable()
        {
            RegisterEvents(false);
        }

        #endregion

        #region INITIALIZATION

        private void InitializeManagers()
        {
            _userManager = UserManager.Instance;
        }

        private void InitializeValues()
        {
            _isWarningEnabled = false;
        }

        private void RegisterEvents(bool register)
        {
            if (register)
                loginButton.onClick.AddListener(ValidateUserInput);
            else
                loginButton.onClick.RemoveListener(ValidateUserInput);
        }

        #endregion

        #region CORE LOGIC

        public void ValidateUserInput()
        {
            string username = inputField.GetText();
            if (string.IsNullOrEmpty(username))
                return;

            if (_isWarningEnabled)
                FinalizeAuthorization(new User(username));
            else if (_userManager.IsUsernameRepetition(username))
                ShowWarning(true);
            else
                RegisterNewUser(new User(username));
        }

        private void FinalizeAuthorization(User user)
        {
            _userManager.SetUser(user);
            CompleteAuthorization(user.Username);
        }

        private void RegisterNewUser(User user)
        {
            _userManager.AddUser(user);
            _userManager.SetUser(user);
            CompleteAuthorization(user.Username);
        }

        #endregion

        #region UI UPDATES

        private void CompleteAuthorization(string username)
        {
            UpdateUI(username);
            canvas.SetSortingOrder(0);
            newGamePanel?.UpdateUIToDefault();
        }

        private void UpdateUI(string username)
        {
            userInfoText.SetText(string.Format(USERNAME_LABEL, username));
            inputField.SetText(username);
            sceneBlocker.SetRaycastTarget(false);
            ShowWarning(false);
        }

        private void ShowWarning(bool isActive)
        {
            _isWarningEnabled = isActive;
            duplicateUsernameWarning.SetActive(isActive);
            loginText.SetText(isActive ? CONTINUE_TEXT : CHANGE_TEXT);
        }

        #endregion
    }
}