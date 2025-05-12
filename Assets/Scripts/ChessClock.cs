using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChessClock : MonoBehaviour
{
    [SerializeField] private ChessGameManager gameManager;

    [SerializeField] private XRSimpleInteractable interactableScript;

    [SerializeField] private TextMeshProUGUI botHH;
    [SerializeField] private TextMeshProUGUI botMM;
    [SerializeField] private TextMeshProUGUI botSS;
    [SerializeField] private TextMeshProUGUI playerHH;
    [SerializeField] private TextMeshProUGUI playerMM;
    [SerializeField] private TextMeshProUGUI playerSS;

    private float playerTime = -1;
    private float botTime = -1;

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isPlayerWhite)
        {
            if (playerTime != gameManager.whiteCurrentTime)
                UpdatePlayerTimes();
            if (botTime != gameManager.blackCurrentTime)
                UpdateBotTimes();
        }
        else
        {
            if (playerTime != gameManager.blackCurrentTime)
                UpdatePlayerTimes();
            if(botTime != gameManager.whiteCurrentTime)
                UpdateBotTimes();
        }
    }

    private void UpdateBotTimes()
    {
        if (gameManager.isPlayerWhite)
            botTime = gameManager.blackCurrentTime;
        else
            botTime = gameManager.whiteCurrentTime;

        int botHours = Mathf.FloorToInt(botTime / 3600);
        int botMinutes = Mathf.FloorToInt((botTime - (botHours * 3600)) / 60);
        int botSeconds = Mathf.FloorToInt(botTime - (botHours * 3600) - (botMinutes * 60));
        
        if (botHours < 10)
            botHH.SetText("0" + botHours.ToString());
        else
            botHH.SetText(botHours.ToString());

        if (botMinutes < 10) 
            botMM.SetText("0" + botMinutes.ToString());
        else
            botMM.SetText(botMinutes.ToString());

        if (botSeconds < 10)
            botSS.SetText("0" + botSeconds.ToString());
        else
            botSS.SetText(botSeconds.ToString());
    }
    private void UpdatePlayerTimes()
    {
        if (gameManager.isPlayerWhite)
            playerTime = gameManager.whiteCurrentTime;
        else
            playerTime = gameManager.blackCurrentTime;

        int playerHours = Mathf.FloorToInt(playerTime / 3600);
        int playerMinutes = Mathf.FloorToInt((playerTime - (playerHours * 3600)) / 60);
        int playerSeconds = Mathf.FloorToInt(playerTime - (playerHours * 3600) - (playerMinutes * 60));

        if (playerHours < 10)
            playerHH.SetText("0" + playerHours.ToString());
        else
            playerHH.SetText(playerHours.ToString());

        if (playerMinutes < 10)
            playerMM.SetText("0" + playerMinutes.ToString());
        else
            playerMM.SetText(playerMinutes.ToString());

        if (playerSeconds < 10)
            playerSS.SetText("0" + playerSeconds.ToString());
        else
            playerSS.SetText(playerSeconds.ToString());
    }

    public void EndPlayerTurn()
    {
        EnableChessClockInteractable(false);
        gameManager.SwitchTurn();
    }

    public void EnableChessClockInteractable(bool enable)
    {
        if (enable)
            interactableScript.interactionLayers = InteractionLayerMask.GetMask("ChessClock");
        else
            interactableScript.interactionLayers = InteractionLayerMask.GetMask("Nothing");
    }
}
