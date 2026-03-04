using UltimateFramework.InventorySystem;
using UnityEngine.Rendering.Universal;
using UltimateFramework.UISystem;
using UltimateFramework.Inputs;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UltimateFramework.Tools;
using UnityEngine.Rendering;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UltimateFramework.Utils;
using UltimateFramework.ItemSystem;

public class EquipmentVisuals : RuntimeVisualsBase
{
    public GameObject HUDCanvas;
    public InventoryAndEquipmentComponent inventoryAndEquipment;
    public GameObject firstSelected;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    [Space] public UnityEvent onSelectEquipSlot;

    private ItemSelectionVisuals itemSelectionVisuals;

    #region Mono
    private void OnEnable()
    {
        InputsManager.UI.Cancel.performed += CancelInputHandler;
        InputsManager.UI.Unequip.performed += UnequipInputHandler;
        MenuVisuals.OnOpenEquipment += OnShow;

        itemSelectionVisuals = FindAnyObjectByType<ItemSelectionVisuals>();
    }
    private void LateUpdate()
    {
        if (ActiveWindow == this)
        {
            if (EventSystem.current.currentSelectedGameObject.TryGetComponent<EquipmentSlot>(out var currentSlot))
            {
                var item = SettingsMasterData.Instance.itemDB.FindItem(currentSlot.SlotInfo.itemId);
                if (item != null)
                {
                    itemNameText.text = item.name;
                    itemDescriptionText.text = item.description;
                    itemSelectionVisuals.SelectedSlot = () => currentSlot;
                }
                else
                {
                    itemNameText.text = string.Empty;
                    itemDescriptionText.text = string.Empty;
                    itemSelectionVisuals.SelectedSlot = () => currentSlot;
                }
            }
        }
    }
    private void OnDisable()
    {
        InputsManager.UI.Cancel.performed -= CancelInputHandler;
        InputsManager.UI.Unequip.performed -= UnequipInputHandler;
        MenuVisuals.OnOpenEquipment -= OnShow;
    }
    #endregion

    #region Overriddes
    protected override RuntimeVisualsBase PreviousWindow => FindObjectOfType<MenuVisuals>();
    protected override void OnShow()
    {
        base.OnShow();
        HUDCanvas.SetActive(false);
        SetSlotSelectionFunctions();
        EventSystem.current.SetSelectedGameObject(firstSelected);
        if (volume.profile.TryGet<DepthOfField>(out depthOfField)) depthOfField.active = true;
    }
    #endregion

    #region Internal
    private void SetSlotSelectionFunctions()
    {
        var equipSlot = inventoryAndEquipment.EquipSlots;

        foreach (var slot in equipSlot)
        {
            if (slot.TryGetComponent<Button>(out var button))
            {
                var item = SettingsMasterData.Instance.itemDB.FindItem(slot.SlotInfo.itemId);
                if (item == null || item.type != ItemType.Weapon) button.onClick.AddListener(onSelectEquipSlot.Invoke);
            }
        }
    }
    private void UnequipItem()
    {
        if (EventSystem.current.currentSelectedGameObject.TryGetComponent<EquipmentSlot>(out var currentSlot))
        {
            if (!currentSlot.SlotInfo.isEmpty)
            {
                var item = SettingsMasterData.Instance.itemDB.FindItem(currentSlot.SlotInfo.itemId);

                if (item.type == ItemType.Weapon) 
                     inventoryAndEquipment.UnequipWeapon(currentSlot.SlotInfo.itemId, currentSlot.SlotInfo.amount, currentSlot);
                else inventoryAndEquipment.UnequipItem(currentSlot.SlotInfo.itemId, currentSlot.SlotInfo.amount);

                currentSlot.UpdateFAUI();
            }
        }
    }
    #endregion

    #region Callbacks
    public void OnShowHandler() => OnShow();
    public void OnHideHandler() => OnHide();
    private void CancelInputHandler(InputAction.CallbackContext input)
    {
        if (!input.performed || ActiveWindow != this) return;
        OnBack();
    }
    private void UnequipInputHandler(InputAction.CallbackContext input)
    {
        if (!input.performed || ActiveWindow != this) return;
        UnequipItem();
    }
    #endregion
}