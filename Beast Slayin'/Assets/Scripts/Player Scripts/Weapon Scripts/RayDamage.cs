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
    private bool isFiring;
    private float chargeTimer;
    private float cooldownTimer;
    private bool rayActive;
    private bool ready = true;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
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
                // TODO: Update charge UI or perform any charge visualizations
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
            isFiring = false;
            chargeTimer = 0f;
        }

        if (rayActive)
        {
            UpdateRayDamage();
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

        for (int i = 0; i < Mathf.Min(3, hits.Length); i++)
        {
            RaycastHit2D hit = hits[i];
            Collider2D collider = hit.collider;
            Damageable damageable = collider.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.Hit(rayDamage);
                lineRenderer.SetPosition(1, hit.point);
                break;
            }
            else if (collider.gameObject.CompareTag("Ground"))
            {
                lineRenderer.SetPosition(1, hit.point);
                break;
            }
        }
    }
}
