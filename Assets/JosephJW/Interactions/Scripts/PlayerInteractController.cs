using GinjaGaming.FinalCharacterController;
using UnityEngine;

public class PlayerInteractController : MonoBehaviour
{
    [SerializeField] PlayerLocomotionInput playerLocomotionInput;
    [SerializeField] CharacterInteract characterInteract;

    private void Awake()
    {
        playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        characterInteract = GetComponent<CharacterInteract>();
    }
    private void Update()
    {
        if (playerLocomotionInput.InteractPressed)
        {
            characterInteract.Interact();
        }
    }
}
