using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

namespace UltimateFramework.TempoSyncSystem
{
    [RequireComponent(typeof(Slider))]
    public class TempoBar : MonoBehaviour
    {
        //public float startDelay = 0;
        public float duration = 2.5f;
        [Space(5)] public UnityEvent OnFinish;
        private Slider slider;

        private void Awake() => slider = GetComponent<Slider>();

        public void StartSliderLerp()
        {
            if (this.gameObject.activeInHierarchy) 
                StartCoroutine(LerpNumber());
        }
        public void StopBar() => StopAllCoroutines();

        IEnumerator LerpNumber()
        {
            //yield return new WaitForSeconds(startDelay);

            float elapsedTime = 0f;
            float startValue = slider.minValue;
            float endValue = slider.maxValue;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                slider.value = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
                yield return null;
            }

            OnFinish?.Invoke();
        }
    }
}
