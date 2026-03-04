using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

namespace UltimateFramework
{
    public class UIDescriptionElement : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private TextMeshProUGUI m_Text;
        [TextArea(5, 10)] public string description;

        public void OnSelect(BaseEventData eventData)
        {
            m_Text.text = description;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            m_Text.text = string.Empty;
        }
    }
}
