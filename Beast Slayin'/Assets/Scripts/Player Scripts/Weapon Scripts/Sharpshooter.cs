using UnityEngine;
using UnityEngine.InputSystem;

public class Sharpshooter : MonoBehaviour
{
    public Transform centerPoint; // Center point around which the gun rotates
    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject playerGameObject;
    public AudioClip gunshotClip;
    public float bulletForce = 100f;
    public float cooldownTime = 0.49f;
    public float rotationSpeed = 10f;
    public float distanceFromCenter = 1.5f; // Set the desired distance from the center point

    private Camera mainCamera;
    private AudioSource audioSource;
    private Vector2 mousePosition;
    private float shootTimer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        shootTimer = cooldownTime;
    }

    private void Update()
    {
        mousePosition = Mouse.current.position.ReadValue();
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

    private void FixedUpdate()
    {
        RotateTowardsMouse();
        shootTimer += Time.fixedDeltaTime;
    }

    private void RotateTowardsMouse()
    {
        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // Calculate the direction from the center point to the mouse position
        Vector3 directionFromCenter = worldMousePosition - centerPoint.position;
        directionFromCenter.z = 0f; // Set the z-component to 0 for 2D

        // Calculate the desired position based on the distance from the center point
        Vector3 desiredPosition = centerPoint.position + directionFromCenter.normalized * distanceFromCenter;

        // Set the position of the gun relative to the desired position
        transform.position = desiredPosition;

        // Calculate the angle to rotate towards the mouse with a 90-degree offset
        float angle = Mathf.Atan2(directionFromCenter.y, directionFromCenter.x) * Mathf.Rad2Deg;

        // Set the rotation of the gun to point towards the mouse
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, angle), rotationSpeed * Time.fixedDeltaTime);

        // Update the player's facing direction
        bool isPlayerFacingRight = GetPlayerFacingDirection(directionFromCenter.x);

        // Flip the scale based on the player's facing direction and the aiming direction
        float scaleX = isPlayerFacingRight ? 1f : -1f;
        float scaleY = Mathf.Sign(directionFromCenter.x);
        float scaleZ = 1f;

        transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }


    private bool GetPlayerFacingDirection(float directionX)
    {
        // Assuming the player has a script called "PlayerController" with a "IsFacingRight" property
        PlayerController playerController = playerGameObject.GetComponent<PlayerController>();

        return playerController.IsFacingRight;
    }


    private void Shoot()
    {
        Vector3 direction = (mousePosition - (Vector2)mainCamera.WorldToScreenPoint(firePoint.position)).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(direction * bulletForce, ForceMode2D.Impulse);
        audioSource.PlayOneShot(gunshotClip, 0.45f);

        // Destroy the bullet object after a certain time
        Destroy(bullet, 1f); // Change the time as per your requirements
    }
}