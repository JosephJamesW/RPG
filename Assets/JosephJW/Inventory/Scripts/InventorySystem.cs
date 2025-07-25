using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    private Dictionary<InventoryItemData, InventoryItem> m_itemDictionary;
    [SerializeField] public List<InventoryItem> inventory = new List<InventoryItem>();
    //public InventoryItem holdingItem;
    [SerializeField] public PlayerHolding playerHolding;

    private int returnIndex;

    [Header("Item Dropping")]
    [SerializeField] private Transform dropDirectionRefTransform;
    [SerializeField] private Vector3 dropItemPositionOffset;
    [SerializeField] private float dropForce = 5f;

    private bool inventoryUpdated = false;

    public TradeManager CurrentTrader { get; set; }
    public InventorySystem CurrentInteractingInventory { get; set; }

    private void Awake()
    {
        m_itemDictionary = new Dictionary<InventoryItemData, InventoryItem>();
        if (inventory != null)
        {
            foreach (InventoryItem item in inventory)
            {
                if (item != null && item.data != null)
                {
                    item.AddToStack(0);

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
        // Add logic to drop item if no space to go

        if (referenceData == null || quantity <= 0)
        {
            return;
        }

        inventoryUpdated = true;
        int quantityRemaining = quantity;

        // First pass: Add to existing stacks
        for (int i = 0; i < inventory.Count; i++)
        {
            if (quantityRemaining == 0) break;

            InventoryItem existingItem = inventory[i];
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

        // Second pass: Place in empty slots
        for (int i = 0; i < inventory.Count; i++)
        {
            if (quantityRemaining == 0) break;

            if (inventory[i] == null || (inventory[i] != null && inventory[i].data == null))
            {
                int amountForNewStack = Mathf.Min(quantityRemaining, referenceData.maxStackSize);
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
            if (currentItem != null && currentItem.data == referenceData)
            {
                int amountCanBeTakenFromStack = currentItem.stackSize;
                int amountToTake = Mathf.Min(quantityToRemove, amountCanBeTakenFromStack);

                currentItem.RemoveFromStack(amountToTake);
                quantityToRemove -= amountToTake;

                if (currentItem.stackSize == 0)
                {
                    // We use 'referenceData' here because 'currentItem.data' has just been set to null.
                    bool wasRegistered = m_itemDictionary.TryGetValue(referenceData, out InventoryItem registeredItem) && registeredItem == currentItem;

                    inventory[i] = null;

                    if (wasRegistered)
                    {
                        m_itemDictionary.Remove(referenceData);

                        // Find the next available stack of the same item type to register in the dictionary.
                        for (int j = 0; j < inventory.Count; j++)
                        {
                            if (inventory[j] != null && inventory[j].data == referenceData)
                            {
                                m_itemDictionary.Add(referenceData, inventory[j]);
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

    public void SelectSlot (int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventory.Count) return;

        bool inventorySlotEmpty = inventory[slotIndex] == null || inventory[slotIndex].data == null;
        bool playerHoldingEmpty = playerHolding.holdingItem == null || playerHolding.GetData() == null;

        if (inventorySlotEmpty && playerHoldingEmpty) return;

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (!inventorySlotEmpty && inventory[slotIndex].data.prefab != null) DropItem(slotIndex);
            return;
        }

        if (!playerHoldingEmpty)
        {
            // Intergrate with input system.
            int amountToMove = Input.GetKey(KeyCode.LeftControl) ? 1 : playerHolding.GetStackSize();

            if (inventorySlotEmpty)
            {
                // Create a new item in the empty slot with the correct data and quantity.
                inventory[slotIndex] = new InventoryItem(playerHolding.GetData(), amountToMove);

                // Remove the moved quantity from the item being held.
                playerHolding.holdingItem.RemoveFromStack(amountToMove);
            }
            else if (playerHolding.GetData() == inventory[slotIndex].data && amountToMove + inventory[slotIndex].stackSize <= inventory[slotIndex].data.maxStackSize)
            {
                inventory[slotIndex].AddToStack(amountToMove);
                playerHolding.holdingItem.RemoveFromStack(amountToMove);
            }
            else
            {
                InventoryItem tempItem = inventory[slotIndex];
                inventory[slotIndex] = playerHolding.holdingItem;
                playerHolding.holdingItem = tempItem;
            }
        }
        else
        {
            // Intergrate with input system.
            int amountToMove = Input.GetKey(KeyCode.LeftControl) ? 1 : inventory[slotIndex].stackSize;

            if (inventory[slotIndex] != null || inventory[slotIndex].data != null)
            {
                playerHolding.SetData(inventory[slotIndex].data);
                playerHolding.SetStackSize(amountToMove);
                inventory[slotIndex].RemoveFromStack(amountToMove);
                returnIndex = slotIndex;
            }
        }

        inventoryUpdated = true;
    }


    public void ReturnItem()
    {
        if (playerHolding.holdingItem == null) return;
        if (playerHolding.GetData() != null)
        {
            if (inventory[returnIndex] == null || inventory[returnIndex].data == null)
            {
                inventory[returnIndex] = playerHolding.holdingItem;
            }
            else
            {
                Add(playerHolding.GetData(), playerHolding.GetStackSize());
            }
            playerHolding.SetData(null);
            playerHolding.SetStackSize(0);
        }
    }

    public void DropItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventory.Count) return;

        InventoryItem itemToDrop = inventory[slotIndex];

        if (itemToDrop == null || itemToDrop.data == null) return;

        InventoryItemData referenceData = itemToDrop.data;

        if (referenceData.prefab != null)
        {
            GameObject droppedItemObject = Instantiate(referenceData.prefab, (transform.position + dropItemPositionOffset), transform.rotation);

            if (dropDirectionRefTransform != null)
            {
                Rigidbody rb = droppedItemObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 throwDirection = new Vector3(dropDirectionRefTransform.forward.x, 0f, dropDirectionRefTransform.forward.z).normalized;

                    rb.linearVelocity = throwDirection * dropForce;
                }
            }
        }

        inventoryUpdated = true;
        itemToDrop.RemoveFromStack(1);

        if (itemToDrop.stackSize == 0)
        {
            bool wasRegistered = m_itemDictionary.TryGetValue(referenceData, out InventoryItem registeredItem) && registeredItem == itemToDrop;
            inventory[slotIndex] = null;

            if (wasRegistered)
            {
                m_itemDictionary.Remove(referenceData);
                for (int j = 0; j < inventory.Count; j++)
                {
                    if (inventory[j] != null && inventory[j].data == referenceData)
                    {
                        m_itemDictionary.Add(referenceData, inventory[j]);
                        break;
                    }
                }
            }
        }
    }
}