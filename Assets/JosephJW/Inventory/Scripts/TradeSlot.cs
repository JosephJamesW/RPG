using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TradeSlot : MonoBehaviour
{
    [SerializeField] public Transform askingTransform;
    [SerializeField] public Transform givingTransform;
    public InventoryUI inventoryUI;
    public int TradeIndex { get; set; }

    [SerializeField] private Button tradeButton;
    [SerializeField] private Image buttonImage;

    private void Awake()
    {
        if (tradeButton != null)
        {
            tradeButton.onClick.AddListener(SelectThisTrade);
        }
    }

    public void SelectThisTrade()
    {
        inventoryUI.SelectTrade(TradeIndex);
    }

    public void SetState(bool isPossible)
    {
        if (tradeButton != null)
        {
            tradeButton.interactable = isPossible;
        }

        if (buttonImage != null)
        {
            buttonImage.color = isPossible ? Color.white : Color.grey;
        }
    }
}