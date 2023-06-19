using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb2d;
    private TouchingDirections touching;
    private Animator animator;
    private Damageable damageable;
    private Dash dash;

    public AudioSource currentAudioSource;

    Vector2 moveInput;
    public float runSpeed = 7.5f;

    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove && !touching.IsOnWall)
            {
                if (IsMoving)
                {
                    return runSpeed;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        set
        {

        }
    }
    private bool isMoving;
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
    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public float jumpImpulse = 7.5f;
    private int jumpCount;
    private readonly float fallGravityScale = 7f;

    private bool isFacingRight = true;
    public bool IsFacingRight
    {
        get
        {
            return isFacingRight;
        }
        private set
        {
            if (isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }

            isFacingRight = value;
        }
    }

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        touching = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
        dash = GetComponent<Dash>();

        if (currentAudioSource == null)
            currentAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        animator.SetFloat(AnimationStrings.yVelocity, rb2d.velocity.y);

        if (rb2d.velocity.y < 0)
        {
            rb2d.gravityScale = fallGravityScale;
        }
        else
        {
            rb2d.gravityScale = 4f;
        }
    }

    private void FixedUpdate()
    {
        if (!damageable.IsHit)
            rb2d.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb2d.velocity.y);
        if (dash.IsDashing)
        {
            rb2d.velocity = new Vector2(dash.dashSpeed * transform.localScale.x, rb2d.velocity.y);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        IsMoving = moveInput != Vector2.zero;

        SetFacingDirection(moveInput);
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (touching.IsGrounded)
        {
            jumpCount = 0;
            Debug.Log("Jumps reset!");
        }

        if (context.started && touching.IsGrounded)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpImpulse);
            animator.SetTrigger(AnimationStrings.jump);
        }
        else if (context.started && jumpCount == 0 && touching.IsOnWall)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpImpulse);
            jumpCount++;
        }
        else if (jumpCount < 3 && context.started && touching.IsOnWall)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpImpulse);
            jumpCount++;
        }
        else if (context.canceled)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * 0.5f);
            rb2d.gravityScale = fallGravityScale;
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb2d.velocity = new Vector2(knockback.x, rb2d.velocity.y + knockback.y);
    }
}
