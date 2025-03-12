using System.Collections.Generic;
using UnityEngine;

public class ActionBar : MonoBehaviour
{
    private List<ActionCard> m_actionCardList = new();

    private void SpawnActionButtons(ActionSO[] actions, int curActionIdx)
    {
        Clear();

        foreach (ActionSO action in actions)
        {
            ActionCard actionCardClone = Instantiate(action.ActionCardPrefab, this.transform);
            actionCardClone.Init(action, FocusAction);

            m_actionCardList.Add(actionCardClone);
        }

        FocusAction(curActionIdx);
    }

    private void FocusAction(int idx)
    {
        UnfocusAllAction();

        m_actionCardList[idx].Focus();
    }

    private void FocusAction(ActionSO actionSO)
    {
        UnfocusAllAction();

        m_actionCardList.Find(action => action.m_actionSO.Guid == actionSO.Guid).Focus();
    }

    private void UnfocusAllAction()
    {
        foreach (ActionCard actionCard in m_actionCardList)
        {
            actionCard.Unfocus();
        }
    }

    public void Show(ActionSO[] actions, int curActionIdx)
    {
        SpawnActionButtons(actions, curActionIdx);
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private void Clear()
    {
        foreach (RectTransform child in this.transform)
        {
            Destroy(child.gameObject);
        }

        m_actionCardList.Clear();
    }
}