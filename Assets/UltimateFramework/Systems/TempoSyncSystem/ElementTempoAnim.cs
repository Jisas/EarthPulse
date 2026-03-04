using UltimateFramework.TempSyncSystem;
using UltimateFramework.Utils;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using MyBox;

namespace UltimateFramework.TempoSyncSystem
{
    public class ElementTempoAnim : MonoBehaviour
    {
        public TempoManager TempoManager;
        public ElementAnimationType animType;
        [MyBox.ConditionalField(nameof(animType), false, ElementAnimationType.Scale)]
        public Vector3 newScale;
        [MyBox.ConditionalField(nameof(animType), false, ElementAnimationType.Width)] 
        public float newWidth;

        RectTransform rectTransform;
        Vector3 initialScale;
        float initialWidth;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            initialWidth = rectTransform.rect.width;
            initialScale = transform.localScale;
        }
        private void OnEnable()
        {
            if (animType == ElementAnimationType.Width) TempoManager.OnVerifyByTempo += DoWidthAnim;
            if (animType == ElementAnimationType.Scale) TempoManager.OnVerifyByTempo += DoScaleAnim;
        }
        private void OnDisable()
        {
            if (animType == ElementAnimationType.Width) TempoManager.OnVerifyByTempo -= DoWidthAnim;
            if (animType == ElementAnimationType.Scale) TempoManager.OnVerifyByTempo -= DoScaleAnim;
        }

        public void DoWidthAnim() => StartCoroutine(DoWidthOnTempo(newWidth));
        public void DoScaleAnim() => StartCoroutine(DoScaleOnTempo(newScale));

        IEnumerator DoWidthOnTempo(float newValue)
        {
            var time = 0f;
            rectTransform.SetWidth(initialWidth);

            while (time < 0.25f)
            {
                time += Time.deltaTime;
                rectTransform.SetWidth(Mathf.Lerp(initialWidth, newValue, time));
                yield return null;
            }
        }
        IEnumerator DoScaleOnTempo(Vector3 newValue)
        {
            var time = 0f;
            transform.DOScale(newValue, 0);

            while (time < 0.25f)
            {
                time += Time.deltaTime;
                transform.DOScale(initialScale, 0.25f);
                yield return null;
            }
        }
    }
}
