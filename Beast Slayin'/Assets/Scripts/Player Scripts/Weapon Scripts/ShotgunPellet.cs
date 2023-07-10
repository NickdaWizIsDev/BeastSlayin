using System.Collections.Generic;
using UnityEngine;

public class ShotgunPellet : MonoBehaviour
{
    public float damage = 0.25f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent(out Damageable damageable))
            {
                damageable.Hit(damage);
            }
            Invoke(nameof(Die), 0.1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Die();
        }
    }

    private void OnTriggerStay2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
