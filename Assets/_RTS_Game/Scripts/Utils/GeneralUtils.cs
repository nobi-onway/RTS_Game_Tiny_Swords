using Unity.VisualScripting;
using UnityEngine;

public static class GeneralUtils
{
    public static void SetUpComponent<T>(Transform transform, ref T component) where T : Component
    {
        if (transform.TryGetComponent(out T componentTryToGet))
        {
            component = componentTryToGet;
            return;
        }

        component = transform.AddComponent<T>();
    }
}