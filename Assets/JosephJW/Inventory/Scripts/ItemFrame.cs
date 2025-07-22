using UnityEngine;
using UnityEngine.EventSystems;

public class ItemFrame : MonoBehaviour, IPointerClickHandler
{
    public InventorySystem inventorySystem;
    public int slotIndex;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Pass the specific slot index to the HoldItem method on left-click.
        if (inventorySystem != null && eventData.button == PointerEventData.InputButton.Left)
        {
            // The 'false' argument tells HoldItem to move the entire stack.
            inventorySystem.SelectSlot(slotIndex);
        }
    }
}