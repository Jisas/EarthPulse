using UltimateFramework.TempSyncSystem;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using System;
using MyBox;

namespace UltimateFramework.TempoSyncSystem
{
    [Serializable]
    public class PracticeInputs : CollectionWrapper<InputPracticeListener> { }

    public class TempoVerifier : MonoBehaviour
    {
        public MissTutorialManager missTutorialManager;
        public Slider barSlider;
        public GameObject counter;
        [Space(5)]

        public bool isInteractive = false;
        public TempoManager tempoManager;
        [Space(5)]

        [ConditionalField(nameof(isInteractive), false, true)] public InputPatternChecker inputPatterns;
        [ConditionalField(nameof(isInteractive), false, true)] public int currentPattern;
        [ConditionalField(nameof(isInteractive), false, true)] public PracticeInputs inputRefs;
        [Space(5)] 

        public RectTransform movingRect;
        public List<RectTransform> targetRects = new();
        [Space(5)] 

        public UnityEvent OnWin;
        public UnityEvent OnMiss;
        public UnityEvent OnLastTempo;
        public List<Func<bool>> Checks = new();

        public bool GoPlayer { get; set; } = false;
        private int currentTargetIndex = 0;

        #region Mono
        private void Awake()
        {
            if (isInteractive)
            {
                for (int i = 0; i < inputPatterns.inputPatterns[currentPattern].pattern.Count; i++)
                {
                    inputRefs.Value[i].inputRef = inputPatterns.inputPatterns[currentPattern].pattern[i].input;
                }
            }
        }
        private void OnEnable()
        {
            if (isInteractive)
            {
                foreach (var inputRef in inputRefs.Value)
                {
                    var listener = inputRef.GetComponent<InputPracticeListener>();
                    Checks.Add(listener.Verifier);
                }

                var counterController = counter.GetComponent<Counter>();
                missTutorialManager.dialogueLines[0].onChangeLine.AddListener(counterController.ResetAndStartCounter);
                missTutorialManager.dialogueLines[0].onChangeLine.AddListener(resetBarsliderHandler);
                missTutorialManager.dialogueLines[0].onChangeLine.AddListener(this.ResetVerifier);
            }
            else
            {
                var counterController = counter.GetComponent<Counter>();
                var tempoBar = barSlider.gameObject.GetComponent<TempoBar>();
                counterController.OnFinishCount.AddListener(tempoBar.StartSliderLerp);

            }
        }
        private void OnDisable()
        {
            if (isInteractive)
            {
                missTutorialManager.dialogueLines[0].onChangeLine?.RemoveAllListeners();
                Checks.Clear();
            }
        }
        void Update()
        {
            if (currentTargetIndex >= targetRects.Count) return;
            RectTransform targetRect = targetRects[currentTargetIndex];

            if (!isInteractive)
            {
                if (IsRectTransformInside(movingRect, targetRect))
                {
                    targetRect.transform.GetComponentInParent<Image>().color = Color.yellow;
                    currentTargetIndex++;

                    if (currentTargetIndex == targetRects.Count && tempoManager.IsBeat(1))
                    {
                        tempoManager.ResetBeatExecuted();
                        OnLastTempo?.Invoke();
                    }
                }
            }
            else
            {
                if (GoPlayer)
                {
                    if (IsRectTransformInside(movingRect, targetRect) && Checks[currentTargetIndex]())
                    {
                        targetRect.transform.GetComponentInParent<Image>().color = Color.yellow;
                        currentTargetIndex++;

                        if (currentTargetIndex >= targetRects.Count)
                        {
                            GoPlayer = false;
                            OnWin?.Invoke();
                        }
                    }
                    else if (!IsRectTransformInside(movingRect, targetRect) && Checks[currentTargetIndex]())
                    {
                        GoPlayer = false;
                        OnMiss?.Invoke();
                    }
                }
            }
        }
        #endregion

        #region Internal
        private void resetBarsliderHandler() => barSlider.value = 0;
        bool IsRectTransformInside(RectTransform moving, RectTransform target)
        {
            Vector3[] movingCorners = new Vector3[4];
            Vector3[] targetCorners = new Vector3[4];

            moving.GetWorldCorners(movingCorners);
            target.GetWorldCorners(targetCorners);

            Rect targetRect = new Rect(targetCorners[0], targetCorners[2] - targetCorners[0]);

            foreach (Vector3 corner in movingCorners)
            {
                if (!targetRect.Contains(corner))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Public Methods
        public void ResetVerifier()
        {
            GoPlayer = false;
            currentTargetIndex = 0;

            foreach (RectTransform rect in targetRects)
                rect.transform.GetComponentInParent<Image>().color = Color.white;
        }
        public void EvaluateMiss()
        {
            if (currentTargetIndex < targetRects.Count)
                OnMiss?.Invoke();
        }
        #endregion
    }
}
