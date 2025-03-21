using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : SingletonManager<InputManager>
{
    private const float PAN_SPEED = 50.0f;
    private const float MOBILE_PAN_SPEED = 10.0f;

    private Vector2 m_initialTouchPosition;
    private bool m_isLeftClickOrTapDown => Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    private bool m_isLeftClickOrTapUp => Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);
    public Vector2 InputPosition => Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;

    public bool IsLockPan { get; set; }
    public Action<Vector2, float> OnPanPosition;

    private void Update()
    {
        if (TryGetShortClickPosition(out Vector2 inputPosition))
        {
            DetectClick(inputPosition);
        }

        TryGetPanPosition();
    }

    private bool TryGetPanPosition()
    {
        if (IsLockPan) return false;

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            Vector2 normalizedDelta = touchDeltaPosition / new Vector2(Screen.width, Screen.height);

            OnPanPosition?.Invoke(normalizedDelta, MOBILE_PAN_SPEED);
            return true;
        }

        if (Input.touchCount == 0 && Input.GetMouseButton(0))
        {
            Vector2 mouseDeltaPosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            OnPanPosition?.Invoke(mouseDeltaPosition, PAN_SPEED);
            return true;
        }

        return false;
    }

    public bool TryGetHoldWorldPosition(out Vector2 worldPosition)
    {
        if (Input.touchCount > 0)
        {
            worldPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            return true;
        }

        if (Input.GetMouseButton(0))
        {
            worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return true;
        }

        worldPosition = Vector2.zero;
        return false;
    }

    private bool TryGetShortClickPosition(out Vector2 inputPosition, float maxDistance = 20.0f)
    {
        inputPosition = InputPosition;

        if (m_isLeftClickOrTapDown)
        {
            m_initialTouchPosition = InputPosition;
        }

        if (m_isLeftClickOrTapUp)
        {
            if (Vector2.Distance(m_initialTouchPosition, InputPosition) < maxDistance)
            {
                return true;
            }
        }

        return false;
    }

    private void DetectClick(Vector2 inputPosition)
    {
        if (IsPointerOverUIElement()) return;

        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (HasClickOn(hit, out Unit clickedUnit))
        {
            HandleClickOnUnit(clickedUnit);
        }
        else if (HasClickOn(hit, out Tree tree))
        {
            HandleClickOnTree(tree);
        }
        else
        {
            HandleClickOnGround(worldPosition);
        }
    }

    private bool IsPointerOverUIElement()
    {
        if (Input.touchCount > 0) return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

        return EventSystem.current.IsPointerOverGameObject();
    }

    private bool HasClickOn<T>(RaycastHit2D hit, out T unit)
    {
        if (hit.collider != null && hit.collider.TryGetComponent(out T clickedUnit))
        {
            unit = clickedUnit;
            return true;
        }

        unit = default(T);
        return false;
    }

    private void HandleClickOnTree(Tree tree)
    {
        GameManager.Instance.SendWorkerToChop(tree);
    }

    private void HandleClickOnUnit(Unit unit)
    {
        GameManager.Instance.SelectUnit(unit);
    }

    private void HandleClickOnGround(Vector2 position)
    {
        GameManager.Instance.ExecuteActiveUnit(position);
    }
}