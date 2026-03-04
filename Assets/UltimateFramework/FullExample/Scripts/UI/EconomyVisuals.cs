using UltimateFramework.EconomySystem;
using UnityEngine;
using TMPro;

namespace UltimateFramework
{
    public class EconomyVisuals : MonoBehaviour
    {
        [SerializeField] private EconomyComponent entityEconomy;
        [SerializeField] private TextMeshProUGUI economyText;

        private void OnEnable() => entityEconomy.OnEconomyChange += UpdateEconomyUI;
        private void Start() => UpdateEconomyUI(entityEconomy.GetEconomy());
        public void UpdateEconomyUI(int amount) => economyText.text = amount.ToString();
    }
}
