using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class TradeOffer
{
    [SerializeField] public List<InventoryItem> itemsAsking;
    [SerializeField] public List<InventoryItem> itemsGiving;

    public List<InventoryItem> GetItemsAsking() { return itemsAsking; }
    public List<InventoryItem> GetItemsGiving() { return itemsGiving; }
}

public class TradeManager : MonoBehaviour
{
    [SerializeField] TradeOffer[] tradeOffers;
    private InventorySystem inventorySystem;

    private void Awake()
    {
        inventorySystem = GetComponent<InventorySystem>();
    }

    public int GetTradeIndex(TradeOffer tradeOffer)
    {
        return Array.IndexOf(tradeOffers, tradeOffer);
    }

    public TradeOffer[] GetTradeOffers()
    {
        return tradeOffers;
    }

    public bool CheckTrade(InventorySystem recieverInventorySystem, TradeOffer offer)
    {
        foreach (var item in offer.itemsGiving)
        {
            if (!inventorySystem.HasItems(item.data, item.stackSize))
            {
                return false;
            }
        }

        foreach (var item in offer.itemsAsking)
        {
            if (!recieverInventorySystem.HasItems(item.data, item.stackSize))
            {
                return false;
            }
        }

        return true;
    }

    public void AcceptTrade(InventorySystem recieverInventorySystem, int tradeIndex)
    {
        if (CheckTrade(recieverInventorySystem, tradeOffers[tradeIndex]))
        {
            foreach (var item in tradeOffers[tradeIndex].itemsAsking)
            {
                recieverInventorySystem.Remove(item.data, item.stackSize);
                inventorySystem.Add(item.data, item.stackSize);
            }
            foreach (var item in tradeOffers[tradeIndex].itemsGiving)
            {
                recieverInventorySystem.Add(item.data, item.stackSize);
                inventorySystem.Remove(item.data, item.stackSize);
            }
        }
    }
}