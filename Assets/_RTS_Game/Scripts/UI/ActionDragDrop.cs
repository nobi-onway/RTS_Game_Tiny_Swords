using UnityEngine;
using UnityEngine.EventSystems;

public class ActionDragDrop : ActionCard, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        m_actionSO.PrepareExecute();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_actionSO.Execute();
    }
}