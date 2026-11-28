using UnityEngine;

public class Player_Bow : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    public GameObject arrow;
    public Transform launchPoint;

    [Header("Settings")]
    public float fireRate = 0.5f;
    public int archerLayerIndex = 1;

    private Vector2 aimDirection = Vector2.right;
    private float nextFire = 0f;
    private bool isShooting;

    private void Awake()
    {
        if (anim == null)
            anim = GetComponent<Animator>();
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        isShooting = false;

        if (anim != null)
            anim.SetLayerWeight(archerLayerIndex, 1f);
    }

    private void OnDisable()
    {
        isShooting = false;

        if (anim != null)
        {
            anim.SetBool("isShooting", false);
            anim.SetLayerWeight(archerLayerIndex, 0f);
        }
    }

    private void Update()
    {
        HandleAiming();

        if (Input.GetMouseButtonDown(0) && !isShooting && Time.time >= nextFire)
        {
            StartShoot();
        }
    }

    private void HandleAiming()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 inputDir = new Vector2(horizontal, vertical);
        if (inputDir.sqrMagnitude > 0.001f)
        {
            aimDirection = inputDir.normalized;
        }

        if (anim != null)
        {
            anim.SetFloat("aimX", aimDirection.x);
            anim.SetFloat("aimY", aimDirection.y);
        }
    }

    private void StartShoot()
    {
        isShooting = true;
        nextFire = Time.time + fireRate;

        if (anim != null)
        {
            anim.SetBool("isShooting", true);
        }
        else
        {
            Shoot();
        }
    }

    // Call this from an Animation Event on the arrow-release frame
    public void Shoot()
    {
        if (arrow == null || launchPoint == null)
        {
            Debug.LogWarning("Player_Bow: arrow prefab or launchPoint is not assigned!");
        }
        else
        {
            GameObject spawnedArrow = Instantiate(arrow, launchPoint.position, Quaternion.identity);
            Arrow arrowScript = spawnedArrow.GetComponent<Arrow>();
            if (arrowScript != null)
            {
                arrowScript.SetDirection(aimDirection);
            }
        }

        isShooting = false;

        if (anim != null)
        {
            anim.SetBool("isShooting", false);
        }
    }
}