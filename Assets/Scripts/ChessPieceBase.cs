using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King,
}

public class ChessPieceBase : MonoBehaviour
{
    public ChessboardSquare previousSquare;
    public ChessboardSquare currentSquare;

    public PieceType pieceType;
    public bool isWhite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
