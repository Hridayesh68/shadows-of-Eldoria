using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public Sprite buriedSprite;

    [Header("Movement & Damage")]
    public Vector2 direction = Vector2.right;
    public float speed = 10f;
    public float lifeSpan = 2f;
    public int damage = 1;

    [Header("Collision Layers")]
    public LayerMask enemyLayer;
    public LayerMask obstacleLayer;

    private bool hasHit;
    private Collider2D arrowCollider;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (sr == null)
            sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = GetComponentInChildren<SpriteRenderer>();

        arrowCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        ApplyVelocityAndRotation();
        Destroy(gameObject, lifeSpan);
    }

    public void SetDirection(Vector2 newDirection)
    {
        if (newDirection.sqrMagnitude > 0.001f)
            direction = newDirection.normalized;

        ApplyVelocityAndRotation();
    }

    private void ApplyVelocityAndRotation()
    {
        if (hasHit)
            return;

        Vector2 dir = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector2.right;

        if (rb != null)
            rb.linearVelocity = dir * speed;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleHit(collision);
    }

    private void HandleHit(Collider2D collision)
    {
        if (hasHit || collision == null || collision.CompareTag("Player"))
            return;

        if (IsInLayerMask(collision.gameObject.layer, enemyLayer) || collision.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.TakeDamage(damage);

            EnemyKnockback enemyKnockback = collision.GetComponentInParent<EnemyKnockback>();
            if (enemyKnockback != null)
                enemyKnockback.Knockback(direction.normalized);

            AttachToTarget(collision.transform);
        }
        else if (IsInLayerMask(collision.gameObject.layer, obstacleLayer) || collision.CompareTag("Wall"))
        {
            AttachToTarget(collision.transform);
        }
    }

    private void AttachToTarget(Transform target)
    {
        hasHit = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (arrowCollider != null)
            arrowCollider.enabled = false;

        if (sr != null && buriedSprite != null)
            sr.sprite = buriedSprite;

        if (target != null)
            transform.SetParent(target, true);
    }

    private static bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}