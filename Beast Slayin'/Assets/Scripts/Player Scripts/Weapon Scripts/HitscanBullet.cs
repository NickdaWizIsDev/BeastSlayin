using System.Collections.Generic;
using UnityEngine;

public class HitscanBullet : MonoBehaviour
{
    public float damage = 1f;
    public bool hitCoin;

    public AudioClip ricoshot;
    private Animator animator;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weakpoint"))
        {
            damage *= 2;
            Damageable damageable = collision.GetComponentInParent<Damageable>();
            damageable.Hit(damage);
            Crash();
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent(out Damageable damageable))
            {
                damageable.Hit(damage);
            }

            Crash();
        }
        else if (collision.gameObject.CompareTag("Coin") && !hitCoin)
        {
            Destroy(collision.gameObject);
            FindAndShootToWeakpoint(velocityMagnitude: 1000f);
            Debug.Log("Damage increased to " + damage);
            audioSource.PlayOneShot(ricoshot, 0.2f);
            hitCoin = true;
            damage += 1;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Crash();
        }
    }

    private void OnTriggerStay2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
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

    private void FindAndShootToWeakpoint(float velocityMagnitude)
    {
        GameObject[] weakpoints = GameObject.FindGameObjectsWithTag("Weakpoint");
        if (weakpoints.Length > 0)
        {
            GameObject nearestWeakpoint = null;
            float nearestDistance = Mathf.Infinity;

            foreach (GameObject weakpoint in weakpoints)
            {
                float distance = Vector2.Distance(transform.position, weakpoint.transform.position);
                if (distance < nearestDistance)
                {
                    nearestWeakpoint = weakpoint;
                    nearestDistance = distance;
                }
            }

            if (nearestWeakpoint != null)
            {
                Vector2 direction = (nearestWeakpoint.transform.position - transform.position).normalized;
                rb.velocity = direction * velocityMagnitude;
            }
        }
    }
}
