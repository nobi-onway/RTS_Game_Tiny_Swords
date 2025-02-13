using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : SingletonManager<InputManager>
{
    private Vector2 m_initialTouchPosition;
    private bool m_isLeftClickOrTapDown => Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    private bool m_isLeftClickOrTapUp => Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);

    public Vector2 InputPosition => Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;

    private void Update()
    {
        if (TryGetShortClickPosition(out Vector2 inputPosition))
        {
            DetectClick(inputPosition);
        }
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

        if (HasClickOnUnit(hit, out Unit clickedUnit))
        {
            HandleClickOnUnit(clickedUnit);
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

    private bool HasClickOnUnit(RaycastHit2D hit, out Unit unit)
    {
        if (hit.collider != null && hit.collider.TryGetComponent(out Unit clickedUnit))
        {
            unit = clickedUnit;
            return true;
        }

        unit = null;
        return false;
    }

    private void HandleClickOnUnit(Unit unit)
    {
        GameManager.Instance.SelectUnit(unit);
    }

    private void HandleClickOnGround(Vector2 position)
    {
        GameManager.Instance.MoveActiveUnitTo(position);
    }
}