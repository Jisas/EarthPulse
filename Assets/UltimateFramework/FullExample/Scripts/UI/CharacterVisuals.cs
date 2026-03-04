using UltimateFramework.StatisticsSystem;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UltimateFramework.UISystem;
using UltimateFramework.Inputs;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine;
using System;
using TMPro;
using UltimateFramework.InventorySystem;

[Serializable]
public struct UIElementComposition
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI valueText;
}

public class CharacterVisuals : RuntimeVisualsBase
{
    public GameObject HUDCanvas;
    public StatisticsComponent statisticsComponent;
    public InventoryAndEquipmentComponent inventoryAndEquipmentComponent;

    [Header("Attributes")]
    [SerializeField] private UIElementComposition[] primaryAttributeCompositions;

    [Header("Stats")]
    [SerializeField] private UIElementComposition[] statsCompositions;
    [SerializeField] private TagSelector weaponDamageTag;
    [SerializeField] private TextMeshProUGUI weaponDamageValueText;

    [Header("Defence")]
    [SerializeField] private string tagKeyWord;
    [SerializeField] private string ignoreKeyWord;
    [SerializeField] private UIElementComposition[] defenceAttributeCompositions;

    #region Mono
    private void OnEnable()
    {
        InputsManager.UI.Cancel.performed += CancelInputHandler;
        MenuVisuals.OnOpenCharacter += OnShow;
    }
    private void OnDisable()
    {
        InputsManager.UI.Cancel.performed -= CancelInputHandler;
        MenuVisuals.OnOpenCharacter -= OnShow;
    }
    #endregion

    #region Overriddes
    protected override RuntimeVisualsBase PreviousWindow => FindObjectOfType<MenuVisuals>();
    protected override void OnShow()
    {
        base.OnShow();
        UpdateInfo();
        HUDCanvas.SetActive(false);
        if (volume.profile.TryGet<DepthOfField>(out depthOfField)) depthOfField.active = true;
    }
    #endregion

    #region Callbacks
    private void CancelInputHandler(InputAction.CallbackContext input)
    {
        if (!input.performed || ActiveWindow != this) return;
        OnBack();
    }
    #endregion

    #region Internal
    private void UpdateInfo()
    {
        UpdatePrimaryAttInfo();
        UpdateStatsInfo();
        UpdateDefeceInfo();
    }
    private void UpdatePrimaryAttInfo()
    {
        for (int i = 0; i < statisticsComponent.primaryAttributes.Count; i++)
        {
            string[] splitTag = statisticsComponent.primaryAttributes[i].attributeType.tag.Split('.');
            if (splitTag.Length > 0)
            {
                primaryAttributeCompositions[i].nameText.text = $"{splitTag[1]}:";
                primaryAttributeCompositions[i].valueText.text = statisticsComponent.primaryAttributes[i].CurrentValue.ToString();
            }
        }
    }
    private void UpdateStatsInfo()
    {
        for (int i = 0; i < statisticsComponent.stats.Count; i++)
        {
            string[] splitTag = statisticsComponent.stats[i].statType.tag.Split('.');
            if (splitTag.Length > 0)
            {
                statsCompositions[i].nameText.text = $"{splitTag[1]}:";
                statsCompositions[i].valueText.text = statisticsComponent.stats[i].CurrentValue.ToString();
            }
        }

        var item = inventoryAndEquipmentComponent.GetCurrentMainWeapon().WeaponComponent.Item;
        weaponDamageValueText.text = item.FindStat(weaponDamageTag.tag).CurrentValue.ToString();
    }
    private void UpdateDefeceInfo()
    {
        var tags = new List<string>();
        var values = new List<string>();

        foreach (var attribute in statisticsComponent.attributes)
        {
            string[] splitTag = attribute.attributeType.tag.Split('.');

            if (splitTag.Length > 0)
            {
                if (splitTag[1] == tagKeyWord && splitTag[2] != ignoreKeyWord)
                {
                    tags.Add(splitTag[2]);
                    values.Add(attribute.CurrentValue.ToString());
                }
                else continue;
            }
        }

        for (int i = 0; i < defenceAttributeCompositions.Length; i++)
        {
            defenceAttributeCompositions[i].nameText.text = $"{tags[i]}:";
            defenceAttributeCompositions[i].valueText.text = values[i];
        }
    }
    #endregion
}
