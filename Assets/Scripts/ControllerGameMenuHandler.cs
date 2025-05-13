using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControllerGameMenuHandler : MonoBehaviour
{
    [SerializeField] private ChessGameManager gameManager;
    [SerializeField] private GameObject controllerGameMenu;
    [SerializeField] private InputActionReference OpenCloseGameMenu;

    [SerializeField] private Button NewMatchButton;
    [SerializeField] private Button QuitGameButton;

    private void OnEnable()
    {
        OpenCloseGameMenu.action.Enable();
        OpenCloseGameMenu.action.performed += ToggleMenuVisibility;
    }

    private void OnDisable()
    {
        OpenCloseGameMenu.action.Disable();
        OpenCloseGameMenu.action.performed -= ToggleMenuVisibility;
    }

    private void ToggleMenuVisibility(InputAction.CallbackContext context)
    {
        controllerGameMenu.SetActive(!controllerGameMenu.activeSelf);
    }

    // Called by OnClick in NewMatchButton
    public void StartNewMatch()
    {
        controllerGameMenu.SetActive(!controllerGameMenu.activeSelf);
        gameManager.ResetGame();
    }

    // Called by OnClick in QuitButton
    public void QuitGame()
    {
        Application.Quit();
    }
}
