using UnityEngine;

public class ActionBar : MonoBehaviour
{
    private void SpawnActionButtons(ActionSO[] actions)
    {
        Clear();

        foreach (ActionSO action in actions)
        {
            ActionCard actionCardClone = Instantiate(action.ActionCardPrefab, this.transform);
            actionCardClone.Init(action);
        }
    }

    public void Show(ActionSO[] actions)
    {
        SpawnActionButtons(actions);
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
    }
}