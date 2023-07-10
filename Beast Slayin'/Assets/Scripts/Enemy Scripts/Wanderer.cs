using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanderer : MonoBehaviour
{
    public float moveSpeed = 3f;  // Speed at which the enemy moves
    public float detectionRange = 10f;  // Range within which the enemy can detect the player
    public float attackRange = 2f; // Range within which the animation is triggered
    public float lungeForce = 10f;

    public bool isMoving;
    public bool isAttacking;

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

    private Rigidbody2D rb;
    private Animator animator;
    private TouchingDirections touching;
    private Damageable damageable;
    private Transform playerTransform;  // Reference to the player's transform
    private bool isFollowingPlayer;  // Indicates whether the enemy is following the player
    private bool canMove = true;

    public bool IsMoving
    {
        get
        {
            return isMoving;
        }
        private set
        {
            isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touching = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();

        // Find the player's transform by tag or other means
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {   
        if (damageable.IsAlive)
        {
            if (isAttacking)
            {
                rb.velocity = Vector2.zero;
            }

            // Check if the player is within the animation range
            if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange && !isAttacking)
            {
                isAttacking = true;
                canMove = false;
                animator.SetTrigger(AnimationStrings.atk);
            }
            // Check if the player is within the detection range
            else if (playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) <= detectionRange && canMove)
            {
                // Calculate the direction to the player along the X-axis
                float directionX = Mathf.Sign(playerTransform.position.x - transform.position.x);

                // Set the walking direction based on the X-axis direction
                WalkDirection = directionX > 0f ? WalkableDirection.Right : WalkableDirection.Left;

                // Set the enemy's velocity to follow the player along the X-axis
                rb.velocity = new Vector2(moveSpeed * directionX, rb.velocity.y);

                isFollowingPlayer = true;
            }
            else if (canMove)
            {
                rb.velocity = new Vector2(moveSpeed * walkDirectionVector.x, rb.velocity.y);

                isFollowingPlayer = false;

                // If the player is not within range, continue with the regular wandering behavior
                if (touching.IsOnWall && touching.IsGrounded)
                {
                    FlipDirection();
                }
            }
        }

        if (rb.velocity != Vector2.zero || isFollowingPlayer)
        {
            IsMoving = true;
        }
        else
        {
            IsMoving = false;
        }
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        }
        else if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        }
        else
        {
            Debug.LogError("Current walkable direction is invalid.");
        }
    }

    public IEnumerator Lunge()
    {
        float direction = Mathf.Sign(playerTransform.position.x - transform.position.x);
        WalkDirection = direction > 0f ? WalkableDirection.Right : WalkableDirection.Left;

        // Set the enemy's velocity to follow the player along the X-axis
        rb.velocity = new Vector2(direction * lungeForce, rb.velocity.y);

        yield return new WaitForSeconds(1f);
        canMove = true;
        isAttacking = false;
    }

}
