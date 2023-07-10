using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PumpCharge : MonoBehaviour
{
    public bool isFiring;
    public bool canPump;

    public int charges = 1;
    public int pelletNum;

    public float damage = 0.25f;
    public float cdTimer;
    public float altTimer;
    public float rotationSpeed = 10f;
    public float distanceFromCenter = 1.88f;
    public float spreadAngle;
    private float cdTime = 0.92f;
    private float altTime = 0.2f;
    private float pelletForce = 50f;

    public GameObject centerPoint;
    public Transform firePoint;
    public GameObject playerGameObject;
    public GameObject pelletPrefab;
    public AudioClip shotgunClip;
    public AudioClip chargeClip;
    private Camera mainCamera;
    private AudioSource audioSource;
    private Vector2 mousePosition;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        centerPoint = GameObject.Find("centerPoint");
        playerGameObject = GameObject.Find("Trinity");
    }

    void Start()
    {
        mainCamera = Camera.main;
        cdTimer = cdTime;
        altTimer = altTime;
        charges = 1;
    }

    void Update()
    {
        mousePosition = Mouse.current.position.ReadValue();

        if (Input.GetMouseButtonDown(1) && canPump)
        {
            charges++;
            if (charges == 1)
            {
                audioSource.pitch = 1f;
                audioSource.PlayOneShot(chargeClip, 0.2f);
            }
            else if (charges == 2)
            {
                audioSource.pitch = 1.25f;
                audioSource.PlayOneShot(chargeClip, 0.2f);
            }
            else if (charges >= 3)
            {
                audioSource.pitch = 1.75f;
                audioSource.PlayOneShot(chargeClip, 0.2f);
            }

            canPump = false;
            altTimer = 0f;
        }
        if(charges >= 4)
        {
            //animator.SetBool(AnimationStrings.overpumped, true);//
        }
    }
    private void FixedUpdate()
    {
        RotateTowardsMouse();
        if (cdTimer < cdTime)
        {
            cdTimer += Time.deltaTime;
        }
        if (altTimer < altTime)
        {
            altTimer += Time.deltaTime;
        }

        if (altTimer >= altTime)
            canPump = true;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            audioSource.pitch = 1f;
            if (cdTimer >= cdTime && charges != 4)
            {
                Shoot();
                cdTimer = 0f;
                charges = 1;
            }
            else if (cdTimer >= cdTime && charges >= 4)
            {
                Explode();
                cdTimer = 0f;
                charges = 1;
            }
        }
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
        isFiring = true;

        // Calculate the direction from the firePoint position to the mouse position
        Vector2 fireDirection = (mousePosition - (Vector2)mainCamera.WorldToScreenPoint(centerPoint.transform.position)).normalized;

        // Calculate the spread angle for the pellets
        
        if (charges == 1)
        {
            spreadAngle = 10f;
            pelletNum = 10;
        }            
        else if (charges == 2)
        {
            spreadAngle = 15f;
            pelletNum = 16;
        }
        else if (charges == 3)
        {
            spreadAngle = 20f;
            pelletNum = 24;
        }

        // Create and shoot pellets with random spread
        for (int i = 0; i < pelletNum; i++)
        {
            // Calculate the random spread angle for the current pellet
            float randomSpreadAngle = Random.Range(-spreadAngle, spreadAngle);

            // Calculate the direction of the current pellet with the spread angle
            Quaternion rotation = Quaternion.Euler(0f, 0f, randomSpreadAngle);
            Vector2 pelletDirection = rotation * fireDirection;

            // Instantiate the pellet prefab
            GameObject pellet = Instantiate(pelletPrefab, firePoint.position, Quaternion.identity);
            pellet.transform.right = pelletDirection;

            // Apply force to the pellet in the calculated direction
            Rigidbody2D pelletRigidbody = pellet.GetComponent<Rigidbody2D>();
            pelletRigidbody.AddForce(pelletDirection * pelletForce, ForceMode2D.Impulse);

            Destroy(pellet, 1f);
        }

        // Play audio clip
        audioSource.PlayOneShot(shotgunClip, 0.3f);
        isFiring = false;
        audioSource.PlayOneShot(chargeClip, 0.2f);

    }

    private void Explode()
    {
        throw new System.NotImplementedException();
    }

}
