using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Sentry : MonoBehaviour
{
    public float detectionRange = 10f;
    public float damageAmount = 10f;
    public float lineWidth = 0.1f;
    public float lineMaxLength = 20f;
    public Color aimColor = Color.yellow;
    public Color chargeColor = Color.red;
    public GameObject projectile;
    public Transform firePoint;

    private Transform playerTransform;
    private LineRenderer lineRenderer;
    private Animator animator;
    private bool isAiming = false;

    public bool HasTarget { get { return isAiming; } private set { isAiming = value; animator.SetBool(AnimationStrings.hasTarget, value); } }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 0;
    }

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= detectionRange && !isAiming)
        {
            StartAiming();
        }

        if (isAiming)
        {
            Aim();
        }
    }

    private void StartAiming()
    {
        isAiming = true;
    }

    private void Aim()
    {
        HasTarget = isAiming;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = true;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = aimColor;
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, playerTransform.position);
    }

    public IEnumerator Fire()
    {
        Color tempAim = aimColor;
        aimColor = chargeColor; 
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, playerTransform.position.normalized);
        Vector2 direction = (playerTransform.position - firePoint.position).normalized;

        int projectileSpeed = 100;

        yield return new WaitForSeconds(.5f);

        GameObject SentryProjectile = Instantiate(projectile, firePoint.position, Quaternion.identity);
        SentryProjectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;

        Destroy(SentryProjectile, 1f); // Destroy projectile after 1 second

        aimColor = tempAim;

        lineRenderer.enabled = false;
        ResetSentry();
    }

    private void ResetSentry()
    {
        isAiming = false;
        lineRenderer.positionCount = 0;
    }
}
