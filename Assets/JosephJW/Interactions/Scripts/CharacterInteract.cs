using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInteract : MonoBehaviour
{
    [SerializeField] public Transform head;

    public void Interact()
    {
        IInteractable interactable = GetInteractableObject();
        if (interactable != null)
        {
            interactable.Interact(transform);
        }
    }

    public IInteractable GetInteractableObject()
    {
        List<IInteractable> interactableList = new List<IInteractable>();
        float interactRange = 2f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.IsInteractable())
                {
                    interactableList.Add(interactable);
                }
            }
        }

        IInteractable closestInteractable = null;
        foreach (IInteractable interactable in interactableList)
        {
            if (closestInteractable == null)
            {
                closestInteractable = interactable;
            }
            else
            {
                if (Vector3.Distance(transform.position, interactable.GetTransform().position) <
                    Vector3.Distance(transform.position, closestInteractable.GetTransform().position))
                {
                    closestInteractable = interactable;
                }
            }
        }
        return closestInteractable;
    }
}
