using Unity.VisualScripting;
using UnityEngine;

public class ContainerInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string containerName;
    private InventorySystem inventorySystem;

    private void Awake()
    {
        inventorySystem = GetComponent<InventorySystem>();
    }

    public void Interact(Transform interactorTransform)
    {
        InventorySystem interactorInventorySystem = interactorTransform.GetComponent<InventorySystem>();
        if (interactorInventorySystem)
        {
            interactorInventorySystem.CurrentInteractingInventory = inventorySystem;
        }
    }

    public string GetInteractText()
    {
        string nameExtension = containerName != "" ? " " + containerName : "";
        return "Open" + nameExtension;
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
