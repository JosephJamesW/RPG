using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactText;
    [SerializeField] private SpeechHandler speechHandler;
    [SerializeField] private TradeManager tradeManager;

    private void Awake()
    {
        speechHandler = GetComponent<SpeechHandler>();
    }

    public void Interact(Transform interactorTransform)
    {
        speechHandler.Talk(interactorTransform);
        if (tradeManager)
        {
            InventorySystem interactorInventorySystem = interactorTransform.GetComponent<InventorySystem>();
            if (interactorInventorySystem)
            {
                interactorInventorySystem.CurrentTrader = tradeManager;
            }
        }
    }

    public string GetInteractText()
    {
        return interactText;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool IsInteractable()
    {
        return !speechHandler.GetActive();
    }
}

/* References:
 * 
 *  - https://www.youtube.com/watch?v=LdoImzaY6M4&t=811s&ab_channel=CodeMonkey
 */
