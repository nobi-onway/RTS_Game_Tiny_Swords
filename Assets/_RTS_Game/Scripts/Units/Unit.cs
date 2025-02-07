using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    protected Animator animator;

    private void Awake()
    {
        SetUpComponent<Animator>(ref animator);
    }

    private void SetUpComponent<T>(ref T component) where T : Component
    {
        if (!TryGetComponent(out T componentTryToGet)) return;

        component = componentTryToGet;
    }
}