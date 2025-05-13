using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Chessboard : MonoBehaviour
{
    [SerializeField] private ChessGameManager gameManager;

    public ChessboardSquare[,] squares = new ChessboardSquare[9, 9];
    public List<ChessboardSquare[]> moveList = new List<ChessboardSquare[]>();
    public List<ChessPieceBase> activePieces = new List<ChessPieceBase>();
    public List<ChessPieceBase> deadWhitePieces = new List<ChessPieceBase>();
    public List<ChessPieceBase> deadBlackPieces = new List<ChessPieceBase>();

    public float tileSize;
    public float deadScale;
    public float deathSpacing;

    // Start is called before the first frame update
    void Start()
    {
        ChessboardSquare[] squareArray = GetComponentsInChildren<ChessboardSquare>();
        foreach (ChessboardSquare square in squareArray)
            squares[square.X, square.Y] = square;

        tileSize = Mathf.Abs(squares[1, 1].transform.position.x - squares[2, 2].transform.position.x);
    }

    public void InstantiateNewPieces()
    {
        DestroyAllPieces();

        foreach (ChessboardSquare square in squares)
            if (square != null)
                square.InstantiateChessPiece();
    }
    private void DestroyAllPieces()
    {
        foreach (ChessPieceBase piece in activePieces)
            Destroy(piece.gameObject);

        foreach (ChessPieceBase piece in deadWhitePieces)
            Destroy(piece.gameObject);

        foreach (ChessPieceBase piece in deadBlackPieces)
            Destroy(piece.gameObject);

        activePieces.Clear();
        deadWhitePieces.Clear();
        deadBlackPieces.Clear();
    }

    public void ClearAllHighlights()
    {
        foreach (ChessboardSquare square in squares)
            if (square != null)
            {
                square.isEngineLastMove = false;

                if (square.currentHighlight != HighlightColour.None)
                    square.SetSquareHighlight(HighlightColour.None);
            }
        foreach (ChessPieceBase piece in activePieces)
        {
            PieceHighlightHandler highlightHandler = piece.GetComponent<PieceHighlightHandler>();
            if (highlightHandler.currentHighlight != HighlightColour.None)
                highlightHandler.SetNoHighlight();
        }
    }
    
    public void PositionDeadPiece(ChessPieceBase deadPiece)
    {
        activePieces.Remove(deadPiece);
        deadPiece.GetComponent<XRGrabInteractable>().interactionLayers = InteractionLayerMask.GetMask("Nothing");
        deadPiece.transform.localScale = (Vector3.one * deadScale);
        deadPiece.currentSquare = null;

        if (deadPiece.isWhite)
        {
            deadWhitePieces.Add(deadPiece);

            deadPiece.transform.position = squares[1, 8].piecePlaceTransform.position
                + (gameManager.isPlayerWhite ? new Vector3(-tileSize, 0, -tileSize) : new Vector3(tileSize, 0, tileSize))
                + (gameManager.isPlayerWhite ? Vector3.right : Vector3.left) * deathSpacing * (deadWhitePieces.Count - 1);
        }
        else
        {
            deadBlackPieces.Add(deadPiece);

            deadPiece.transform.position = squares[8, 1].piecePlaceTransform.position
                + (!gameManager.isPlayerWhite ? new Vector3(-tileSize, 0, -tileSize) : new Vector3(tileSize, 0, tileSize))
                + (!gameManager.isPlayerWhite ? Vector3.right : Vector3.left) * deathSpacing * (deadBlackPieces.Count - 1);
        }
    }
}
