using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class NPCHeadLookAt : MonoBehaviour
{
    [SerializeField] private Rig rig;
    [SerializeField] private Transform headLookAtTransform;
    [SerializeField] private float headTurnSpeed = 2f;
    private Transform targetTransform;

    private bool isLookingAtTarget;

    private void Update()
    {
        updateTarget();
        float targetWeight = isLookingAtTarget ? 1.0f : 0.0f;
        rig.weight = Mathf.Lerp(rig.weight, targetWeight, Time.deltaTime * headTurnSpeed);
    }

    public void LookAtPosition(Transform lookAtTarget)
    {
        targetTransform = lookAtTarget;
        isLookingAtTarget = true;
    }

    public void ResetLookAt()
    {
        isLookingAtTarget = false;
    }

    private void updateTarget()
    {
        if (isLookingAtTarget)
        {
            headLookAtTransform.position = Vector3.Lerp(headLookAtTransform.position, targetTransform.position, Time.deltaTime * headTurnSpeed);
        }
    }
}

/* References:
 * 
 *  - https://www.youtube.com/watch?v=LdoImzaY6M4&t=811s&ab_channel=CodeMonkey
 */
