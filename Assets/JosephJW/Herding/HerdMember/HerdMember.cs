using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class HerdMember : MonoBehaviour
{
    [Header("References")]
    private NavMeshAgent agent;
    private Transform herderTransform;

    [Header("Movement Settings")]
    public float maxSpeed = 3.5f;
    public float maxForce = 2f; // Max steering force [cite: 93]

    [Header("Flocking Behavior Weights")]
    public float separationWeight = 1.5f;
    public float alignmentWeight = 1.0f; 
    public float cohesionWeight = 1.0f;

    [Header("Flocking Perception Radii")]
    public float separationRadius = 2.0f; // Boids 'protectedRange' [cite: 92, 158]
    public float perceptionRadius = 5.0f; // Boids 'visualRange' for alignment and cohesion [cite: 91, 158]

    [Header("Herder Interaction")]
    public float fleeDistance = 10.0f; // Distance to start fleeing from herder
    public float fleeStrength = 3.0f;

    private Vector3 currentVelocity;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = maxSpeed; // NavMeshAgent speed can also control movement
        agent.stoppingDistance = 0.1f; // Allow agent to get close to its micro-targets
        agent.updateRotation = false; // We'll handle rotation manually based on velocity
        agent.updatePosition = true; // Let agent handle position updates based on Move()

        // Find the herder (assuming herder has "Herder" tag)
        GameObject herderObj = GameObject.FindGameObjectWithTag("Herder");
        if (herderObj != null)
        {
            herderTransform = herderObj.transform;
        }
        else
        {
            Debug.LogWarning("HerdMember could not find GameObject with tag 'Herder'.");
        }

        // Register with HerdManager
        if (HerdManager.Instance != null)
        {
            HerdManager.Instance.RegisterMember(this);
        }

        // Initialize velocity (optional, could be random or forward)
        currentVelocity = transform.forward * Random.Range(0.5f * maxSpeed, maxSpeed);
    }

    void OnDestroy()
    {
        // Unregister from HerdManager
        if (HerdManager.Instance != null)
        {
            HerdManager.Instance.UnregisterMember(this);
        }
    }

    void Update()
    {
        Vector3 steeringForce = Vector3.zero;

        if (HerdManager.Instance != null && HerdManager.Instance.herdMembersList.Count > 1)
        {
            steeringForce += CalculateSeparation() * separationWeight;
            steeringForce += CalculateAlignment() * alignmentWeight;
            steeringForce += CalculateCohesion() * cohesionWeight;
        }

        if (herderTransform != null)
        {
            steeringForce += CalculateFlee() * fleeStrength;
        }

        // Apply steering force
        Vector3 acceleration = Vector3.ClampMagnitude(steeringForce, maxForce); // Assuming mass or just treat as 1
        currentVelocity += acceleration * Time.deltaTime;
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);

        // Use NavMeshAgent.Move for NavMesh constraints and local movement [cite: 276]
        agent.Move(currentVelocity * Time.deltaTime);

        // Update rotation to face movement direction
        if (currentVelocity.sqrMagnitude > 0.01f) // Avoid zero vector for LookRotation [cite: 104]
        {
            transform.rotation = Quaternion.LookRotation(currentVelocity.normalized);
        }
    }

    Vector3 CalculateSeparation()
    {
        Vector3 separationForce = Vector3.zero;
        int neighborsInSeparationRange = 0;

        foreach (HerdMember otherMember in HerdManager.Instance.herdMembersList)
        {
            if (otherMember == this || otherMember == null) continue;

            float distSqr = (transform.position - otherMember.transform.position).sqrMagnitude;
            if (distSqr > 0 && distSqr < separationRadius * separationRadius)
            {
                Vector3 fleeDirection = (transform.position - otherMember.transform.position).normalized;
                separationForce += fleeDirection / Mathf.Sqrt(distSqr); // Weight by inverse distance [cite: 107]
                neighborsInSeparationRange++;
            }
        }

        if (neighborsInSeparationRange > 0)
        {
            separationForce /= neighborsInSeparationRange;
        }
        return SteerTowards(separationForce);
    }

    Vector3 CalculateAlignment()
    {
        Vector3 averageHeading = Vector3.zero;
        int neighborsInPerceptionRange = 0;

        foreach (HerdMember otherMember in HerdManager.Instance.herdMembersList)
        {
            if (otherMember == this || otherMember == null) continue;

            float distSqr = (transform.position - otherMember.transform.position).sqrMagnitude;
            if (distSqr < perceptionRadius * perceptionRadius)
            {
                averageHeading += otherMember.currentVelocity; // Use currentVelocity for alignment
                neighborsInPerceptionRange++;
            }
        }

        if (neighborsInPerceptionRange > 0)
        {
            averageHeading /= neighborsInPerceptionRange;
            averageHeading.Normalize(); // We only care about direction for average heading for this implementation
        }
        return SteerTowards(averageHeading * maxSpeed); // Steer towards the average heading
    }

    Vector3 CalculateCohesion()
    {
        Vector3 centerOfMass = Vector3.zero;
        int neighborsInPerceptionRange = 0;

        foreach (HerdMember otherMember in HerdManager.Instance.herdMembersList)
        {
            if (otherMember == this || otherMember == null) continue;

            float distSqr = (transform.position - otherMember.transform.position).sqrMagnitude;
            if (distSqr < perceptionRadius * perceptionRadius)
            {
                centerOfMass += otherMember.transform.position;
                neighborsInPerceptionRange++;
            }
        }

        if (neighborsInPerceptionRange > 0)
        {
            centerOfMass /= neighborsInPerceptionRange;
            Vector3 desiredDirection = centerOfMass - transform.position;
            return SteerTowards(desiredDirection);
        }
        return Vector3.zero;
    }

    Vector3 CalculateFlee()
    {
        Vector3 fleeForce = Vector3.zero;
        if (herderTransform == null) return fleeForce;

        float distToHerderSqr = (transform.position - herderTransform.position).sqrMagnitude;
        if (distToHerderSqr < fleeDistance * fleeDistance)
        {
            Vector3 directionFromHerder = (transform.position - herderTransform.position).normalized;
            fleeForce = directionFromHerder * maxSpeed; // Flee at max speed
        }
        return SteerTowards(fleeForce); // Use SteerTowards to incorporate maxForce
    }

    Vector3 SteerTowards(Vector3 desired)
    {
        Vector3 steer = desired.normalized * maxSpeed - currentVelocity;
        return Vector3.ClampMagnitude(steer, maxForce); 
    }


    // Gizmos for visualizing perception radii
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, perceptionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
        if (herderTransform != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, fleeDistance);
        }
    }
}