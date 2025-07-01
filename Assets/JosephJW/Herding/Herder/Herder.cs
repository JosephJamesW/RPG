using System.IO.Pipes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
public class Herder : MonoBehaviour
{
    [Header("References")]
    private NavMeshAgent agent;
    private HerdManager herdManager;

    [Header("Herding Behavior")]
    [Tooltip("How much further than the herd's calculated radius to circle. Ensures herder is on the outside.")]
    public float circlingRadiusOffset = 5.0f;

    [Tooltip("The desired linear speed the herder's target point should maintain along its circular orbit. The NavMeshAgent's own speed should be set to allow it to achieve this.")]
    public float desiredOrbitLinearSpeed = 5.0f;

    [Tooltip("Minimum angular speed in radians per second (e.g., 0.5 rad/s ~= 30 deg/s). Ensures movement on large circles.")]
    public float minAngularSpeed = 0.5f; // NEW: Minimum angular speed

    [Tooltip("Maximum angular speed in radians per second (e.g., PI*2 = 1 full circle/sec). Prevents extreme speeds on very small radii.")]
    public float maxAngularSpeed = Mathf.PI * 2f;

    [Tooltip("Minimum radius used in speed calculations to prevent division by zero or excessively high speeds for the 'desiredOrbitLinearSpeed' calculation.")]
    public float minRadiusForSpeedCalc = 1.0f;

    [Tooltip("Minimum overall radius for circling, used if herd is very small or not present.")]
    public float minCirclingRadius = 10f;

    private bool isClockwise = true;
    private float currentAngle = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("Herder script requires a NavMeshAgent component on " + name);
            enabled = false;
            return;
        }

        agent.stoppingDistance = 1.0f;
        agent.updateRotation = true;
        agent.updatePosition = true;

        herdManager = HerdManager.Instance;
        if (herdManager == null)
        {
            Debug.LogWarning("Herder (" + name + ") could not find HerdManager instance. Herding behavior will be limited.");
        }

        if (gameObject.tag != "Herder")
        {
            Debug.LogWarning("Herder GameObject (" + name + ") is not tagged as 'Herder'. Herd members might not detect it properly.");
        }
    }

    void Update()
    {
        HandleInput();
        UpdateHerderMovement();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            isClockwise = !isClockwise;
        }
    }

    void UpdateHerderMovement()
    {
        Vector3 herdCenterPoint;
        float targetCirclingRadius;

        if (herdManager != null && herdManager.herdMembersList.Count > 0)
        {
            herdCenterPoint = herdManager.currentHerdCenter;
            targetCirclingRadius = herdManager.herdRadius + circlingRadiusOffset;
            targetCirclingRadius = Mathf.Max(targetCirclingRadius, minCirclingRadius);
        }
        else
        {
            herdCenterPoint = transform.position + transform.forward * minCirclingRadius;
            targetCirclingRadius = minCirclingRadius;
        }

        float effectiveRadiusForSpeedCalc = Mathf.Max(targetCirclingRadius, minRadiusForSpeedCalc);
        float calculatedAngularSpeed;

        if (effectiveRadiusForSpeedCalc <= 0.01f)
        {
            // If radius is effectively zero, use max angular speed (or could be min, depending on desired behavior)
            calculatedAngularSpeed = maxAngularSpeed;
        }
        else
        {
            // Calculate angular speed based on desired linear speed
            calculatedAngularSpeed = desiredOrbitLinearSpeed / effectiveRadiusForSpeedCalc;
        }

        // Now, clamp the calculatedAngularSpeed between the new minAngularSpeed and existing maxAngularSpeed
        float currentAngularSpeed = Mathf.Clamp(calculatedAngularSpeed, minAngularSpeed, maxAngularSpeed);

        float angleStep = currentAngularSpeed * Time.deltaTime;
        currentAngle += (isClockwise ? angleStep : -angleStep);
        currentAngle %= (2 * Mathf.PI);

        Vector3 offsetFromHerdCenter = new Vector3(Mathf.Cos(currentAngle), 0, Mathf.Sin(currentAngle)) * targetCirclingRadius;
        Vector3 destinationTarget = herdCenterPoint + offsetFromHerdCenter;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(destinationTarget, out hit, targetCirclingRadius * 0.5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.SetDestination(destinationTarget);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && agent != null)
        {
            Vector3 actualHerdCenter = (herdManager != null && herdManager.herdMembersList.Count > 0) ? herdManager.currentHerdCenter : transform.position;
            float actualCirclingRadius = (herdManager != null && herdManager.herdMembersList.Count > 0) ? (herdManager.herdRadius + circlingRadiusOffset) : minCirclingRadius;
            actualCirclingRadius = Mathf.Max(actualCirclingRadius, minCirclingRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(actualHerdCenter, actualCirclingRadius);

            Vector3 offsetFromHerdCenterViz = new Vector3(Mathf.Cos(currentAngle), 0, Mathf.Sin(currentAngle)) * actualCirclingRadius;
            Vector3 currentDestinationTargetViz = actualHerdCenter + offsetFromHerdCenterViz;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(currentDestinationTargetViz, 0.5f);
            if (agent.hasPath)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, agent.destination);
            }
        }
    }
}