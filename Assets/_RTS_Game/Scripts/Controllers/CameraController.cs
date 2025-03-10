using UnityEngine;

public class CameraController : MonoBehaviour
{
    private void OnEnable()
    {
        InputManager.Instance.OnPanPosition += UpdatePosition;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnPanPosition -= UpdatePosition;
    }

    private void UpdatePosition(Vector2 deltaPosition, float panSpeed)
    {
        this.transform.Translate(-deltaPosition * Time.deltaTime * panSpeed);
    }
}