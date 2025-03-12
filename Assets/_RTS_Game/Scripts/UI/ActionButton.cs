using UnityEngine;
using UnityEngine.UI;

public class ActionButton : ActionCard
{
    [SerializeField] private Button m_button;

    private void OnDestroy()
    {
        m_button.onClick.RemoveAllListeners();
    }

    public override void Init(ActionSO actionSO)
    {
        base.Init(actionSO);

        m_button.onClick.AddListener(() => OnButtonPressed(actionSO));
    }

    private void OnButtonPressed(ActionSO actionSO)
    {
        actionSO.Execute();
        OnFocus?.Invoke(actionSO);
    }
}