using System.Collections.Generic;
using UnityEngine;

public class SharpshooterBullet : MonoBehaviour
{
    public float damage = 1f;
    public bool hitCoin;

    public AudioClip ricoshot;
    public int maxBounces = 3; // Maximum number of bounces allowed

    private int currentBounces = 0; // Current number of bounces

    private Rigidbody2D rb;
    private AudioSource audioSource;

    private void Awake()
    {
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
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent(out Damageable damageable))
            {
                damageable.Hit(damage);
            }
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
        if (collision.collider.CompareTag("Ground"))
        {
            Vector2 contactPoint = collision.GetContact(0).point;
            Vector2 contactNormal = collision.GetContact(0).normal;

            // Calculate the reflection direction in the 2D plane
            Vector2 reflection = Vector2.Reflect(rb.velocity, contactNormal);

            // Calculate the corrected direction by projecting the reflection onto the ground plane
            Vector2 groundPlaneNormal = new Vector2(contactNormal.y, -contactNormal.x);
            Vector2 correctedDirection = reflection - 2f * Vector2.Dot(reflection, groundPlaneNormal) * groundPlaneNormal;

            rb.velocity = correctedDirection.normalized * rb.velocity.magnitude;
        }
    }

    public void Die()
    {
        Destroy(gameObject, 0.3f); // Destroy the bullet object after 0.7 seconds
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
