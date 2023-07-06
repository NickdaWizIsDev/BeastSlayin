using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sharpshooter : MonoBehaviour
{
    public GameObject centerPoint; // Center point around which the gun rotates
    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject ricoPrefab;
    public GameObject playerGameObject;
    public AudioClip gunshotClip;
    public float bulletForce = 100f;
    public float cooldownTime = 0.49f;
    public float rotationSpeed = 10f;
    public float distanceFromCenter = 1.5f; // Set the desired distance from the center point

    public int altFireCharges = 3;

    [SerializeField]
    public int AltBounces
    { 
        get
        {
           return altFire.maxBounces;
        }
        set
        {
            altFire.maxBounces = value;
        }
    }

    public float altFireForce = 555f;
    public float chargeTimer = 0f;
    public float altCooldownTimer = 6.4f;
    public bool isCharging;
    public bool altFireReady;
    public bool isFiring;

    private Camera mainCamera;
    private AudioSource audioSource;
    private Animator animator;
    private Vector2 mousePosition;
    private float shootTimer;
    private SharpshooterBullet altFire;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        centerPoint = GameObject.Find("centerPoint");
        playerGameObject = GameObject.Find("Trinity");
        altFire = ricoPrefab.GetComponent<SharpshooterBullet>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        shootTimer = cooldownTime;
        altCooldownTimer = 0f;
        chargeTimer = 0f;
    }

    private void Update()
    {
        mousePosition = Mouse.current.position.ReadValue();

        if (Input.GetMouseButtonDown(1))
        {
            if (altFireCharges != 0)
            {
                isFiring = true;
                chargeTimer = 0f;
            }
        }
        else if (Input.GetMouseButton(1))
        {
            if (isFiring && chargeTimer < 1f)
            {
                chargeTimer += Time.deltaTime;
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            if (isFiring && chargeTimer >= 0.3f)
            {
                audioSource.Stop();
                AltFire();
                altFireCharges--;
                altFire.maxBounces = 1;
            }
            else if (isFiring && chargeTimer >= 0.6f)
            {
                audioSource.Stop();
                AltFire();
                altFireCharges--;
                altFire.maxBounces = 2;
            }
            else if (isFiring && chargeTimer >= 1f)
            {
                audioSource.Stop();
                AltFire();
                altFireCharges--;
                altFire.maxBounces = 3;
            }
            audioSource.clip = null;
            animator.SetBool(AnimationStrings.isChargingSharpshooter, false);
            isFiring = false;
            chargeTimer = 0f;
        }

        if (altFireCharges < 3)
        {
            altCooldownTimer += Time.deltaTime;
        }            

        if(altCooldownTimer >= 6.4f && altFireCharges < 3)
        {
            altFireCharges++;
            altCooldownTimer = 0f;
        }

    }

    private void AltFire()
    {
        Vector3 direction = (mousePosition - (Vector2)mainCamera.WorldToScreenPoint(firePoint.position)).normalized;

        GameObject bullet = Instantiate(ricoPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(direction * altFireForce, ForceMode2D.Impulse);
        audioSource.PlayOneShot(gunshotClip, 0.45f);
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
        Vector3 directionFromCenter = worldMousePosition - centerPoint.transform.position;
        directionFromCenter.z = 0f; // Set the z-component to 0 for 2D

        // Calculate the desired position based on the distance from the center point
        Vector3 desiredPosition = centerPoint.transform.position + directionFromCenter.normalized * distanceFromCenter;

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