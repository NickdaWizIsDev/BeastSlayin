using UnityEngine;
using UnityEngine.InputSystem;

public class Piercer : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 20f;
    public float cooldownTime = 0.5f;
    public float rotationSpeed = 10f;

    private Camera mainCamera;
    private Vector3 mousePosition;
    private float shootTimer;

    private void Start()
    {
        mainCamera = Camera.main;
        shootTimer = 0.25f;
    }

    private void Update()
    {
        RotateTowardsMouse();
        shootTimer += Time.deltaTime;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (CanShoot())
            {
                Shoot();
                ResetCooldown();
            }
        }
    }

    private bool CanShoot()
    {
        return shootTimer >= cooldownTime;
    }

    private void ResetCooldown()
    {
        shootTimer = 0f;
    }

    private void RotateTowardsMouse()
    {
        mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
        Vector3 direction = (worldMousePosition - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void Shoot()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
        Vector3 direction = (worldMousePosition - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(direction * bulletForce, ForceMode2D.Impulse);

        // Destroy the bullet object after a certain time
        Destroy(bullet, 1f); // Change the time as per your requirements
    }
}
