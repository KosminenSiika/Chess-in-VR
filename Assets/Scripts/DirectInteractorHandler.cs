using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DirectInteractorHandler : MonoBehaviour
{
    [SerializeField] XRDirectInteractor otherDirectInteractor;

    private bool canRotate = false;

    // Update is called once per frame
    void Update()
    {
        if (!canRotate)
        {
            transform.rotation = Quaternion.Euler(transform.parent.rotation.x * -1.0f, 0.0f, transform.parent.rotation.z * -1.0f);
        }
    }

    public void EnableOtherInteractor() { otherDirectInteractor.enabled = true; }
    public void DisableOtherInteractor() { otherDirectInteractor.enabled = false; }

    public void EnableRotation() { canRotate = true; }
    public void DisableRotation() { canRotate = false; }
}
