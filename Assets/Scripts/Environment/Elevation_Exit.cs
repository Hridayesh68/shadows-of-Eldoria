using UnityEngine;

public class Elevation_Exit : MonoBehaviour
{
    public Collider2D[] mountainColliders;
    public Collider2D[] boundaryColliders;
    public int groundSortingOrder = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        // Re-enable mountain collisions
        SetCollidersEnabled(mountainColliders, true);

        // Disable elevated boundary collisions
        SetCollidersEnabled(boundaryColliders, false);

        // Return player to ground sorting order (behind high-ground tiles)
        SpriteRenderer sr = collision.GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = collision.GetComponentInChildren<SpriteRenderer>();

        if (sr != null)
            sr.sortingOrder = groundSortingOrder;
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