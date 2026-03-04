using UltimateFramework.Inputs;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine;

namespace UltimateFramework.UISystem
{
    public class CustomSelectableNavigate : MonoBehaviour
    {
        [SerializeField] private GameObject rightSelection;
        [SerializeField] private GameObject leftSelection;

        public void OnEnable()
        {
            InputsManager.UI.RightCustomNavigate.performed += RightSelectHandler;
            InputsManager.UI.LeftCustomNavigate.performed += LeftSelectHandler;
        }
        public void OnDisable()
        {
            InputsManager.UI.RightCustomNavigate.performed -= RightSelectHandler;
            InputsManager.UI.LeftCustomNavigate.performed -= LeftSelectHandler;
        }

        private void RightSelectHandler(InputAction.CallbackContext context)
        {
            EventSystem.current.SetSelectedGameObject(rightSelection);
        }
        private void LeftSelectHandler(InputAction.CallbackContext context)
        {
            Debug.Log("Entre");
            EventSystem.current.SetSelectedGameObject(leftSelection);
        }
    }
}
