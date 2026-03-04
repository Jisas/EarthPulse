using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UltimateFramework.UISystem
{
    public class ComboVisualsManager : MonoBehaviour
    {
        [SerializeField] private GameObject notificationPrefab;
        public List<NotificationSlot> notificationSlots = new();

        private TextMeshProUGUI firstText;
        private TextMeshProUGUI secondText;

        public void InstantiateMessage(string firstText, string secondText = default)
        {
            // Verificar si ambos slots están llenos
            bool allSlotsFull = true;
            foreach (NotificationSlot slot in notificationSlots)
            {
                if (slot.isEmpty)
                {
                    allSlotsFull = false;
                    break;
                }
            }

            // Si ambos slots están llenos, destruir el contenido del primer slot
            if (allSlotsFull)
            {
                if (notificationSlots[0].gameObject.transform.childCount > 0)
                {
                    Destroy(notificationSlots[0].gameObject.transform.GetChild(0).gameObject);
                    notificationSlots[0].isEmpty = true;
                }
            }

            // Instanciar el mensaje en el primer slot vacío
            foreach (NotificationSlot slot in notificationSlots)
            {
                if (slot.isEmpty && slot.gameObject.activeInHierarchy)
                {
                    var notification = Instantiate(notificationPrefab, slot.gameObject.transform);
                    var notificationManager = notification.GetComponent<ComboNotificationManager>();

                    this.firstText = notificationManager.firstText;
                    this.secondText = notificationManager.secondText;

                    UpdateComboVisuals(notificationManager, firstText, secondText);
                    slot.UpdateSlot();
                    break;
                }
            }
        }
        private void UpdateComboVisuals(ComboNotificationManager notificationManager, string firstText, string secondText = default)
        {
            this.firstText.text = firstText;

            if (!string.IsNullOrEmpty(secondText))
            {
                notificationManager.secondTextGroup.SetActive(true);
                this.secondText.text = secondText;
            }
        }
    }
}
