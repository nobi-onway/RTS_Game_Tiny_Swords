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

    public static Vector3 GetTopPosition(Transform transform)
    {
        if (!transform.TryGetComponent(out Collider2D collider2D)) return transform.position;

        return transform.position + Vector3.up * (collider2D.bounds.size.y / 2);
    }
}