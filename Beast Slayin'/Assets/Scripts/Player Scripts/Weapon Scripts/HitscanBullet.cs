using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanBullet : MonoBehaviour
{
    public int damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        if(damageable != null)
        {
            damageable.Hit(damage);
        }

        Destroy(gameObject);
    }
}
