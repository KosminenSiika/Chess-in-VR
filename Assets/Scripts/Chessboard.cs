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
        {
            squares[square.X, square.Y] = square;
        }

    }

    public void InstantiatePieces()
    {
        foreach (ChessboardSquare square in squares)
        {
            if (square != null)
            {
                square.InstantiateChessPiece();
            }
        }
    }
}
