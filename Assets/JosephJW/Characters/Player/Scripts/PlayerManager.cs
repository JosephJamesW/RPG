using GinjaGaming.FinalCharacterController;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void CursorLockSwitch()
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.Confined : CursorLockMode.Locked;
    }
}
