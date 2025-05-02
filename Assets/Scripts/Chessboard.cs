using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    public ChessboardSquare[,] squares = new ChessboardSquare[9, 9];

    // Start is called before the first frame update
    void Start()
    {
        ChessboardSquare[] squareArray = GetComponentsInChildren<ChessboardSquare>();
        foreach (ChessboardSquare square in squareArray)
        {
            squares[square.X, square.Y] = square;
            square.InstantiateChessPiece();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
