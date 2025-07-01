using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteractUI : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private CharacterInteract characterInteract;
    [SerializeField] private TextMeshProUGUI interactText;

    private void Update()
    {
        if (characterInteract.GetInteractableObject() != null)
        {
            Show(characterInteract.GetInteractableObject());
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
