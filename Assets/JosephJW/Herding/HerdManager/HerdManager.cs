using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HerdManager : MonoBehaviour
{
    public static HerdManager Instance { get; private set; }

    public List<HerdMember> herdMembersList = new List<HerdMember>();
    public Vector3 currentHerdCenter { get; private set; }
    public float herdRadius { get; private set; } // Approximate radius of the herd

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (herdMembersList.Count > 0)
        {
            CalculateHerdCenterAndRadius();
        }
        else
        {
            currentHerdCenter = transform.position; // Default to own position if no members
            herdRadius = 0f;
        }
    }

    public void RegisterMember(HerdMember member)
    {
        if (!herdMembersList.Contains(member))
        {
            herdMembersList.Add(member);
        }
    }

    public void UnregisterMember(HerdMember member)
    {
        if (herdMembersList.Contains(member))
        {
            herdMembersList.Remove(member);
        }
    }

    void CalculateHerdCenterAndRadius()
    {
        if (herdMembersList.Count == 0) return;

        Vector3 centerSum = Vector3.zero;
        foreach (HerdMember member in herdMembersList)
        {
            if (member != null) // Ensure member hasn't been destroyed
            {
                centerSum += member.transform.position;
            }
        }
        currentHerdCenter = centerSum / herdMembersList.Count;

        // Calculate approximate herd radius (max distance from center)
        float maxSqrDist = 0f;
        foreach (HerdMember member in herdMembersList)
        {
            if (member != null)
            {
                float sqrDist = (member.transform.position - currentHerdCenter).sqrMagnitude;
                if (sqrDist > maxSqrDist)
                {
                    maxSqrDist = sqrDist;
                }
            }
        }
        herdRadius = Mathf.Sqrt(maxSqrDist);
    }

    // Optional: Gizmo to visualize the herd center and radius
    void OnDrawGizmosSelected()
    {
        if (herdMembersList.Count > 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(currentHerdCenter, 0.5f);
            Gizmos.DrawWireSphere(currentHerdCenter, herdRadius);
        }
    }
}