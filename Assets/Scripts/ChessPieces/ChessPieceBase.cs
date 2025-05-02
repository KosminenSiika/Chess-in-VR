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

        if (pieceType == PieceType.Pawn)
        {
            int direction = isWhite ? 1 : -1;

            if (currentSquare.Y + direction >= 1 && currentSquare.Y + direction <= 8)
            {
                // One Forward
                ChessboardSquare candidateSquare = squares[currentSquare.X, currentSquare.Y + direction];
                if (candidateSquare.pieceOnTop == null)
                {
                    r.Add(candidateSquare);

                    // Two Forward
                    if (!hasMoved)
                    {
                        candidateSquare = squares[currentSquare.X, currentSquare.Y + 2 * direction];
                        if (candidateSquare.pieceOnTop == null) 
                            r.Add(candidateSquare);
                    }
                        
                }

                // Eat left
                if (currentSquare.X != 1)
                {
                    candidateSquare = squares[currentSquare.X - 1, currentSquare.Y + direction];
                    if (candidateSquare.pieceOnTop != null)
                        if (candidateSquare.pieceOnTop.isWhite != this.isWhite)
                            r.Add(candidateSquare);
                }

                // Eat right
                if (currentSquare.X != 8)
                {
                    candidateSquare = squares[currentSquare.X + 1, currentSquare.Y + direction];
                    if (candidateSquare.pieceOnTop != null)
                        if (candidateSquare.pieceOnTop.isWhite != this.isWhite)
                            r.Add(candidateSquare);
                }
            }
        }

        if (pieceType == PieceType.Rook)
        {
            // Up
            for (int i = currentSquare.Y + 1; i <= 8; i++)
            {
                ChessboardSquare candidateSquare = squares[currentSquare.X, i];
                if (candidateSquare.pieceOnTop == null)
                    r.Add(candidateSquare);
                else
                {
                    if (candidateSquare.pieceOnTop.isWhite != this.isWhite)
                        r.Add(candidateSquare);

                    break;
                }  
            }

            // Down
            for (int i = currentSquare.Y - 1; i >= 1; i--)
            {
                ChessboardSquare candidateSquare = squares[currentSquare.X, i];
                if (candidateSquare.pieceOnTop == null)
                    r.Add(candidateSquare);
                else
                {
                    if (candidateSquare.pieceOnTop.isWhite != this.isWhite)
                        r.Add(candidateSquare);

                    break;
                }
            }

            // Left
            for (int i = currentSquare.X - 1; i >= 1; i--)
            {
                ChessboardSquare candidateSquare = squares[i, currentSquare.Y];
                if (candidateSquare.pieceOnTop == null)
                    r.Add(candidateSquare);
                else
                {
                    if (candidateSquare.pieceOnTop.isWhite != this.isWhite)
                        r.Add(candidateSquare);

                    break;
                }
            }

            // Right
            for (int i = currentSquare.X + 1; i <= 8; i++)
            {
                ChessboardSquare candidateSquare = squares[i, currentSquare.Y];
                if (candidateSquare.pieceOnTop == null)
                    r.Add(candidateSquare);
                else
                {
                    if (candidateSquare.pieceOnTop.isWhite != this.isWhite)
                        r.Add(candidateSquare);

                    break;
                }
            }
        }

        if (pieceType == PieceType.Knight)
        {

        }

        return r;
    }
}
