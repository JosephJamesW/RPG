using UnityEngine;
using UnityEngine.EventSystems;

public class ItemFrame : MonoBehaviour, IPointerClickHandler
{
    public InventorySystem inventorySystem;
    public int slotIndex;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Pass the specific slot index to the DropItem method
        if (inventorySystem != null && eventData.button == PointerEventData.InputButton.Left)
        {
            inventorySystem.DropItem(slotIndex);
        }
    }
}