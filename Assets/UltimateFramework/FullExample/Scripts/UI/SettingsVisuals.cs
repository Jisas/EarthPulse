using UnityEngine.Rendering.Universal;
using UltimateFramework.UISystem;
using UltimateFramework.Inputs;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UltimateFramework;
using UnityEngine;

public class SettingsVisuals : RuntimeVisualsBase
{
    public GameObject HUDCanvas;
    public GameObject firstSelected;

    private void OnEnable()
    {
        InputsManager.UI.Cancel.performed += HandleCancelInput;
        MenuVisuals.OnOpenSettings += OnShow;
        MainMenuVisuals.OnOpenSetting += OnShow;
    }
    private void OnDisable()
    {
        InputsManager.UI.Cancel.performed -= HandleCancelInput;
        MenuVisuals.OnOpenSettings -= OnShow;
        MainMenuVisuals.OnOpenSetting -= OnShow;
    }

    #region Overriddes
    protected override RuntimeVisualsBase PreviousWindow 
    {
        get 
        {
            RuntimeVisualsBase menu = FindObjectOfType<MenuVisuals>();
            if (menu == null) menu = FindObjectOfType<MainMenuVisuals>();
            return menu;
        }
    }
    protected override void OnShow()
    {
        base.OnShow();
        if (HUDCanvas != null) HUDCanvas.SetActive(false);
        EventSystem.current.SetSelectedGameObject(firstSelected);
        if (volume != null && volume.profile.TryGet<DepthOfField>(out depthOfField)) depthOfField.active = true;
    }
    #endregion

    #region Callbacks
    private void HandleCancelInput(InputAction.CallbackContext input)
    {
        if (!input.performed || ActiveWindow != this) return;
        OnBack();
    }
    #endregion
}
