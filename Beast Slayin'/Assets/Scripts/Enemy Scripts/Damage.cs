using UnityEngine;

public class Damage : MonoBehaviour
{
    public float damage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Damageable damageable))
        {
            damageable.Hit(damage);
        }
    }
}