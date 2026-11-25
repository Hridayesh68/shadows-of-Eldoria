using UnityEngine;

public class Elevation : MonoBehaviour
{
    public Collider2D[] mountainColliders;
    public Collider2D[] boundaryColliders;
    public int elevatedSortingOrder = 15;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        SetCollidersEnabled(mountainColliders, false);
        SetCollidersEnabled(boundaryColliders, true);

        SpriteRenderer sr = collision.GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = collision.GetComponentInChildren<SpriteRenderer>();

        if (sr != null)
            sr.sortingOrder = elevatedSortingOrder;
    }

    private static void SetCollidersEnabled(Collider2D[] colliders, bool enabled)
    {
        if (colliders == null)
            return;

        foreach (Collider2D col in colliders)
        {
            if (col != null)
                col.enabled = enabled;
        }
    }
}