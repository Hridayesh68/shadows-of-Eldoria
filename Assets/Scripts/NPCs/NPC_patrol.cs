using System.Collections;
using UnityEngine;

public class NPC_patrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [Tooltip("Patrol destination points (X and Y coordinates)")]
    public Vector2[] patrolPoints;
    public float speed = 2f;
    public float pauseDuration = 2f;
    public float pointReachThreshold = 0.1f;

    [Header("References")]
    public Rigidbody2D rb;
    public Animator anim;

    [Header("State")]
    public bool isPaused = false;
    public int currentPatrolIndex = 0;

    [Header("Animation Parameters (Optional)")]
    [Tooltip("If using boolean or float parameters in Animator, specify names here")]
    public string walkAnimStateName = "pawn_running";
    public string idleAnimStateName = "pawn_idle";
    public string isMovingBoolParam = "isMoving";

    private Coroutine pauseCoroutine;

    private void Awake()
    {
        // Get Rigidbody2D on self if not assigned
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Get Animator in child if not assigned
        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        // If no points configured, use current position as single point
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            patrolPoints = new Vector2[] { transform.position };
        }
    }

    private void FixedUpdate()
    {
        // If NPC is paused, ensure velocity is 0 and idle animation is active
        if (isPaused)
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            PlayIdleAnimation();
            return;
        }

        MoveToCurrentPoint();
    }

    private void MoveToCurrentPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Vector2 currentTarget = patrolPoints[currentPatrolIndex];
        Vector2 currentPosition = transform.position;
        Vector2 direction = currentTarget - currentPosition;
        float distance = direction.magnitude;

        // Check if reached destination
        if (distance <= pointReachThreshold)
        {
            if (pauseCoroutine == null)
            {
                pauseCoroutine = StartCoroutine(PauseAtPointRoutine());
            }
            return;
        }

        // Move towards target point
        Vector2 moveDirection = direction.normalized;
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * speed;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, currentTarget, speed * Time.fixedDeltaTime);
        }

        // Flip character based on horizontal movement direction
        Flip(moveDirection.x);

        // Play walking animation between points
        PlayWalkAnimation();
    }

    private IEnumerator PauseAtPointRoutine()
    {
        // Toggle paused status to true
        isPaused = true;

        // Stop movement immediately
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        PlayIdleAnimation();

        // Wait pause duration
        yield return new WaitForSeconds(pauseDuration);

        // Update patrol index increment
        UpdatePatrolIndex();

        // Toggle paused status back
        isPaused = false;
        pauseCoroutine = null;
    }

    /// <summary>
    /// Updates the patrol index, incrementing it and wrapping around array.
    /// </summary>
    public void UpdatePatrolIndex()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    /// <summary>
    /// Flips character scale based on movement direction.
    /// </summary>
    private void Flip(float horizontalDirection)
    {
        if (Mathf.Abs(horizontalDirection) < 0.01f) return;

        Vector3 scale = transform.localScale;

        if (horizontalDirection > 0f)
        {
            // Moving right: set localScale.x positive
            scale.x = Mathf.Abs(scale.x);
        }
        else if (horizontalDirection < 0f)
        {
            // Moving left: set localScale.x to negative (-1 * absolute scale)
            scale.x = -Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }

    private void PlayWalkAnimation()
    {
        if (anim == null) return;

        // Support animator parameter if configured
        if (HasParameter(anim, isMovingBoolParam))
        {
            anim.SetBool(isMovingBoolParam, true);
        }

        // Also play direct animation state if state exists or animator has no transitions set up
        if (!string.IsNullOrEmpty(walkAnimStateName) && anim.HasState(0, Animator.StringToHash(walkAnimStateName)))
        {
            anim.Play(walkAnimStateName);
        }
    }

    private void PlayIdleAnimation()
    {
        if (anim == null) return;

        if (HasParameter(anim, isMovingBoolParam))
        {
            anim.SetBool(isMovingBoolParam, false);
        }

        if (!string.IsNullOrEmpty(idleAnimStateName) && anim.HasState(0, Animator.StringToHash(idleAnimStateName)))
        {
            anim.Play(idleAnimStateName);
        }
    }

    private static bool HasParameter(Animator animator, string paramName)
    {
        if (animator == null || string.IsNullOrEmpty(paramName)) return false;

        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            Gizmos.DrawSphere(patrolPoints[i], 0.2f);
            int nextIndex = (i + 1) % patrolPoints.Length;
            Gizmos.DrawLine(patrolPoints[i], patrolPoints[nextIndex]);
        }
    }
}
