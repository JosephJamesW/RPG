using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image m_icon;

    [SerializeField] TextMeshProUGUI m_label;

    [SerializeField] GameObject m_stackObj;

    [SerializeField] TextMeshProUGUI m_stackLabel;

    public void Set(InventoryItem item)
    {
        m_icon.sprite = item.data.icon;
        m_label.text = item.data.displayName;

        // If the stack is greater than one, show the stack count.
        if (item.stackSize > 1)
        {
            m_stackObj.SetActive(true);
            m_stackLabel.text = item.stackSize.ToString();
        }
        // Otherwise, hide it.
        else
        {
            m_stackObj.SetActive(false);
        }
    }
}