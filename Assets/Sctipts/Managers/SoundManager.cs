using System;
using UnityEngine;

namespace Game.Managers
{
    public class SoundManager : MonoBehaviour
    {
        #region SINGLETON

        public static SoundManager Instance { get; private set; }

        #endregion

        #region CORE

        [Header("Core")]
        [SerializeField] private AudioSource backgroundAudioSource;
        [SerializeField] private AudioSource leftClickAudioSource;

        private AudioSource[] _audioSources;

        [Header("Managers")]
        [SerializeField] private GameInputManager gameInputManager;
        [SerializeField] private AppSettingsManager appSettings;

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
            UpdateGeneralSound(appSettings.AppSettingData.DefaultSound);
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
            _audioSources = new AudioSource[2] 
                { backgroundAudioSource, leftClickAudioSource };
        }

        private void RegisterEvents(bool register)
        {
            if (register) 
                gameInputManager.OnLeftClick += PlayLeftClick;
            else
                gameInputManager.OnLeftClick -= PlayLeftClick;
        }

        #endregion

        #region CORE LOGIC

        public void UpdateGeneralSound(float value)
        {
            Array.ForEach(_audioSources, audioSource => audioSource.volume = value);
            appSettings.SetDefaultSound(value);
        }

        #endregion

        #region CALLBACKS

        private void PlayLeftClick() => leftClickAudioSource.Play();

        #endregion
    }
}