using GinjaGaming.FinalCharacterController;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerLocomotionInput _playerLocomotionInput {  get; private set; }
    public InventorySystem playerInventorySystem { get; private set; }
    private CharacterInteract _characterInteract;

    private void Awake()
    {
        _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        playerInventorySystem = GetComponent<InventorySystem>();
        _characterInteract = GetComponent<CharacterInteract>();
        Cursor.lockState = CursorLockMode.Locked;


    }

    private void Update()
    {
        if (_playerLocomotionInput.InteractPressed)
        {
            _characterInteract.Interact();
        }
    }

    public void CursorLockSwitch()
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.Confined : CursorLockMode.Locked;
    }
}
