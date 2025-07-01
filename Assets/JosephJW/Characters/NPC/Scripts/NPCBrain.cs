using System.Collections.Generic;
using UnityEngine;

public class NPCBrain : MonoBehaviour
{
    [SerializeField] private Dictionary<InventoryItemData, InventoryItem> m_tradeDictionary;
    private float health;
    private float hunger;
    private float hungerSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
