using UltimateFramework.InventorySystem;
using UnityEngine.Rendering.Universal;
using UltimateFramework.ItemSystem;
using UltimateFramework.UISystem;
using System.Collections.Generic;
using UltimateFramework.Inputs;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UltimateFramework.Tools;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UltimateFramework;

public class InventoryVisuals : RuntimeVisualsBase
{
    public GameObject HUDCanvas;
    public GameObject firstSelected;
    public InventoryAndEquipmentComponent inventoryAndEquipment;

    [Header("Add Item Notification")]
    [SerializeField] private GameObject notificationPrefab;
    public List<NotificationSlot> notificationSlots = new();

    [Header("UI Data")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;

    [Header("SrollControll")]
    public ScrollRect scrollRect;
    public RectTransform maskRect;

    private readonly int defaultSelectionIndex = 0;

    #region Mono
    private void OnEnable()
    {
        InputsManager.UI.Cancel.performed += CancelInputHandler;
        MenuVisuals.OnOpenInventory += OnShow;
        inventoryAndEquipment.OnAddItemToInventory += InstantiateMessage;
    }
    private void LateUpdate()
    {
        if (ActiveWindow == this)
        {
            if (EventSystem.current.currentSelectedGameObject.TryGetComponent<InventorySlot>(out var currentSlot))
            {
                var item = SettingsMasterData.Instance.itemDB.FindItem(currentSlot.SlotInfo.itemId);
                if (item != null)
                {
                    itemNameText.text = item.name;
                    itemDescriptionText.text = item.description;
                }
                else
                {
                    itemNameText.text = string.Empty;
                    itemDescriptionText.text = string.Empty;
                }
            }
        }
    }
    private void OnDisable()
    {
        InputsManager.UI.Cancel.performed -= CancelInputHandler;
        MenuVisuals.OnOpenInventory -= OnShow;
        inventoryAndEquipment.OnAddItemToInventory -= InstantiateMessage;
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
    public void InstantiateMessage(Item item, int amount)
    {
        bool allSlotsFull = true;
        foreach (NotificationSlot slot in notificationSlots)
        {
            // Verificar si todos los slots están llenos
            if (slot.isEmpty)
            {
                allSlotsFull = false;
                break;
            }
        }

        // Si todos los slots están llenos, destruir el contenido del primer slot
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
                var notificationManager = notification.GetComponent<InventoryNotificationManager>();

                UpdateNotficationVisuals(notificationManager, item.name, item.icon, amount);
                slot.UpdateSlot();
                break;
            }
        }
    }

    private void UpdateNotficationVisuals(InventoryNotificationManager manager, string itemName, Sprite itemImage, int itemAmount)
    {
        manager.nameText.text = itemName;
        manager.AmountText.text = itemAmount.ToString();
        manager.iconImage.sprite = itemImage;
    }
    #endregion

    #region Overriddes
    protected override RuntimeVisualsBase PreviousWindow => FindObjectOfType<MenuVisuals>();
    protected override void OnShow()
    {
        base.OnShow();
        HUDCanvas.SetActive(false);
        StartCoroutine(DelayedSelectChild(defaultSelectionIndex));
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

}