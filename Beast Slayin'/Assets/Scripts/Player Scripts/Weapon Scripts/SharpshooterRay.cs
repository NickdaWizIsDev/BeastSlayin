using UnityEngine;

public class SharpshooterRay : MonoBehaviour
{
    public Transform firePoint;
    public LineRenderer lineRenderer;
    public float maxChargeTime = 1.5f;
    public float raySpeed = 10f;
    public LayerMask enemyLayer;
    public LayerMask groundLayer;

    private float chargeTimer;
    private bool isCharging;

    private void Start()
    {
        chargeTimer = 0f;
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            if (!isCharging)
            {
                isCharging = true;
                chargeTimer = 0f;
                // Add visual and audio feedback for charging
            }
            else
            {
                chargeTimer += Time.deltaTime;
                // Update charging visual feedback, e.g., fill a progress bar
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (isCharging && chargeTimer >= maxChargeTime)
            {
                FirePiercingRay();
            }

            chargeTimer = 0f;
            isCharging = false;
            // Reset visual and audio feedback for charging
        }
    }

    private void FirePiercingRay()
    {
        Vector3 direction = firePoint.forward;
        Vector3 startPoint = firePoint.position;

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, startPoint);

        RaycastHit hit;
        bool shouldContinue = true;

        while (shouldContinue)
        {
            if (Physics.Raycast(startPoint, direction, out hit, Mathf.Infinity, enemyLayer))
            {
                // Apply damage to the enemy hit by the ray
                Damageable damageable = hit.collider.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.Hit(GetRayDamage(chargeTimer));
                    // Add visual and audio effects for enemy hit
                }

                // Exit the loop if the ray hit an enemy
                shouldContinue = false;
            }
            else if (Physics.Raycast(startPoint, direction, out hit, Mathf.Infinity, groundLayer))
            {
                // Bounce the ray off the ground and update the direction
                Vector3 reflection = Vector3.Reflect(direction, hit.normal);
                startPoint = hit.point;
                direction = reflection;

                // Add visual effects for ray reflection, e.g., lineRenderer.SetPosition(...)
            }
            else
            {
                // Exit the loop if the ray didn't hit anything
                shouldContinue = false;
            }
        }

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(1, startPoint + direction * 100f);
        lineRenderer.enabled = true;

        // Add visual and audio effects for ray firing
    }

    private int GetRayDamage(float chargeTime)
    {
        // Calculate and return the damage based on the charge time or other factors
        // Adjust the damage to suit your game's balance and design
        return Mathf.RoundToInt(chargeTime * 10f);
    }
}
