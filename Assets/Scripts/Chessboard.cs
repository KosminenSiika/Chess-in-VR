using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    public ChessboardSquare[,] squares = new ChessboardSquare[9, 9];
    public List<ChessboardSquare[]> moveList = new List<ChessboardSquare[]>();
    public List<ChessPieceBase> activePieces = new List<ChessPieceBase>();

    // Start is called before the first frame update
    void Start()
    {
        ChessboardSquare[] squareArray = GetComponentsInChildren<ChessboardSquare>();
        foreach (ChessboardSquare square in squareArray)
            squares[square.X, square.Y] = square;

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

        activePieces.Clear();
    }

    public void ClearAllHighlights()
    {
        foreach (ChessboardSquare square in squares)
            if (square != null)
            {
                square.isEngineLastMove = false;

                if (square.currentHighlight != HighlightColour.None)
                    square.SetSquareHighlight(HighlightColour.None);

                if (square.pieceOnTop != null)
                {
                    PieceHighlightHandler highlightHandler = square.pieceOnTop.GetComponent<PieceHighlightHandler>();
                    if (highlightHandler.currentHighlight != HighlightColour.None)
                        highlightHandler.SetNoHighlight();
                }
            }
    }
}
