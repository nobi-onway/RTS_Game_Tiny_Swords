using UnityEngine;

public class ActionBar : MonoBehaviour
{
    [SerializeField] private ActionButton m_actionButtonPrefab;

    private void SpawnActionButtons(ActionSO[] actions)
    {
        Clear();

        foreach (ActionSO action in actions)
        {
            ActionButton actionButtonClone = Instantiate(m_actionButtonPrefab, this.transform);
            actionButtonClone.Init(action);
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