using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSquareSelector : MonoBehaviour
{
    private ChessboardSquare currentlyHovering;
    private HighlightColour previousHighlight;
    private LayerMask layerMask;
    private bool raycastEnabled = false;


    private void Awake()
    {
        layerMask = LayerMask.GetMask("ChessboardSquare");
    }

    // Update is called once per frame
    void Update()
    {
        if (raycastEnabled)
        {
            CheckHoveringSquare();
        }
    }

    private void CheckHoveringSquare()
    {
        ChessboardSquare raycastTarget;

        // Find target square under the piece
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, Mathf.Infinity, layerMask);
        if (hits.Length > 0)
        {
            raycastTarget = hits[0].collider.GetComponent<ChessboardSquare>();
            if (raycastTarget != currentlyHovering)
            {
                ChangeHoveringSquare(raycastTarget);
            }
        }
        else if (currentlyHovering != null)
        {
            currentlyHovering.SetSquareHighlight(previousHighlight);
            currentlyHovering = null;
        }
    }

    private void ChangeHoveringSquare(ChessboardSquare newSquare)
    {
        if (currentlyHovering != null)
            currentlyHovering.SetSquareHighlight(previousHighlight);

        currentlyHovering = newSquare;
        previousHighlight = currentlyHovering.currentHighlight;
        currentlyHovering.SetSquareHighlight(HighlightColour.Hovering);
    }

    public void EnableSquareRaycast()
    {
        raycastEnabled = true;
    }

    public void DisableSquareRaycast()
    {
        raycastEnabled = false;
    }

    public void OnPieceDropped()
    {
        if (currentlyHovering != null)
        {
            currentlyHovering.SetSquareHighlight(HighlightColour.None);
            currentlyHovering = null;
        }
    }
}
