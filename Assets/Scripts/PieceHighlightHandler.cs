using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceHighlightHandler : MonoBehaviour
{
    private ChessGameManager gameManager;

    private HighlightColour previousHighlight;
    public HighlightColour currentHighlight = HighlightColour.None;
    private MaterialPropertyBlock m_TintPropertyBlock;
    [SerializeField] private Color HoveringColour = Color.cyan;
    [SerializeField] private Color InCheckColour = Color.red;

    private void Awake()
    {
        m_TintPropertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        gameManager = FindFirstObjectByType<ChessGameManager>();
    }

    public void SetPreviousHighlight()
    {
        if (!gameManager.isPieceMovedThisTurn)
            SetPieceHighlight(previousHighlight);
    }

    public void SetNoHighlight()
    {
        SetPieceHighlight(HighlightColour.None);
    }

    public void SetInCheckHighlight()
    {
        SetPieceHighlight(HighlightColour.InCheck);
    }

    public void SetHoveringHighlight()
    {
        if (gameManager.isWhiteTurn == gameManager.isPlayerWhite || !gameManager.isGameOver)
            SetPieceHighlight(HighlightColour.Hovering);
    }

    private void SetPieceHighlight(HighlightColour highlightColour)
    {
        Color emissionColour;
        if (highlightColour == HighlightColour.None)
            emissionColour = Color.black;
        else if (highlightColour == HighlightColour.Hovering)
            emissionColour = HoveringColour * Mathf.LinearToGammaSpace(1f);
        else if (highlightColour == HighlightColour.InCheck)
            emissionColour = InCheckColour * Mathf.LinearToGammaSpace(1f);
        else
            emissionColour = Color.black;

        foreach (MeshRenderer render in GetComponentsInChildren<MeshRenderer>())
        {
            if (render == null)
                continue;

            // Emissions don't really show on white materials, so we'll darken the material while it's highlighted
            if (render.material.color == Color.gray && highlightColour == HighlightColour.None)
                render.material.color = Color.white;
            else if (render.material.color == Color.white && highlightColour != HighlightColour.None)
                render.material.color = Color.gray;


            render.GetPropertyBlock(m_TintPropertyBlock);
            m_TintPropertyBlock.SetColor(Shader.PropertyToID("_EmissionColor"), emissionColour);
            render.SetPropertyBlock(m_TintPropertyBlock);
        }

        if (currentHighlight != HighlightColour.Hovering)
            previousHighlight = currentHighlight;
        currentHighlight = highlightColour;
    }
}
