using System.Net;
using UnityEngine;

public class RayDamage : MonoBehaviour
{
    public Transform firePoint;
    public LineRenderer lineRenderer;
    public AudioClip rechargeClip;
    public AudioClip readyClip;
    public AudioClip rayClip;
    public AudioClip chargeUpClip;

    public int rayDamage = 1;
    public float chargeUpDuration = 1f;
    public float rayDuration = 0.75f;
    public float cooldownTime = 3f;
    public float raySpeed = 100f;

    private Camera mainCamera;
    private AudioSource audioSource;
    private Animator animator;
    private bool isFiring;
    private float chargeTimer;
    private float cooldownTimer;
    private bool rayActive;
    private bool ready = true;
    private bool hitWeakpoint;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        lineRenderer.enabled = false;
        chargeTimer = 0f;
        cooldownTimer = 0f;
        rayActive = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (ready)
            {
                isFiring = true;
                chargeTimer = 0f;
                Charge();
            }
        }
        else if (Input.GetMouseButton(1))
        {
            if (isFiring && chargeTimer < chargeUpDuration)
            {
                chargeTimer += Time.deltaTime;
                float chargePercent = chargeTimer / chargeUpDuration;
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            if (isFiring && chargeTimer >= chargeUpDuration)
            {
                audioSource.Stop();
                FireRay();
                cooldownTimer = cooldownTime;
                Invoke(nameof(DisableFiring), rayDuration);
            }
            audioSource.clip = null;
            animator.SetBool(AnimationStrings.isChargingPiercer, false);
            isFiring = false;
            chargeTimer = 0f;
        }

        if (rayActive)
        {
            UpdateRayDamage();
            WeakpointDamage();

            rayDamage = 1;

            if (hitWeakpoint)
            {
                rayDamage *= 2;
            }
        }

        if (cooldownTimer > 0f)
        {
            ready = false;
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                cooldownTimer = 0f;
                audioSource.Stop();
                audioSource.PlayOneShot(readyClip);
                ready = true;
                // TODO: Update cooldown UI or perform any cooldown visualizations
            }
        }
    }

    private void Charge()
    {
        audioSource.loop = true;
        audioSource.clip = chargeUpClip;
        audioSource.Play();
        animator.SetBool(AnimationStrings.isChargingPiercer, true);
    }

    private void DisableFiring()
    {
        lineRenderer.enabled = false;
        rayActive = false;
        audioSource.PlayOneShot(rechargeClip);
    }

    private void FireRay()
    {
        Vector3 direction = (Input.mousePosition - mainCamera.WorldToScreenPoint(firePoint.position)).normalized;
        Vector3 endPoint = firePoint.position + direction * 100f;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, endPoint);

        rayActive = true;
        lineRenderer.enabled = true;
        audioSource.PlayOneShot(rayClip);
    }

    private void UpdateRayDamage()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(firePoint.position, lineRenderer.GetPosition(1) - firePoint.position, 100f);
        bool hitEnemy = false;
        int hitCount = 0;

        foreach (RaycastHit2D hit in hits)
        {
            Collider2D collider = hit.collider;
            Damageable damageable = collider.GetComponent<Damageable>();

            if (damageable != null)
            {
                if (!hitEnemy)
                {
                    damageable.Hit(rayDamage);
                    lineRenderer.SetPosition(1, hit.point);
                    hitEnemy = true;
                    hitCount++;
                }
                else
                {
                    break; // Stop processing hits after the first enemy hit
                }
            }
            else if (collider.gameObject.CompareTag("Ground"))
            {
                lineRenderer.SetPosition(1, hit.point);
                break;
            }

            if (hitCount >= 3)
            {
                break; // Stop processing hits after reaching the maximum hit count
            }
        }

        if (!hitEnemy)
        {
            // If the ray didn't hit an enemy, extend the ray to its maximum length
            lineRenderer.SetPosition(1, lineRenderer.GetPosition(0) + lineRenderer.GetPosition(1) - firePoint.position);
        }
    }

    private void WeakpointDamage()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(firePoint.position, lineRenderer.GetPosition(1) - firePoint.position, 100f);
        for (int i = 0; i < Mathf.Min(3, hits.Length); i++)
        {
            RaycastHit2D hit = hits[i];
            Collider2D collider = hit.collider;

            if (hit.collider.CompareTag("Weakpoint"))
            {
                hitWeakpoint = true;
            }
        }
    }
}