// PlayerInventoryUI.cs
// Start of changed code block
using GinjaGaming.FinalCharacterController;
using System.Collections.Generic;
using Unity.VisualScripting; // Not used, consider removing if not needed elsewhere
using UnityEngine;
//using static UnityEditor.Progress; // Not used, consider removing

public class InventoryUI : MonoBehaviour
{
    [SerializeField] PlayerManager playerManager;
    [SerializeField] Transform playerInventoryTransform;
    [SerializeField] Transform interactingInventoryTransform;
    [SerializeField] Transform tradeTransform;
    [SerializeField] GameObject tradeSlotPrefab;
    [SerializeField] GameObject itemFramePrefab;
    [SerializeField] GameObject itemSlotPrefab;

    private Transform[] itemPositionTransforms; // New variable that has not been intergrated yet.
    private bool refreshTradeQueued = false;

    private void Awake()
    {
        tradeTransform.gameObject.SetActive(false);
        // itemPositionTransforms = GetComponentsInChildren<Transform>(); // Original line, kept as is.
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
        else if (!playerManager._playerLocomotionInput.InventoryToggleOn &&
            playerInventoryTransform.gameObject.activeSelf && !tradeTransform.gameObject.activeSelf &&
            !interactingInventoryTransform.gameObject.activeSelf)
            ClosePlayerInventory();
        else if (playerManager.playerInventorySystem.InventoryUpdated())
        {
            if (tradeTransform.gameObject.activeSelf)
                refreshTradeQueued = true;
            UpdateInventory(playerInventoryTransform, playerManager.playerInventorySystem.inventory);
        }
    }

    private void OpenPlayerInventory()
    {
        UpdateInventory(playerInventoryTransform, playerManager.playerInventorySystem.inventory);
        playerInventoryTransform.gameObject.SetActive(true);
    }

    private void ClosePlayerInventory()
    {
        playerInventoryTransform.gameObject.SetActive(false);
    }
    private void UpdateInteractingInventory()
    {
        if (playerManager.playerInventorySystem.CurrentInteractingInventory != null &&
            !interactingInventoryTransform.gameObject.activeSelf)
            OpenInteractingInventory();
        else if (interactingInventoryTransform.gameObject.activeSelf &&
            playerManager._playerLocomotionInput.LockedInteractPressed)
            CloseInteractingInventory();
        else if (playerManager.playerInventorySystem.CurrentInteractingInventory != null &&
            playerManager.playerInventorySystem.CurrentInteractingInventory.InventoryUpdated())
            UpdateInventory(interactingInventoryTransform, playerManager.playerInventorySystem.CurrentInteractingInventory.inventory);
    }

    private void OpenInteractingInventory()
    {
        interactingInventoryTransform.gameObject.SetActive(true);
        SwitchPlayerLocked();
        UpdateInventory(interactingInventoryTransform, playerManager.playerInventorySystem.CurrentInteractingInventory.inventory);
        OpenPlayerInventory();
    }

    private void CloseInteractingInventory()
    {
        playerManager.playerInventorySystem.CurrentInteractingInventory = null;
        interactingInventoryTransform.gameObject.SetActive(false);
        SwitchPlayerLocked();
    }

    private void UpdateTrade()
    {
        if (playerManager.playerInventorySystem.CurrentTrader != null &&
            !tradeTransform.gameObject.activeSelf)
            OpenTrade();
        else if (tradeTransform.gameObject.activeSelf &&
            playerManager._playerLocomotionInput.LockedInteractPressed)
            CloseTrade();
        else if (refreshTradeQueued)
            RefreshTrade();
    }

    private void OpenTrade()
    {
        tradeTransform.gameObject.SetActive(true);
        SwitchPlayerLocked();
        FillTrade();
        OpenPlayerInventory();
    }

    public void CloseTrade()
    {
        playerManager.playerInventorySystem.CurrentTrader = null;
        tradeTransform.gameObject.SetActive(false);
        SwitchPlayerLocked();
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

    private void UpdateInventory(Transform uITransform, List<InventoryItem> inventory)
    {
        ClearInventory(uITransform);
        DrawInventorty(uITransform, inventory);
    }

    private void ClearInventory(Transform uITransform)
    {
        foreach (Transform t in uITransform)
        {
            Destroy(t.gameObject);
        }
    }
    public void DrawInventorty(Transform uITransform, List<InventoryItem> inventory)
    {
        if (inventory == null) return;

        // Iterate through all InventoryItem objects in the list.
        // An ItemFrame is created for every slot.
        // An ItemSlot is only created if the InventoryItem object has non-null data.
        foreach (InventoryItem item in inventory)
        {
            // Robustness: Check if 'item' itself is null, though the expectation is a list of InventoryItem objects.
            if (item == null)
            {
                // This case implies a 'null' entry in the list, which is different from an 'InventoryItem object with null data'.
                // Create an empty frame for such a slot too, to maintain layout integrity.
                GameObject emptyFrameForNullEntry = Instantiate(itemFramePrefab);
                emptyFrameForNullEntry.transform.SetParent(uITransform, false);
                continue; // Skip trying to access 'item.data'
            }

            GameObject frameInstance = Instantiate(itemFramePrefab);
            frameInstance.transform.SetParent(uITransform, false);

            // An item is considered "filled" if its 'data' field is not null.
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
            // If item.data is null, an empty frame is displayed for that slot.
        }
    }

    private void SwitchPlayerLocked()
    {
        playerManager._playerLocomotionInput.inputLocked = !playerManager._playerLocomotionInput.inputLocked;
        playerManager.CursorLockSwitch();
    }
}
// End of changed code block