using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    ChessboardSquare[] squareArray;

    // Start is called before the first frame update
    void Start()
    {
        squareArray = GetComponentsInChildren<ChessboardSquare>();
        foreach (ChessboardSquare square in squareArray)
        {
            square.InstantiateChessPiece();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
