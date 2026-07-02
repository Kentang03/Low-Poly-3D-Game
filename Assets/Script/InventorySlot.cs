using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI quantityText;
    
    private InventoryItem currentItem;
    
    public void SetItem(InventoryItem item)
    {
        currentItem = item;
        
        if (item != null)
        {
            // Update icon
            if (itemIcon != null)
            {
                itemIcon.sprite = item.itemIcon;
                itemIcon.color = Color.white;
                itemIcon.gameObject.SetActive(true);
            }
            
            // Update name
            if (itemNameText != null)
            {
                itemNameText.text = item.itemName;
            }
            
            // Update quantity
            if (quantityText != null)
            {
                quantityText.text = item.quantity > 1 ? $"x{item.quantity}" : "";
            }
        }
        else
        {
            // Clear slot
            if (itemIcon != null)
                itemIcon.gameObject.SetActive(false);
            
            if (itemNameText != null)
                itemNameText.text = "";
            
            if (quantityText != null)
                quantityText.text = "";
        }
    }
}