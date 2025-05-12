using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChessClock : MonoBehaviour
{
    public static int[] SecondsToHHMMSS(float totalSeconds)
    {
        int hours = Mathf.FloorToInt(totalSeconds / 3600);
        int minutes = Mathf.FloorToInt((totalSeconds - (hours * 3600)) / 60);
        int seconds = Mathf.FloorToInt(totalSeconds - (hours * 3600) - (minutes * 60));

        return new int[] { hours, minutes, seconds };
    }

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

        int[] times = SecondsToHHMMSS(botTime);

        if (times[0] < 10)
            botHH.SetText("0" + times[0].ToString());
        else
            botHH.SetText(times[0].ToString());

        if (times[1] < 10) 
            botMM.SetText("0" + times[1].ToString());
        else
            botMM.SetText(times[1].ToString());

        if (times[2] < 10)
            botSS.SetText("0" + times[2].ToString());
        else
            botSS.SetText(times[2].ToString());
    }
    private void UpdatePlayerTimes()
    {
        if (gameManager.isPlayerWhite)
            playerTime = gameManager.whiteCurrentTime;
        else
            playerTime = gameManager.blackCurrentTime;

        int[] times = SecondsToHHMMSS(playerTime);

        if (times[0] < 10)
            botHH.SetText("0" + times[0].ToString());
        else
            botHH.SetText(times[0].ToString());

        if (times[1] < 10)
            botMM.SetText("0" + times[1].ToString());
        else
            botMM.SetText(times[1].ToString());

        if (times[2] < 10)
            botSS.SetText("0" + times[2].ToString());
        else
            botSS.SetText(times[2].ToString());
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
