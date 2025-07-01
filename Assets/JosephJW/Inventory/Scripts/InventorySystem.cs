// InventorySystem.cs
// Start of changed code block
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    private Dictionary<InventoryItemData, InventoryItem> m_itemDictionary;
    [SerializeField] public List<InventoryItem> inventory = new List<InventoryItem>(); // Max length is defined by initial editor size
    private bool inventoryUpdated = false;

    public TradeManager CurrentTrader { get; set; }
    public InventorySystem CurrentInteractingInventory { get; set; }

    private void Awake()
    {
        m_itemDictionary = new Dictionary<InventoryItemData, InventoryItem>();
        if (inventory != null)
        {
            // Initialize based on editor-defined items, list size is now fixed.
            // Assumes 'inventory' is pre-populated with InventoryItem objects;
            // empty slots are InventoryItem objects with 'data == null'.
            foreach (InventoryItem item in inventory)
            {
                // The 'item != null' check is for robustness, in case the list could somehow get actual null entries.
                // If 'item' is guaranteed to be a valid InventoryItem object, 'item.data != null' is the key check.
                if (item != null && item.data != null)
                {
                    item.AddToStack(0); // Ensures stackSize is handled, original behavior.

                    if (!m_itemDictionary.ContainsKey(item.data))
                    {
                        m_itemDictionary.Add(item.data, item);
                    }
                }
            }
        }
    }

    public void Add(InventoryItemData referenceData, int quantity)
    {
        if (referenceData == null || quantity <= 0)
        {
            return;
        }

        inventoryUpdated = true;
        int quantityRemaining = quantity;

        // First pass: Add to existing stacks of the same item type
        for (int i = 0; i < inventory.Count; i++)
        {
            if (quantityRemaining == 0) break;

            InventoryItem existingItem = inventory[i];
            // Ensure existingItem and its data are not null before using them
            if (existingItem != null && existingItem.data == referenceData && existingItem.stackSize < referenceData.maxStackSize)
            {
                int canAddToStack = referenceData.maxStackSize - existingItem.stackSize;
                int amountToFill = Mathf.Min(quantityRemaining, canAddToStack);

                existingItem.AddToStack(amountToFill);
                quantityRemaining -= amountToFill;

                if (!m_itemDictionary.ContainsKey(referenceData))
                {
                    m_itemDictionary.Add(referenceData, existingItem);
                }
            }
        }

        if (quantityRemaining == 0) return;

        // Second pass: Place in empty slots (InventoryItem object exists, but its 'data' is null)
        for (int i = 0; i < inventory.Count; i++)
        {
            if (quantityRemaining == 0) break;

            // An empty slot is an InventoryItem object with null data.
            if (inventory[i] != null && inventory[i].data == null)
            {
                int amountForNewStack = Mathf.Min(quantityRemaining, referenceData.maxStackSize);
                // Replace the "empty" InventoryItem object with a new, "filled" one.
                InventoryItem newItem = new InventoryItem(referenceData, amountForNewStack);
                inventory[i] = newItem;

                if (!m_itemDictionary.ContainsKey(referenceData))
                {
                    m_itemDictionary.Add(referenceData, newItem);
                }
                quantityRemaining -= amountForNewStack;
            }
        }
    }

    public void Remove(InventoryItemData referenceData, int quantity)
    {
        if (referenceData == null || quantity <= 0)
        {
            return;
        }

        inventoryUpdated = true;
        int quantityToRemove = quantity;

        for (int i = inventory.Count - 1; i >= 0; i--)
        {
            if (quantityToRemove == 0) break;

            InventoryItem currentItem = inventory[i];
            // Ensure currentItem and its data are not null before processing
            if (currentItem != null && currentItem.data == referenceData)
            {
                int amountCanBeTakenFromStack = currentItem.stackSize;
                int amountToTake = Mathf.Min(quantityToRemove, amountCanBeTakenFromStack);

                currentItem.RemoveFromStack(amountToTake);
                quantityToRemove -= amountToTake;

                if (currentItem.stackSize == 0)
                {
                    InventoryItemData dataOfEmptiedItem = currentItem.data; // Store data before nulling

                    bool wasRegistered = m_itemDictionary.TryGetValue(dataOfEmptiedItem, out InventoryItem registeredItem) && registeredItem == currentItem;

                    currentItem.data = null; // Mark the InventoryItem as empty by nulling its data field.
                                             // The InventoryItem object itself remains in the list.

                    if (wasRegistered)
                    {
                        m_itemDictionary.Remove(dataOfEmptiedItem);
                        // Try to find another stack of the same original item type to update the dictionary
                        for (int j = 0; j < inventory.Count; j++)
                        {
                            if (inventory[j] != null && inventory[j].data == dataOfEmptiedItem)
                            {
                                m_itemDictionary.Add(dataOfEmptiedItem, inventory[j]);
                                break;
                            }
                        }
                    }
                }
            }
        }

        if (m_itemDictionary.ContainsKey(referenceData))
        {
            bool itemTypeStillExists = false;
            foreach (InventoryItem itemCheck in inventory)
            {
                if (itemCheck != null && itemCheck.data == referenceData)
                {
                    itemTypeStillExists = true;
                    break;
                }
            }
            if (!itemTypeStillExists)
            {
                m_itemDictionary.Remove(referenceData);
            }
        }
    }

    public bool HasItems(InventoryItemData referenceData, int requiredQuantity)
    {
        if (referenceData == null || requiredQuantity <= 0)
        {
            return requiredQuantity <= 0;
        }

        int totalCount = 0;
        foreach (InventoryItem item in inventory)
        {
            if (item != null && item.data == referenceData)
            {
                totalCount += item.stackSize;
                if (totalCount >= requiredQuantity)
                {
                    return true;
                }
            }
        }
        return totalCount >= requiredQuantity;
    }

    public bool InventoryUpdated()
    {
        if (inventoryUpdated)
        {
            inventoryUpdated = false;
            return true;
        }
        else
        {
            return false;
        }
    }
}
// End of changed code block