using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    [SerializeField] public InventoryItemData data;
    [SerializeField] public int stackSize;

    public InventoryItem(InventoryItemData source, int quantity)
    {
        data = source;
        AddToStack(quantity);

    }

    public void SetStackSize(int quantity)
    {
        if (stackSize + quantity > 0) stackSize = quantity;
        else
        {
            stackSize = 0;
            data = null;
        }
    }

    public void AddToStack(int quantity)
    {
        stackSize = Mathf.Min(stackSize + quantity, data.maxStackSize);

        if (stackSize < 0)
        {
            stackSize = 0;
        }
    }

    public void RemoveFromStack(int quantity)
    {
        if (stackSize - quantity > 0) stackSize -= quantity;
        else
        {
            stackSize = 0;
            data = null;
        }
    }

}