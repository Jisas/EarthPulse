using UltimateFramework.InventorySystem;
using UnityEngine.Rendering.Universal;
using UltimateFramework.UISystem;
using System.Collections.Generic;
using UltimateFramework.Inputs;
using UnityEngine.InputSystem;
using UltimateFramework.Tools;
using UltimateFramework.Utils;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;

public class ItemSelectionVisuals : RuntimeVisualsBase
{
    public GameObject HUDCanvas;
    public ScrollRect scrollRect;
    public Transform contentPanel;
    public InventoryAndEquipmentComponent inventoryAndEquipment;
    public Func<EquipmentSlot> SelectedSlot;

    private readonly int defaultSelectionIndex = 0;
    private readonly List<InventorySlot> itemSlots = new();

    #region Mono
    private void OnEnable()
    {
        InputsManager.UI.Cancel.performed += CancelInputHandler;
    }
    private void OnDisable()
    {
        InputsManager.UI.Cancel.performed -= CancelInputHandler;
    }
    #endregion

    #region Override
    protected override RuntimeVisualsBase PreviousWindow => FindObjectOfType<EquipmentVisuals>();
    protected override void OnShow()
    {
        base.OnShow();
        HUDCanvas.SetActive(false);
        CreateAndSetupItemSlots();
        SetSlotSelectionFunctions();
        StartCoroutine(DelayedSelectChild(defaultSelectionIndex));

        if (volume.profile.TryGet<DepthOfField>(out depthOfField)) 
            depthOfField.active = true;
    }
    #endregion

    #region Internal
    private IEnumerator DelayedSelectChild(int index)
    {
        yield return new WaitForEndOfFrame();
        SelectChild(index);
    }
    private void SelectChild(int index)
    {
        int childCount = scrollRect.content.childCount;
        if (index >= childCount) return;

        GameObject childObj = scrollRect.content.transform.GetChild(index).gameObject;
        InventorySlot slot = childObj.GetComponent<InventorySlot>();
        slot.ObtainSelectionFocus();
    }
    private void CreateAndSetupItemSlots()
    {
        itemSlots.Clear();
        if (contentPanel.childCount > 0)
        {
            for (int i = 0; i < contentPanel.childCount; i++)
            {
                Destroy(contentPanel.GetChild(i).gameObject);
            }
        }

        var itemSlotTag = SettingsMasterData.Instance.itemDB.FindItem(SelectedSlot().SlotTags[0].tag, false).itemSlot;
        var slots = inventoryAndEquipment.GetAllItemsByTagOnInventory(itemSlotTag);

        for (int i = 0; i < slots.Count; i++)
        {
            GameObject slot = Instantiate<GameObject>(inventoryAndEquipment.slotPrefab, contentPanel);
            InventorySlot newSlot = slot.GetComponent<InventorySlot>();
            newSlot.ItemDB = SettingsMasterData.Instance.itemDB;
            newSlot.SlotInfo = slots[i];
            newSlot.UpdateUI();
            itemSlots.Add(newSlot);
        }
    }
    private void SetSlotSelectionFunctions()
    {
        foreach (var slot in itemSlots)
        {
            var button = slot.GetComponent<Button>();
            var item = SettingsMasterData.Instance.itemDB.FindItem(slot.SlotInfo.itemId);

            if (item.type == ItemType.Weapon)
            {
                if (!SelectedSlot().SlotInfo.isEmpty)
                     button.onClick.AddListener(() => inventoryAndEquipment.SwitchWeapon(SelectedSlot().SlotInfo.itemId, slot.SlotInfo.itemId, SocketOrientation.Right));
                else button.onClick.AddListener(() => inventoryAndEquipment.EquipItem(item, 0, slot.SlotInfo.amount, false));
            }
            else 
            {
                int socketIndex = item.type == ItemType.Consumable ? 3 : 0;

                if (!SelectedSlot().SlotInfo.isEmpty)
                     button.onClick.AddListener(() => inventoryAndEquipment.SwitchItem(SelectedSlot().SlotInfo.itemId, slot.SlotInfo.itemId, slot.SlotInfo.amount));
                else button.onClick.AddListener(() => inventoryAndEquipment.EquipItem(item, socketIndex, slot.SlotInfo.amount));
            }

            button.onClick.AddListener(() => OnBack());
        }
    }

    #endregion

    #region Callbacks
    public void OnShowHandler() => OnShow();
    private void CancelInputHandler(InputAction.CallbackContext input)
    {
        if (!input.performed || ActiveWindow != this) return;
        OnBack();
    }
    #endregion
}