using UnityEngine;

public class TradeSlot : MonoBehaviour
{
    [SerializeField] public Transform askingTransform;
    [SerializeField] public Transform givingTransform;
    public InventoryUI inventoryUI;
    public int TradeIndex { get; set; }

    public void SelectThisTrade()
    {
        inventoryUI.SelectTrade(TradeIndex);
    }
}