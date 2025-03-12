using UnityEngine;

public class PointToInteract : PointToClick
{
    protected override void OnEffectComplete()
    {
        base.OnEffectComplete();

        Destroy(gameObject);
    }


    public void DisplayAt(Vector2 position, Sprite sprite)
    {
        m_spriteRenderer.sprite = sprite;

        DisplayAt(position);
    }
}