using UnityEngine;

public class Elevation_Exit : MonoBehaviour
{
    public Collider2D[] mountainColliders;
    public Collider2D[] boundaryColliders;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Disable mountain collisions
            foreach (Collider2D mountain in mountainColliders)
            {
                mountain.enabled = true;
            }

            // Enable boundary collisions
            foreach (Collider2D boundary in boundaryColliders)
            {
                boundary.enabled = false;
            }

            // Put player behind the mountain
            collision.GetComponent<SpriteRenderer>().sortingOrder = 10;
        }
    }

 
    
}