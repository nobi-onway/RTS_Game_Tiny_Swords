using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    [SerializeField]
    private Unit m_activeUnit;
    private bool m_hasActiveUnit => m_activeUnit != null;

    private Vector2 m_initialTouchPosition;

    private bool m_isBeginTouch => Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    private bool m_isEndTouch => Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);

    private void Update()
    {
        Vector2 inputPosition = Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;

        if (m_isBeginTouch)
        {
            m_initialTouchPosition = inputPosition;
        }

        if (m_isEndTouch)
        {
            if (Vector2.Distance(m_initialTouchPosition, inputPosition) < 10)
            {
                DetectClick(inputPosition);
            }
        }
    }

    private void DetectClick(Vector2 inputPosition)
    {
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
        SelectNewUnit(unit);
    }

    private void HandleClickOnGround(Vector2 position)
    {
        if (m_hasActiveUnit)
        {
            m_activeUnit.MoveTo(position);
        }
    }

    private void SelectNewUnit(Unit unit)
    {
        if (m_hasActiveUnit)
        {
            m_activeUnit.Deselect();
        }

        m_activeUnit = unit;
        m_activeUnit.Select();
    }
}