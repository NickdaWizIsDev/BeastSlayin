using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanBullet : MonoBehaviour
{
    public int damage = 1;

    private Animator animator;
    private Rigidbody2D rb;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weakpoint"))
        {
            damage *= 3;
        }

        else if (collision.gameObject.CompareTag("Enemy"))
        { 
            Damageable damageable = collision.GetComponent<Damageable>();
            if(damageable != null)
            {
                damageable.Hit(damage);
            }

            Crash();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Destroy the bullet upon colliding with the ground
            Crash();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Destroy the bullet upon colliding with the ground
            Crash();
        }
    }

    private void Crash()
    {
        animator.SetTrigger(AnimationStrings.bulletHitTrigger);
        rb.velocity = Vector2.zero;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
