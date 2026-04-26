using UnityEngine;

public class VillainAI : MonoBehaviour
{
    [Header("Player Target")]
    public Transform player;

    [Header("Movement & Detection")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float detectionRange = 10f;
    public float attackRange = 1.5f;
    public float attackRate = 1f;

    [Header("Combat")]
    public int attackDamage = 10;
    public int maxHealth = 100;

    [Header("Wandering")]
    public Transform wanderCenterObject;
    public float wanderRadius = 5f;

    private int currentHealth;
    private Vector3 wanderCenter;
    private Vector3 wanderTarget;
    private float nextAttackTime = 0f;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isDead = false;

    private Animator animator;
    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        currentHealth = maxHealth;

        wanderCenter = (wanderCenterObject != null) ? wanderCenterObject.position : transform.position;
        SetNewWanderTarget();
    }

    void Update()
    {
        if (isDead || isAttacking) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            WanderWithinArea();
        }
    }

    void WanderWithinArea()
    {
        if (isChasing || isDead) return;

        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);

        if (Vector3.Distance(transform.position, wanderTarget) < 1f)
        {
            SetNewWanderTarget();
        }

        MoveTowards(wanderTarget, walkSpeed);
    }

    void SetNewWanderTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += wanderCenter;
        randomDirection.y = transform.position.y;
        wanderTarget = randomDirection;
    }

    void ChasePlayer()
    {
        if (isDead) return;

        isChasing = true;

        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);
        animator.SetBool("isAttacking", false);

        MoveTowards(player.position, runSpeed);
    }

    void AttackPlayer()
    {
        if (isDead) return;

        isChasing = false;
        isAttacking = true;

        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", true);

        StopMovement();

        nextAttackTime = Time.time + attackRate;

        Invoke(nameof(ResetAttack), 1f); // Adjust based on animation timing
    }

    public void AttackEvent() // Call this in animation event
    {
        if (isAttacking && !isDead)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    void MoveTowards(Vector3 target, float moveSpeed)
    {
        if (isDead) return;

        Vector3 direction = (target - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        transform.LookAt(new Vector3(target.x, transform.position.y, target.z));
    }

    void StopMovement()
    {
        rb.velocity = Vector3.zero;
    }

    

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        isAttacking = false;
        isChasing = false;

        StopMovement();

        animator.SetTrigger("Die"); 
        Debug.Log("Villain is dying...");

        rb.isKinematic = true;
        if (col != null) col.enabled = false;

        this.enabled = false;

        Destroy(gameObject, 100f); 
    }
}
