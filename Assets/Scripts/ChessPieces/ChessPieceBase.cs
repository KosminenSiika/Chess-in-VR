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
    [SerializeField] private GameObject model;

    public Chessboard chessboard;
    public ChessboardSquare previousSquare;
    public ChessboardSquare currentSquare;

    public PieceType pieceType;
    public bool isWhite;
    private bool hasMoved;

    private List<ChessboardSquare> availableMoves = new List<ChessboardSquare>();

    // Start is called before the first frame update
    void Start()
    {
        model.transform.Rotate(0, chessboard.transform.eulerAngles.y, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPickedUp()
    {
        availableMoves.Clear();
        availableMoves = GetAvailableMoves(pieceType, ref chessboard.squares);

    }

    public void OnDropped()
    {
        ChessboardSquare targetSquare;

        // Find target square under the piece
        LayerMask layerMask = LayerMask.GetMask("ChessboardSquare");
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, Mathf.Infinity, layerMask);
        if (hits.Length > 0)
        {
            // Check if target square is an available move
            targetSquare = hits[0].collider.GetComponent<ChessboardSquare>();
        }
        else
        {
            targetSquare = currentSquare;
        }

        // Place piece onto square
        targetSquare.PlaceChessPiece(this);

        if (targetSquare != currentSquare)
        {
            previousSquare = currentSquare;
            currentSquare = targetSquare;

            // Function call to prevent further interaction with pieces -> press chess clock to end turn
        }

    }

    public List<ChessboardSquare> GetAvailableMoves(PieceType pieceType, ref ChessboardSquare[,] squares)
    {
        List<ChessboardSquare> r = new List<ChessboardSquare>();

        return r;
    }
}
