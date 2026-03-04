using System.Collections.Generic;
using UltimateFramework.UISystem;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using System;
using UltimateFramework.SerializationSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using System.Collections;

namespace UltimateFramework
{
    public class MainMenuVisuals : RuntimeVisualsBase
    {
        #region Public Fields
        public GameObject buttonsGroup;
        public GameObject firstSelectedA;
        public GameObject firstSelectedB;
        public GameObject loadGameMessaje;
        public GameObject warningPanel;
        public Button acceptButton;
        public GameObject cancelButton;
        public SceneManagerController sceneManager;
        public Animator menuAnimator;
        public AnimationEvents animationEvent;
        #endregion

        #region Private Fields
        private Dictionary<string, UnityAction> menuButtonsDictionary;
        #endregion

        #region Actions
        public static Action OnOpenSetting;
        public static Action OnOpenCredits;
        #endregion

        #region Mono
        private void Awake()
        {
            menuAnimator.SetBool("Enter", true);
            SetAnimationListeners();
        }
        private void OnEnable()
        {
            menuButtonsDictionary = new Dictionary<string, UnityAction>()
            {
                {"LoadGame_Button", LoadGame},
                {"NewGame_Button", NewGame},
                {"Options_Button", Settings},
                {"Credits_Button", Credits},
                {"Exit_Button", Exit}
            };

            SetUpButtons();
        }
        #endregion

        #region Overrides
        protected override RuntimeVisualsBase PreviousWindow => null;
        protected override void OnShow()
        {
            if (ActiveWindow == null) InitializeFirstWindow(this);

            if (DataGameManager.IsDataSaved())
                 EventSystem.current.SetSelectedGameObject(firstSelectedA);
            else EventSystem.current.SetSelectedGameObject(firstSelectedB);

            base.OnShow();
        }
        #endregion

        #region Internal
        private void SetAnimationListeners()
        {
            animationEvent.animationEvent.AddListener(OnAnimationEvents);
        }
        private void OnAnimationEvents(string eventName)
        {
            switch (eventName)
            {
                case "on_show":
                    OnShow();
                    break;
            }
        }
        private void SetUpButtons()
        {
            foreach (KeyValuePair<string, UnityAction> entry in menuButtonsDictionary)
            {
                var obj = buttonsGroup.transform.Find(entry.Key);
                if (obj != null)
                {
                    var button = obj.GetComponentInChildren<Button>(true);
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(entry.Value);
                }
            }
        }
        private void LoadGame()
        {
            if (DataGameManager.IsDataSaved()) sceneManager.LoadScene(1);
            else StartCoroutine(MessageCoroutine());
        }
        private void NewGame()
        {
            if (DataGameManager.IsDataSaved())
            {
                buttonsGroup.SetActive(false);
                warningPanel.SetActive(true);
                loadGameMessaje.SetActive(false);

                acceptButton.onClick.RemoveAllListeners();
                acceptButton.onClick.AddListener(DataGameManager.Instance.DeleteAllGameData);
                acceptButton.onClick.AddListener(() => warningPanel.SetActive(false));
                acceptButton.onClick.AddListener(() => buttonsGroup.SetActive(true));
                acceptButton.onClick.AddListener(() => sceneManager.LoadScene(1));

                EventSystem.current.SetSelectedGameObject(cancelButton);
            }
            else sceneManager.LoadScene(1);
        }
        private void Settings()
        {
            OnHide();
            menuAnimator.SetBool("Enter", false);
            OnOpenSetting?.Invoke();
        }
        private void Credits()
        {
            //OnHide();
            //OnOpenCredits?.Invoke();
        }
        private void Exit()
        {
            DataGameManager.Instance.SaveSettingsData();
            Application.Quit();
        }
        private IEnumerator MessageCoroutine()
        {
            loadGameMessaje.SetActive(true);
            yield return new WaitForSeconds(5);
            loadGameMessaje.SetActive(false);
        }
        #endregion
    }
}
