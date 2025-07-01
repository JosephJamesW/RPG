using UnityEngine;
// https://www.youtube.com/watch?v=SGz3sbZkfkg
[CreateAssetMenu(menuName = "Inventory Item Data")]
public class InventoryItemData : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public GameObject prefab;
    public int maxStackSize = 100;
}