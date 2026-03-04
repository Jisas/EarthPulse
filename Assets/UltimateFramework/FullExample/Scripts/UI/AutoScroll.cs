using System.Collections.Generic;
using UltimateFramework.Inputs;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(ScrollRect))]
public class AutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scrollSpeed = 10f;
    private bool mouseOver = false;

    private ScrollRect m_ScrollRect;
    private Vector2 m_NextScrollPosition = Vector2.up;
    private readonly List<Selectable> m_Selectables = new();

    #region Mono
    void Awake()
    {
        m_ScrollRect = GetComponent<ScrollRect>();
    }
    void OnEnable()
    {
        if (m_ScrollRect) m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
    }
    void Start()
    {
        if (m_ScrollRect) m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
        ScrollToSelected(true);
    }
    void Update()
    {
        // Scroll via input.
        InputScroll();

        if (!mouseOver) m_ScrollRect.normalizedPosition = Vector2.Lerp(m_ScrollRect.normalizedPosition, m_NextScrollPosition, scrollSpeed * Time.unscaledDeltaTime);
        else m_NextScrollPosition = m_ScrollRect.normalizedPosition;
    }
    #endregion

    #region Internal
    void InputScroll()
    {
        if (m_Selectables.Count > 0)
            if (InputsManager.UI.Navigate.ReadValue<Vector2>().y != 0.0f) 
                ScrollToSelected(false);
    }
    void ScrollToSelected(bool quickScroll)
    {
        int selectedIndex = -1;
        Selectable selectedElement = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;

        if (selectedElement) selectedIndex = m_Selectables.IndexOf(selectedElement);
        if (selectedIndex > -1)
        {
            if (quickScroll)
            {
                m_ScrollRect.normalizedPosition = new Vector2(0, 1 - (selectedIndex / ((float)m_Selectables.Count - 1)));
                m_NextScrollPosition = m_ScrollRect.normalizedPosition;
            }
            else m_NextScrollPosition = new Vector2(0, (1 - selectedIndex / ((float)m_Selectables.Count - 1)));
        }
    }
    #endregion

    #region Interface Implementation
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        ScrollToSelected(false);
    }
    #endregion
}