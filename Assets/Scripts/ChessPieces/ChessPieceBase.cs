using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPieceBase : MonoBehaviour
{
    [SerializeField] private GameObject model;

    public Chessboard chessboard;
    public ChessboardSquare previousSquare;
    public ChessboardSquare currentSquare;

    public bool isWhite;

    // Start is called before the first frame update
    void Start()
    {
        model.transform.Rotate(0, chessboard.transform.eulerAngles.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
