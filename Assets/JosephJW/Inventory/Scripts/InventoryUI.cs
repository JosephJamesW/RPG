using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] PlayerManager playerManager;
    [SerializeField] Transform playerInventoryTransform;
    [SerializeField] Transform interactingInventoryTransform;
    [SerializeField] Transform tradeTransform;
    [SerializeField] GameObject tradeSlotPrefab;
    [SerializeField] GameObject itemFramePrefab;
    [SerializeField] GameObject itemSlotPrefab;

    private bool refreshTradeQueued = false;

    private void Awake()
    {
        tradeTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdatePlayerInventory();
        UpdateInteractingInventory();
        UpdateTrade();
    }

    private void UpdatePlayerInventory()
    {
        if (playerManager._playerLocomotionInput.InventoryToggleOn &&
            !playerInventoryTransform.gameObject.activeSelf)
            OpenPlayerInventory();
        else if ((playerManager._playerLocomotionInput.InventoryToggleOn || playerManager._playerLocomotionInput.LockedInteractPressed) &&
            playerInventoryTransform.gameObject.activeSelf)
            ClosePlayerInventory();
        else if (playerManager.playerInventorySystem.InventoryUpdated())
        {
            if (tradeTransform.gameObject.activeSelf)
                refreshTradeQueued = true;
            UpdateInventory(playerInventoryTransform, playerManager.playerInventorySystem);
        }
    }

    private bool AdditionalUIOpen()
    {
        return tradeTransform.gameObject.activeSelf || interactingInventoryTransform.gameObject.activeSelf;
    }

    private void OpenPlayerInventory()
    {
        SwitchPlayerLocked();
        UpdateInventory(playerInventoryTransform, playerManager.playerInventorySystem);
        playerInventoryTransform.gameObject.SetActive(true);
    }

    private void ClosePlayerInventory()
    {
        SwitchPlayerLocked();
        playerInventoryTransform.gameObject.SetActive(false);
    }
    private void UpdateInteractingInventory()
    {
        if (playerManager.playerInventorySystem.CurrentInteractingInventory != null &&
            !interactingInventoryTransform.gameObject.activeSelf)
            OpenInteractingInventory();
        else if (interactingInventoryTransform.gameObject.activeSelf && !playerInventoryTransform.gameObject.activeSelf)
            CloseInteractingInventory();
        else if (playerManager.playerInventorySystem.CurrentInteractingInventory != null &&
            playerManager.playerInventorySystem.CurrentInteractingInventory.InventoryUpdated())
            UpdateInventory(interactingInventoryTransform, playerManager.playerInventorySystem.CurrentInteractingInventory);
    }

    private void OpenInteractingInventory()
    {
        interactingInventoryTransform.gameObject.SetActive(true);
        UpdateInventory(interactingInventoryTransform, playerManager.playerInventorySystem.CurrentInteractingInventory);
        OpenPlayerInventory();
    }

    private void CloseInteractingInventory()
    {
        playerManager.playerInventorySystem.CurrentInteractingInventory = null;
        interactingInventoryTransform.gameObject.SetActive(false);
    }

    private void UpdateTrade()
    {
        if (playerManager.playerInventorySystem.CurrentTrader != null &&
            !tradeTransform.gameObject.activeSelf)
            OpenTrade();
        else if (tradeTransform.gameObject.activeSelf && !playerInventoryTransform.gameObject.activeSelf)
            CloseTrade();
        else if (refreshTradeQueued)
            RefreshTrade();
    }

    private void OpenTrade()
    {
        tradeTransform.gameObject.SetActive(true);
        FillTrade();
        OpenPlayerInventory();
    }

    public void CloseTrade()
    {
        playerManager.playerInventorySystem.CurrentTrader = null;
        tradeTransform.gameObject.SetActive(false);
        ClearInventory(tradeTransform.transform);
    }

    public void SelectTrade(int tradeIndex)
    {
        playerManager.playerInventorySystem.CurrentTrader.AcceptTrade(playerManager.playerInventorySystem, tradeIndex);
        refreshTradeQueued = true;
    }

    private void RefreshTrade()
    {
        ClearInventory(tradeTransform.transform);
        FillTrade();
        refreshTradeQueued = false;
    }

    private void FillTrade()
    {
        TradeOffer[] tradeOffers = playerManager.playerInventorySystem.CurrentTrader.ValidTrades(playerManager.playerInventorySystem);
        foreach (TradeOffer tradeOffer in tradeOffers)
        {
            GameObject tradeSlotObj = Instantiate(tradeSlotPrefab);
            tradeSlotObj.transform.SetParent(tradeTransform, false);

            TradeSlot tradeSlot = tradeSlotObj.GetComponent<TradeSlot>();
            tradeSlot.inventoryUI = this;
            tradeSlot.TradeIndex = playerManager.playerInventorySystem.CurrentTrader.GetTradeIndex(tradeOffer);

            DrawInventorty(tradeSlot.askingTransform, tradeOffer.GetItemsAsking());
            DrawInventorty(tradeSlot.givingTransform, tradeOffer.GetItemsGiving());
        }
    }

    private void UpdateInventory(Transform uITransform, InventorySystem inventorySystem)
    {
        ClearInventory(uITransform);
        DrawInventorty(uITransform, inventorySystem.inventory, inventorySystem);
    }

    private void ClearInventory(Transform uITransform)
    {
        foreach (Transform t in uITransform)
        {
            Destroy(t.gameObject);
        }
    }

    // Overloaded to support inventories that should not have clickable/droppable items (e.g., trade UI)
    public void DrawInventorty(Transform uITransform, List<InventoryItem> inventory)
    {
        DrawInventorty(uITransform, inventory, null);
    }

    // Main drawing method with an optional InventorySystem to enable dropping items
    public void DrawInventorty(Transform uITransform, List<InventoryItem> inventory, InventorySystem inventorySystem = null)
    {
        if (inventory == null) return;

        // Use a 'for' loop to get the index of each slot
        for (int i = 0; i < inventory.Count; i++)
        {
            InventoryItem item = inventory[i];

            if (item == null)
            {
                GameObject emptyFrameForNullEntry = Instantiate(itemFramePrefab);
                emptyFrameForNullEntry.transform.SetParent(uITransform, false);
                continue;
            }

            GameObject frameInstance = Instantiate(itemFramePrefab);
            frameInstance.transform.SetParent(uITransform, false);

            // If an inventorySystem is provided, set up the frame for dropping items.
            if (inventorySystem != null)
            {
                ItemFrame itemFrame = frameInstance.GetComponent<ItemFrame>();
                if (itemFrame != null)
                {
                    itemFrame.inventorySystem = inventorySystem;
                    itemFrame.slotIndex = i; // Pass the slot index
                }
                else
                {
                    Debug.LogWarning("ItemFrame component not found on itemFramePrefab. Attach the ItemFrame.cs script to enable dropping items.");
                }
            }

            if (item.data != null)
            {
                GameObject itemSlotInstance = Instantiate(itemSlotPrefab);
                itemSlotInstance.transform.SetParent(frameInstance.transform, false);

                ItemSlot slotComponent = itemSlotInstance.GetComponent<ItemSlot>();
                if (slotComponent != null)
                {
                    slotComponent.Set(item);
                }
                else
                {
                    Debug.LogError("ItemSlot component not found on itemSlotPrefab.", itemSlotPrefab);
                }
            }
        }
    }

    private void SwitchPlayerLocked()
    {
        playerManager._playerLocomotionInput.inputLocked = !playerManager._playerLocomotionInput.inputLocked;
        playerManager.CursorLockSwitch();
    }
}