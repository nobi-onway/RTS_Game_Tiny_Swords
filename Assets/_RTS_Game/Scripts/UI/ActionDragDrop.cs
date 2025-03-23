using UnityEngine;
using UnityEngine.EventSystems;

public class ActionDragDrop : ActionCard, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        m_actionSO.PrepareExecute();
        OnFocus?.Invoke(m_actionSO);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_actionSO.Execute();
        
    }
}