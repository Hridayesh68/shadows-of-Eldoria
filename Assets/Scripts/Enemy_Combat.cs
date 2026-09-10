using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    public int damage = 1;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth player = other.gameObject.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.ChangeHealth(-damage);
                Debug.Log("Player took damage!");
            }
        }
    }
}