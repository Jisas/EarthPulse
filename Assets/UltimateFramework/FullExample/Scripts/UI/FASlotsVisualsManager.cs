using UltimateFramework.InventorySystem;
using System.Collections.Generic;
using UltimateFramework.Tools;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using UltimateFramework.Inputs;
using UnityEngine.InputSystem;

[Serializable]
public class FastAccessSlot
{
    public Image icon;
    public TextMeshProUGUI amontText;
    public TextMeshProUGUI itemNameText;
    public TagSelector tag;
    public EquipmentSlot MyEquipmentSlot { get; set; }
}

public class FASlotsVisualsManager : MonoBehaviour
{
    public List<FastAccessSlot> fastAccessSlots;

    private readonly Dictionary<string, FastAccessSlot> tagToSlotMap = new();
    private InventoryAndEquipmentComponent inventoryAndEquipmentComponent;

    #region Mono
    private void OnEnable()
    {
        InputsManager.Player.SwitchConsumable.performed += SwitchConsumableInputHandler;
    }
    void Start()
    {
        MapDictionary();
        inventoryAndEquipmentComponent = transform.root.GetComponent<InventoryAndEquipmentComponent>();
        StartCoroutine(SetupSlotsDelayed());
    }
    #endregion

    #region Internal
    private void MapDictionary()
    {
        // Crear un diccionario para acceso rápido a los slots por tag
        foreach (var slot in fastAccessSlots)
        {
            if (!tagToSlotMap.ContainsKey(slot.tag.tag))
            {
                tagToSlotMap.Add(slot.tag.tag, slot);
            }
        }
    }
    private IEnumerator SetupSlotsDelayed()
    {
        yield return new WaitForEndOfFrame();

        var equipmentSlots = inventoryAndEquipmentComponent.EquipSlots;

        // Asignar iconos y textos a los slots correspondientes
        foreach (var equipSlot in equipmentSlots)
        {
            if (equipSlot.Selected)
            {
                foreach (var slotTag in equipSlot.SlotTags)
                {
                    if (tagToSlotMap.TryGetValue(slotTag.tag, out var fastAccessSlot))
                    {
                        equipSlot.OnUpdateFAUI -= UpdateFAUI;
                        equipSlot.OnUpdateFAUI += UpdateFAUI;

                        var item = SettingsMasterData.Instance.itemDB.FindItem(equipSlot.SlotInfo.itemId);

                        fastAccessSlot.icon.enabled = true;
                        fastAccessSlot.icon.sprite = item.icon;
                        fastAccessSlot.itemNameText.text = item.name;
                        fastAccessSlot.MyEquipmentSlot = equipSlot;
                        fastAccessSlot.amontText.text = equipSlot.SlotInfo.amount.ToString();
                    }
                }
            }        
        }
    }
    private void UpdateFAUI()
    {
        foreach (var faSlot in fastAccessSlots)
        {
            if (tagToSlotMap.TryGetValue(faSlot.tag.tag, out var fastAccessSlot))
            {
                if(!faSlot.MyEquipmentSlot.SlotInfo.isEmpty)
                {
                    fastAccessSlot.icon.enabled = true;
                    fastAccessSlot.icon.sprite = faSlot.MyEquipmentSlot.itemImage.sprite;
                    fastAccessSlot.amontText.text = faSlot.MyEquipmentSlot.SlotInfo.amount.ToString();
                }
                else
                {
                    fastAccessSlot.icon.enabled = false;
                    fastAccessSlot.icon.sprite = null;
                    fastAccessSlot.itemNameText.text = string.Empty;
                    fastAccessSlot.amontText.text = string.Empty;
                }
            }
        }
    }
    #endregion

    #region Callbacks
    private void SwitchConsumableInputHandler(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        inventoryAndEquipmentComponent.SwitchCosumableSelectedSlot();
        StartCoroutine(SetupSlotsDelayed());
    }
    #endregion
}