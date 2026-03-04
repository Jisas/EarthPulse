using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace UltimateFramework.UISystem
{
    public class ComboNotificationManager : MonoBehaviour
    {
        public TextMeshProUGUI firstText;
        public TextMeshProUGUI secondText;
        public GameObject secondTextGroup;
        public float waitTimeToExit;

        private Animator animator;
        private int _animIDExit;

        private void Start()
        {
            animator = GetComponent<Animator>();
            AssignAnimationIDs();
            StartCoroutine(WaitToExit());
        }

        private void AssignAnimationIDs()
        {
            _animIDExit = Animator.StringToHash("Exit");
        }

        private IEnumerator WaitToExit()
        {
            yield return new WaitForSeconds(waitTimeToExit);
            animator.SetBool(_animIDExit, true);
            DoExit();
        }
        private void DoExit()
        {
            if (transform.parent.TryGetComponent<NotificationSlot>(out var slot))
            {
                slot.UpdateSlot(true);
                Destroy(this.gameObject);
            }
        }
    }
}