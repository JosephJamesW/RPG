using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GinjaGaming.FinalCharacterController
{
    [DefaultExecutionOrder(-2)]
    public class PlayerLocomotionInput : MonoBehaviour, PlayerControls.IPlayerLocomotionMapActions
    {
        #region Class Variables
        [SerializeField] private bool holdToSprint = true;
        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool SprintToggledOn { get; private set; }
        public bool WalkToggledOn { get; private set; }
        public bool InteractPressed { get; private set; }
        #endregion
        public bool LockedInteractPressed { get; private set; }
        public bool ThirdPersonToggleOn { get; private set; } = true;
        public bool InventoryToggleOn { get; private set; }

        public bool inputLocked { get; set; }

        #region Startup
        private void OnEnable()
        {
            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot enable");
                return;
            }

            PlayerInputManager.Instance.PlayerControls.PlayerLocomotionMap.Enable();
            PlayerInputManager.Instance.PlayerControls.PlayerLocomotionMap.SetCallbacks(this);
        }

        private void OnDisable()
        {
            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot disable");
                return;
            }

            PlayerInputManager.Instance.PlayerControls.PlayerLocomotionMap.Disable();
            PlayerInputManager.Instance.PlayerControls.PlayerLocomotionMap.RemoveCallbacks(this);
        }
        #endregion

        #region Late Update Logic
        private void LateUpdate()
        {
            JumpPressed = false;
            InteractPressed = false;
            LockedInteractPressed = false;
            InventoryToggleOn = false;
            if (inputLocked)
            {
                MovementInput = Vector2.zero;
                LookInput = Vector2.zero;
            }
        }
        #endregion

        #region Input Callbacks
        public void OnMovement(InputAction.CallbackContext context)
        {
            if (inputLocked)
                return;

            MovementInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (inputLocked)
                return;

            LookInput = context.ReadValue<Vector2>();
        }

        public void OnToggleSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SprintToggledOn = holdToSprint || !SprintToggledOn;
            }
            else if (context.canceled)
            {
                SprintToggledOn = !holdToSprint && SprintToggledOn;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed || inputLocked)
                return;

            JumpPressed = true;
        }

        public void OnToggleWalk(InputAction.CallbackContext context)
        {
            if (!context.performed || inputLocked)
                return;

            WalkToggledOn = !WalkToggledOn;
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            if (!inputLocked) InteractPressed = true;
            else LockedInteractPressed = true;
        }

        public void OnToggleThirdPerson(InputAction.CallbackContext context)
        {
            if (!context.performed || inputLocked)
                return;

            ThirdPersonToggleOn = !ThirdPersonToggleOn;
        }

        public void OnToggleInventory(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            InventoryToggleOn = true;
        }
        #endregion
    }
}
