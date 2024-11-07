using System.Globalization;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private InGameMenuUI inGameMenuUI;

    private bool inGameMenuActive = false;

    #region InputEvents

    public void OnMove_Player_Input(InputAction.CallbackContext context)
    {
        if (inGameMenuActive)
            return;
        if (Player.LocalInstance == null)
            return;

        Player.LocalInstance.OnMove_Input(context);
    }

    public void OnJump_Player_Input(InputAction.CallbackContext context)
    {
        if (inGameMenuActive)
            return;
        if (Player.LocalInstance == null)
            return;

        Player.LocalInstance.OnJump_Input(context);
    }

    public void OnInteract_Player_Input(InputAction.CallbackContext context)
    {
        if (inGameMenuActive)
            return;
        if (Player.LocalInstance == null)
            return;

        Player.LocalInstance.OnInteract_Input(context);
    }

    public void OnInteractAlternative_Player_Input(InputAction.CallbackContext context)
    {
        if (inGameMenuActive)
            return;
        if (Player.LocalInstance == null)
            return;

        Player.LocalInstance.OnInteractAlternative_Input(context);
    }

    public void OnPause_UI_Input(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        inGameMenuActive = !inGameMenuActive;

        if (inGameMenuActive)
        {
            if(Player.LocalInstance != null)
            {
                Player.LocalInstance.StopMovement();
            }
            inGameMenuUI.Show();
        }
            
        else
            inGameMenuUI.Hide();
    }

    #endregion
}
