using UnityEngine;

public class PlayerHolding: MonoBehaviour 
{
    public InventoryItem holdingItem;

    public InventoryItemData GetData() { return holdingItem.data; }
    public void SetData(InventoryItemData data) { holdingItem.data = data; }

    public int GetStackSize() { return holdingItem.stackSize; }
    public void SetStackSize(int size) {  holdingItem.stackSize = size; }
}
