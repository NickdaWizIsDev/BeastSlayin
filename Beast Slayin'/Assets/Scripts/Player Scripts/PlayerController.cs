using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb2d;

    private TouchingDirections touching;

    Vector2 moveInput;
    public float runSpeed = 7.5f;

    public float jumpImpulse = 7.5f;
    private readonly float fallGravityScale = 4f;

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

    public AudioSource currentAudioSource;
    public AudioClip swordSwing;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        touching = GetComponent<TouchingDirections>();
    }

    void Update()
    {
        if (rb2d.velocity.y < 0)
        {
            rb2d.gravityScale = fallGravityScale;
        }
        else
        {
            rb2d.gravityScale = 1f;
        }
    }

    private void FixedUpdate()
    {
            rb2d.velocity = new Vector2(moveInput.x * runSpeed, rb2d.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
            moveInput = context.ReadValue<Vector2>();

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
        if (context.started && touching.IsGrounded)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpImpulse);
        }
        else if (context.canceled)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * 0.5f);
        }
    }
}
