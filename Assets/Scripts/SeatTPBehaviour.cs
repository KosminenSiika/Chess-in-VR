using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SeatTPBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject plane;
    [SerializeField] private GameObject chair;

    [SerializeField] private ActionBasedContinuousMoveProvider moveProvider;
    [SerializeField] private ActionBasedContinuousTurnProvider turnProvider;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private CharacterControllerDriver characterControllerDriver;

    public void OnTeleportToSeat()
    {
        plane.GetComponent<TeleportationAnchor>().enabled = true;
        chair.GetComponent<TeleportationAnchor>().enabled = false;

        moveProvider.enabled = false;
        //characterControllerDriver.enabled = false;
        characterController.enabled = false;
    }

    public void OnTeleportToPlane()
    {
        plane.GetComponent<TeleportationAnchor>().enabled = false;
        chair.GetComponent<TeleportationAnchor>().enabled = true;

        moveProvider.enabled = true;
        characterController.enabled = true;
        //characterControllerDriver.enabled = true;
    }


}
