using UnityEngine;

public class InventoryItemInteractable : MonoBehaviour, IInteractable
{
    public InventoryItemData referenceItem;

    public void Interact(Transform interactorTransform)
    {
        interactorTransform.GetComponent<InventorySystem>().Add(referenceItem, 1);
        Destroy(gameObject);
    }

    public string GetInteractText()
    {
        return "Pick up " + referenceItem.displayName;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool IsInteractable()
    {
        return true;
    }
}
