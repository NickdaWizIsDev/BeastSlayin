using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Railgun : MonoBehaviour
{
    public bool isRailReady;
    public bool isCharging;
    public bool charged;
    public bool isFiring;

    public float damage = 8f;
    public float rotationSpeed = 10f;
    public float distanceFromCenter = 2.22f;
    private float cdTime = 16f;

    public GameObject centerPoint;
    public Transform firePoint;
    public GameObject playerGameObject;
    public AudioClip fireClip;
    public AudioClip chargedClip;
    public FloatVariable cd;
    private Camera mainCamera;
    private AudioSource audioSource;
    private Vector2 mousePosition;

    private Animator animator;

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
    }

    void Update()
    {
        mousePosition = Mouse.current.position.ReadValue();
        if (cd.value >= cdTime)
        {
            charged = true;
            animator.SetBool(AnimationStrings.charged, true);
            isCharging = false;
        }

        else if (cd.value < cdTime)
        {
            isCharging = true;
            animator.SetBool(AnimationStrings.charged, false);
        }    
    }

    public void PlayChargedClip()
    {
        audioSource.clip = chargedClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    private void FixedUpdate()
    {
        RotateTowardsMouse();

        
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (charged)
            {
                StartCoroutine(Shoot());
                charged = false;
                cd.value = 0f;

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

    private IEnumerator Shoot()
    {   
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
        isFiring = true;

        audioSource.Stop();
        audioSource.PlayOneShot(fireClip, 0.3f);

        Vector3 direction = (Input.mousePosition - mainCamera.WorldToScreenPoint(centerPoint.transform.position)).normalized;
        direction.Normalize();
        Vector2 endPoint = firePoint.position + direction * 1000f;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, endPoint);

        RaycastHit2D[] hits = Physics2D.RaycastAll(firePoint.position, direction, Mathf.Infinity);

        foreach (RaycastHit2D hit in hits)
        {
            Collider2D collider = hit.collider;

            if (collider.gameObject.CompareTag("Enemy"))
            {
                if (collider.TryGetComponent(out Damageable damageable))
                    damageable.Hit(damage);
            }
            else if (collider.gameObject.CompareTag("Weakpoint"))
            {
                Damageable damageable = collider.GetComponentInParent<Damageable>();
                damageable.Hit(damage * 2);
            }
            if (collider.gameObject.CompareTag("Ground"))
            {
                lineRenderer.SetPosition(1, hit.point);
                break;
            }
        }

        yield return new WaitForSeconds(0.8f);
        lineRenderer.enabled = false;
        isFiring = false;
    }
}
