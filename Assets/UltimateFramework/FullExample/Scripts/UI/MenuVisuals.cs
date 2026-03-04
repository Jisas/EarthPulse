using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UltimateFramework.UISystem;
using UltimateFramework.SerializationSystem;
using UltimateFramework.Inputs;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using System;

public class MenuVisuals : RuntimeVisualsBase
{
    public GameObject HUDCanvas;
    public GameObject firstSelected;

    #region Private Fields
    private Dictionary<string, UnityAction> menuButtonsDictionary;
    #endregion

    #region Actions
    public static Action OnOpenInventory;
    public static Action OnOpenEquipment;
    public static Action OnOpenCharacter;
    public static Action OnOpenSettings;
    #endregion

    #region Mono
    private void OnEnable()
    {
        if (DataGameManager.IsDataSaved())
            SetInputPerformedActions();

        menuButtonsDictionary = new Dictionary<string, UnityAction>()
        {
            {"Inventory_Menu_Item", OnInventory},
            {"Equipment_Menu_Item", OnEquipment},
            {"Character_Menu_Item", Oncharacter},
            {"Settings_Menu_Item", OnSettings}
        };

        SetUpButtons();
    }
    private void OnDisable()
    {
        InputsManager.Player.OpenMenu.performed -= OpenMenuHandler;
        InputsManager.UI.CloseMenu.performed -= CancelMenuHandler;
        InputsManager.UI.Cancel.performed -= CancelMenuHandler;
    }
    #endregion

    #region Overrides
    protected override RuntimeVisualsBase PreviousWindow => null;
    protected override void OnShow()
    {
        if (ActiveWindow == null) InitializeFirstWindow(this);
        if (volume.profile.TryGet<DepthOfField>(out depthOfField)) depthOfField.active = false;
        EventSystem.current.SetSelectedGameObject(firstSelected);
        HUDCanvas.SetActive(false);
        base.OnShow();
    }
    protected override void OnHide()
    {
        base.OnHide();
        HUDCanvas.SetActive(true);
    }
    #endregion

    #region Callbacks
    private void OpenMenuHandler(InputAction.CallbackContext context)
    {
        if (!context.performed) 
            return;

        OnShow();
    }
    private void CancelMenuHandler(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (ActiveWindow == this)
        {
            ClearWindowStack();
            OnBack();
        }
    }
    #endregion

    #region Event Methods
    public void SetInputPerformedActions()
    {
        InputsManager.Player.OpenMenu.performed += OpenMenuHandler;
        InputsManager.UI.CloseMenu.performed += CancelMenuHandler;
        InputsManager.UI.Cancel.performed += CancelMenuHandler;
    }
    #endregion

    #region Internal
    private void SetUpButtons()
    {
        foreach (KeyValuePair<string, UnityAction> entry in menuButtonsDictionary)
        {
            var obj = UICanvas.transform.GetChild(0).Find(entry.Key);
            if (obj != null)
            {
                var button = obj.GetComponentInChildren<Button>(true);
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(entry.Value);
            }
        }
    }
    private void OnInventory()
    {
        OnHide();
        OnOpenInventory?.Invoke();
    }
    private void OnEquipment()
    {
        OnHide();
        OnOpenEquipment?.Invoke();
    }
    private void Oncharacter()
    {
        OnHide();
        OnOpenCharacter?.Invoke();
    }
    private void OnSettings()
    {
        OnHide();
        OnOpenSettings?.Invoke();
    }
    #endregion
}