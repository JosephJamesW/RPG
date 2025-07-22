using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteractUI : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private CharacterInteract playerInteract;
    [SerializeField] private TextMeshProUGUI interactText;

    private void Update()
    {
        if (playerInteract.GetInteractableObject() != null)
        {
            Show(playerInteract.GetInteractableObject());
        }
        else
        {
            Hide();
        }
    }

    private void Show(IInteractable interactable)
    {
        container.SetActive(true);
        interactText.text = interactable.GetInteractText();
    }

    private void Hide()
    {
        container.SetActive(false);
    }
}
