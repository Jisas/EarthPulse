using UltimateFramework.LocomotionSystem;
using UltimateFramework.ActionsSystem;
using UltimateFramework.Utils;
using UnityEngine;
using System;

namespace UltimateFramework.TempSyncSystem
{
    public class TempoManager : MonoBehaviour
    {
        #region PublicValues
        [Header("General")]
        public bool isInteractiveObject = false;
        [MyBox.ConditionalField(nameof(isInteractiveObject), false, true)] public float speedThreshold = 3.0f;
        public float bpm = 120f;

        [Header("Visuals")]
        public bool useVisuals;
        [ConditionalField("useVisuals", true)] public HUDManager HUDManager;
        #endregion

        #region Properties
        public int BeatCount { get; private set; } = 0;
        public float NextBeatTime { get => nextBeatTime; }
        public int CurrentBeatOnAction { get; set; } = 0;
        #endregion

        #region Delegates And Events
        public delegate void VerifyByTempoHandler();
        public event VerifyByTempoHandler OnVerifyByTempo;

        public delegate void VerifyByTwoTemposHandler();
        public event VerifyByTwoTemposHandler OnVerifyBytwoTempos;

        public delegate void VerifyByCompasHandler();
        public event VerifyByCompasHandler OnVerifyByCompas;
        #endregion

        #region PrivateValues
        private BaseLocomotionComponent m_Locomotion;
        private ActionsComponent m_Actions;
        private Animator animator;

        private float beatInterval;
        private float nextBeatTime;
        private bool beatExecuted = false;
        #endregion

        #region Mono
        private void Awake()
        {
            if (isInteractiveObject)
            {
                m_Locomotion = GetComponent<BaseLocomotionComponent>();
                m_Actions = GetComponent<ActionsComponent>();
                animator = GetComponent<Animator>();
            }
        }
        void Start()
        {
            beatInterval = 60f / bpm;
            nextBeatTime = Time.time + beatInterval;
            if (isInteractiveObject) animator.speed = 1f / beatInterval;
        }
        void Update()
        {
            if (Time.time >= nextBeatTime)
            {
                nextBeatTime += beatInterval;
                BeatCount++;

                if (useVisuals)
                {
                    StartCoroutine(HUDManager.UpdateMarker());
                }
                
                OnVerifyByTempo?.Invoke();

                if (BeatCount % 2 == 0) OnVerifyBytwoTempos?.Invoke();
                if (CurrentBeatOnAction > 0 && BeatCount == (CurrentBeatOnAction + 4))
                {
                    OnVerifyByCompas?.Invoke();
                    CurrentBeatOnAction = 0;
                }

                if (isInteractiveObject)
                {
                    if (m_Locomotion.CurrentSpeed < speedThreshold) return;
                    if (m_Actions.CurrentAction != null && m_Actions.CurrentAction.IsExecuting) return;

                    string state = m_Locomotion.CurrentLocomotionType == Utils.LocomotionType.ForwardFacing ? "FFM" : "EightDirectional";
                    animator.Play(state, -1, 0f);
                }
            }
        }
        #endregion

        #region PublicMethods
        public bool IsBeat(int tempo)
        {
            if (BeatCount % tempo == 0 && !beatExecuted)
            {
                beatExecuted = true;
                return true;
            }
            return false;
        }
        public void ResetBeatExecuted()
        {
            beatExecuted = false;
        }
        #endregion
    }
}
