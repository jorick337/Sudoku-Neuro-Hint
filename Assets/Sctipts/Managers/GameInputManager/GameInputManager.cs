using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Managers
{
    public class GameInputManager : MonoBehaviour
    {
        #region SINGLETON

        public static GameInputManager Instance { get; private set; }

        #endregion

        #region EVENTS

        public event Action OnNumberPressed;
        public event Action OnDeletePressed;
        public event Action OnLeftClick;

        #endregion

        private ControlsInputBtns _inputActions;

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

        private void OnEnable()
        {
            InitializeValues();
            RegisterEvents(true);
        }

        private void OnDisable()
        {
            RegisterEvents(false);
        }

        #endregion

        #region INITIALIZATION

        private void InitializeValues()
        {
            _inputActions = new ControlsInputBtns();
            _inputActions.Enable();
        }

        private void RegisterEvents(bool register)
        {
            if (register)
            {
                _inputActions.OneToNine.Numbers.started += HandleOnNumberPressed;
                _inputActions.OneToNine.Delete.started += HandleOnDeletePressed;
                _inputActions.Mouse.LeftClick.started += HandleOnLeftClickPressed;
            }
            else
            {
                if (_inputActions != null)
                {
                    _inputActions.OneToNine.Numbers.started -= HandleOnNumberPressed;
                    _inputActions.OneToNine.Delete.started -= HandleOnDeletePressed;
                    _inputActions.Mouse.LeftClick.started -= HandleOnLeftClickPressed;
                }
            }
        }

        #endregion

        #region CALLBACKS

        private void HandleOnNumberPressed(InputAction.CallbackContext context) => OnNumberPressed?.Invoke();
        private void HandleOnDeletePressed(InputAction.CallbackContext context) => OnDeletePressed?.Invoke();
        private void HandleOnLeftClickPressed(InputAction.CallbackContext context) => OnLeftClick?.Invoke();

        #endregion
    }
}