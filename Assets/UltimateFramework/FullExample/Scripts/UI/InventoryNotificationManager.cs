using UltimateFramework.UISystem;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace UltimateFramework
{
    public class InventoryNotificationManager : MonoBehaviour
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI AmountText;
        public Image iconImage;
        public float waitTimeToExit;

        private void Start() => StartCoroutine(WaitToExit());
        private IEnumerator WaitToExit()
        {
            yield return new WaitForSeconds(waitTimeToExit);
            if (transform.parent.TryGetComponent<NotificationSlot>(out var slot))
            {
                slot.UpdateSlot(true);
                Destroy(this.gameObject);
            }
        }
    }
}
