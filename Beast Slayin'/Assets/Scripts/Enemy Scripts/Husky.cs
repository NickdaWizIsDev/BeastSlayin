using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Wanderer;

public class Husky : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform core;
    public float projectileSpeed = 20f;
    public float fireRate = 2f;
    public float chargeTime = 1.5f;
    public float attackRange = 15f;
    public float runRange = 2f;
    public float fleeSpeed = 5f;
    private float timeSinceLastFire = 0f;
    private bool isCharging = false;
    private bool isMoving;

    public bool IsMoving 
    { 
        get 
        { return isMoving; }
        private set
        {
            isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    public float distanceToPlayer;

    public enum WalkableDirection { Right, Left }

    private WalkableDirection walkDirection;
    private Vector2 walkDirectionVector;
    public WalkableDirection WalkDirection
    {
        get { return walkDirection; }
        set
        {
            if (walkDirection != value)
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if (value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }
            walkDirection = value;
        }
    }

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        timeSinceLastFire += Time.deltaTime;
        if (timeSinceLastFire >= fireRate)
        {
            timeSinceLastFire = 0f;
            if (!isCharging && IsPlayerInRange(runRange))
            {
                StartCoroutine(Flee());
            }
            else if (!isCharging && IsPlayerInRange(attackRange))
            {
                StartCoroutine(ChargeAndFire());
            }
        }

        if (!IsMoving)
        {
            rb.velocity = Vector2.zero;
        }
    }

    bool IsPlayerInRange(float range)
    {
        distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        return distanceToPlayer <= range;
    }

    IEnumerator Flee()
    {
        IsMoving = true;

        // Calculate the direction away from the player
        Vector2 direction = (transform.position - player.position).normalized;
        WalkDirection = (direction.x > 0f) ? WalkableDirection.Right : WalkableDirection.Left;

        // Flip the local scale based on the walking direction
        float scaleX = (WalkDirection == WalkableDirection.Left) ? 1f : -1f;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);

        // Apply velocity to flee from the player
        rb.velocity = direction * fleeSpeed;

        yield return new WaitForSeconds(0.5f);

        while (IsPlayerInRange(runRange))
        {
            yield return null;
        }

        IsMoving = false;
    }

    IEnumerator ChargeAndFire()
    {
        isCharging = true;

        // Face towards the player
        Vector2 direction = (player.position - transform.position).normalized;
        WalkDirection = (direction.x > 0f) ? WalkableDirection.Right : WalkableDirection.Left;
        float scaleX = (WalkDirection == WalkableDirection.Left) ? 1f : -1f;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);

        // Charge for a certain duration
        animator.SetTrigger(AnimationStrings.atk);
        yield return new WaitForSeconds(chargeTime);

        // Fire projectile towards the player
        FireProjectile();

        isCharging = false;
    }

    void FireProjectile()
    {
        Transform core = transform.Find("Core"); // Find the Core child object

        if (core != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, core.position, Quaternion.identity);
            Vector2 direction = (player.position - core.position).normalized;
            projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;

            // Calculate the rotation angle in degrees based on the launch direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply the rotation to the projectile
            projectile.transform.rotation = Quaternion.Euler(0f, 0f, angle - 180f);

            Destroy(projectile, 1.5f); // Destroy projectile after 1.5 seconds
        }
    }

}
