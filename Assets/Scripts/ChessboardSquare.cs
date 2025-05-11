using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UIElements;

// Used for both squares and pieces
public enum HighlightColour
{
    None,
    Available,
    Hovering,
    InCheck,
    EngineLastMove,
}

public class ChessboardSquare : MonoBehaviour
{
    // References
    public GameObject PieceToInstantiate { get { return pieceToInstantiate; } private set {  pieceToInstantiate = value; } }
    [SerializeField] private Chessboard chessboard;
    [SerializeField] private GameObject pieceToInstantiate;

    // Position
    public int X { get { return x; } private set { x = value; } }
    public int Y { get { return y; } private set { y = value; } }
    [SerializeField] private int x;
    [SerializeField] private int y;

    public ChessPieceBase pieceOnTop;

    // Piece placing
    private float piecePlaceOffset = 2.2f;
    private Transform piecePlaceTransform;

    // Square Highlighting
    public bool isEngineLastMove;
    public HighlightColour currentHighlight;
    private MaterialPropertyBlock m_TintPropertyBlock;
    [SerializeField] private Color AvailableColour = Color.cyan;
    [SerializeField] private Color HoveringColour = Color.green;
    [SerializeField] private Color EngineLastMoveColour = Color.red;

    public bool justQueened = false;

    private void Awake()
    {
        piecePlaceTransform = new GameObject("PiecePlacePos").transform;
        piecePlaceTransform.parent = transform;
        piecePlaceTransform.localPosition = new Vector3(0, piecePlaceOffset, 0);

        m_TintPropertyBlock = new MaterialPropertyBlock();
    }

    // Function for instantiating pieces at game start/reset, all pieces should be deleted before calling these
    public void InstantiateChessPiece()
    {
        if (pieceToInstantiate != null)
        {
            GameObject instantiatedPiece = Instantiate(pieceToInstantiate, piecePlaceTransform.position, transform.localRotation);
            pieceOnTop = instantiatedPiece.GetComponent<ChessPieceBase>();
            pieceOnTop.currentSquare = this;
            pieceOnTop.chessboard = chessboard;
        }
    }

    public void HandleQueening()
    {
        Destroy(pieceOnTop.gameObject);
        
        if (Y == 8)
        {
            GameObject queen = Instantiate(chessboard.squares[4, 1].PieceToInstantiate, piecePlaceTransform.position, transform.localRotation); // White Queen
            pieceOnTop = queen.GetComponent<ChessPieceBase>();
            pieceOnTop.currentSquare = this;
            pieceOnTop.chessboard = chessboard;
        }
        else
        {
            GameObject queen = Instantiate(chessboard.squares[4, 8].PieceToInstantiate, piecePlaceTransform.position, transform.localRotation); // Black Queen
            pieceOnTop = queen.GetComponent<ChessPieceBase>();
            pieceOnTop.currentSquare = this;
            pieceOnTop.chessboard = chessboard;
        }

        justQueened = true;
    }

    public void PlaceChessPiece(ChessPieceBase piece)
    {
        // Eliminate enemy piece if present
        if (pieceOnTop != null && pieceOnTop != piece)
        {
            EliminatePieceOnTop();
        }

        // Place piece onto square
        pieceOnTop = piece;
        piece.transform.position = piecePlaceTransform.position;
        piece.transform.rotation = transform.localRotation;
    }
    public void EliminatePieceOnTop()
    {
        if (pieceOnTop != null)
        {
            Destroy(pieceOnTop.gameObject);
        }
        EmptySquare();
    }

    public void EmptySquare()
    {
        pieceOnTop = null;
    }


    public void SetSquareHighlight(HighlightColour highlightColour)
    {
        Color emissionColour;
        if (highlightColour == HighlightColour.None)
            emissionColour = Color.black;
        else if (highlightColour == HighlightColour.Available)
            emissionColour = AvailableColour * Mathf.LinearToGammaSpace(1f);
        else if (highlightColour == HighlightColour.Hovering)
            emissionColour = HoveringColour * Mathf.LinearToGammaSpace(1f);
        else if (highlightColour == HighlightColour.EngineLastMove)
        {
            emissionColour = EngineLastMoveColour * Mathf.LinearToGammaSpace(1f);
            isEngineLastMove = true;
        }
        else
            emissionColour = Color.black;

        foreach (MeshRenderer render in GetComponents<MeshRenderer>())
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
 
        currentHighlight = highlightColour;
    }

}
