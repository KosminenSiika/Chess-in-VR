using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    public ChessboardSquare[,] squares = new ChessboardSquare[9, 9];
    public List<ChessboardSquare[]> moveList = new List<ChessboardSquare[]>();

    // Start is called before the first frame update
    void Start()
    {
        ChessboardSquare[] squareArray = GetComponentsInChildren<ChessboardSquare>();
        foreach (ChessboardSquare square in squareArray)
            squares[square.X, square.Y] = square;

    }

    public void InstantiatePieces()
    {
        foreach (ChessboardSquare square in squares)
            if (square != null)
                square.InstantiateChessPiece();
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
