using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private bool hasMoved = false;

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

        foreach (ChessboardSquare square in availableMoves)
            square.SetSquareHighlight(HighlightColour.Available);
        // TODO: Highlight availableMoves squares
    }

    public void OnDropped()
    {
        ChessboardSquare targetSquare;

        // Find target square under the piece
        LayerMask layerMask = LayerMask.GetMask("ChessboardSquare");
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, Mathf.Infinity, layerMask);
        if (hits.Length > 0)
        {
            targetSquare = hits[0].collider.GetComponent<ChessboardSquare>();
            if (!availableMoves.Contains(targetSquare))
                targetSquare = currentSquare;
        }
        else
            targetSquare = currentSquare;

        // Place piece onto square
        targetSquare.PlaceChessPiece(this);

        foreach (ChessboardSquare square in availableMoves)
            square.SetSquareHighlight(HighlightColour.None);

        if (targetSquare != currentSquare)
        {
            currentSquare.EmptySquare();

            previousSquare = currentSquare;
            currentSquare = targetSquare;

            if (!hasMoved)
                hasMoved = true;

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
            // Top Right
            int x = currentSquare.X + 1;
            int y = currentSquare.Y + 2;
            if (x <= 8 && y <= 8)
                if (squares[x, y].pieceOnTop == null)
                    r.Add(squares[x, y]);
                else if (squares[x, y].pieceOnTop.isWhite != this.isWhite)
                    r.Add(squares[x, y]);

            x = currentSquare.X + 2;
            y = currentSquare.Y + 1;
            if (x <= 8 && y <= 8)
                if (squares[x, y].pieceOnTop == null)
                    r.Add(squares[x, y]);
                else if (squares[x, y].pieceOnTop.isWhite != this.isWhite)
                    r.Add(squares[x, y]);

            // Top Left
            x = currentSquare.X - 1;
            y = currentSquare.Y + 2;
            if (x >= 1 && y <= 8)
                if (squares[x, y].pieceOnTop == null)
                    r.Add(squares[x, y]);
                else if (squares[x, y].pieceOnTop.isWhite != this.isWhite)
                    r.Add(squares[x, y]);

            x = currentSquare.X - 2;
            y = currentSquare.Y + 1;
            if (x >= 1 && y <= 8)
                if (squares[x, y].pieceOnTop == null)
                    r.Add(squares[x, y]);
                else if (squares[x, y].pieceOnTop.isWhite != this.isWhite)
                    r.Add(squares[x, y]);

            // Bottom Right
            x = currentSquare.X + 1;
            y = currentSquare.Y - 2;
            if (x <= 8 && y  >= 1)
                if (squares[x, y].pieceOnTop == null)
                    r.Add(squares[x, y]);
                else if (squares[x, y].pieceOnTop.isWhite != this.isWhite)
                    r.Add(squares[x, y]);

            x = currentSquare.X + 2;
            y = currentSquare.Y - 1;
            if (x <= 8 && y >= 1)
                if (squares[x, y].pieceOnTop == null)
                    r.Add(squares[x, y]);
                else if (squares[x, y].pieceOnTop.isWhite != this.isWhite)
                    r.Add(squares[x, y]);

            // Bottom Left
            x = currentSquare.X - 1;
            y = currentSquare.Y - 2;
            if (x >= 1 && y >= 1)
                if (squares[x, y].pieceOnTop == null)
                    r.Add(squares[x, y]);
                else if (squares[x, y].pieceOnTop.isWhite != this.isWhite)
                    r.Add(squares[x, y]);

            x = currentSquare.X - 2;
            y = currentSquare.Y - 1;
            if (x >= 1 && y >= 1)
                if (squares[x, y].pieceOnTop == null)
                    r.Add(squares[x, y]);
                else if (squares[x, y].pieceOnTop.isWhite != this.isWhite)
                    r.Add(squares[x, y]);
        }

        if (pieceType == PieceType.Bishop)
        {
            // Top Right
            for (int x = currentSquare.X + 1, y  = currentSquare.Y + 1; x <= 8 && y <= 8; x++, y++)
            {
                if (squares[x, y].pieceOnTop == null)
                    r.Add(squares[x, y]);
                else
                {
                    if (squares[x, y].pieceOnTop.isWhite != this.isWhite)
                        r.Add(squares[x, y]);

                    break;
                }
            }

            // Top Left
            for (int x = currentSquare.X - 1, y = currentSquare.Y + 1; x >= 1 && y <= 8; x--, y++)
            {
                if (squares[x, y].pieceOnTop == null)
                    r.Add(squares[x, y]);
                else
                {
                    if (squares[x, y].pieceOnTop.isWhite != this.isWhite)
                        r.Add(squares[x, y]);

                    break;
                }
            }

            // Bottom Right
            for (int x = currentSquare.X + 1, y = currentSquare.Y - 1; x <= 8 && y >= 1; x++, y--)
            {
                if (squares[x, y].pieceOnTop == null)
                    r.Add(squares[x, y]);
                else
                {
                    if (squares[x, y].pieceOnTop.isWhite != this.isWhite)
                        r.Add(squares[x, y]);

                    break;
                }
            }

            // Bottom Left
            for (int x = currentSquare.X - 1, y = currentSquare.Y - 1; x >= 1 && y >= 1; x--, y--)
            {
                if (squares[x, y].pieceOnTop == null)
                    r.Add(squares[x, y]);
                else
                {
                    if (squares[x, y].pieceOnTop.isWhite != this.isWhite)
                        r.Add(squares[x, y]);

                    break;
                }
            }
        }
        
        if (pieceType == PieceType.Queen)
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

            // Top Right
            for (int x = currentSquare.X + 1, y = currentSquare.Y + 1; x <= 8 && y <= 8; x++, y++)
            {
                if (squares[x, y].pieceOnTop == null)
                    r.Add(squares[x, y]);
                else
                {
                    if (squares[x, y].pieceOnTop.isWhite != this.isWhite)
                        r.Add(squares[x, y]);

                    break;
                }
            }

            // Top Left
            for (int x = currentSquare.X - 1, y = currentSquare.Y + 1; x >= 1 && y <= 8; x--, y++)
            {
                if (squares[x, y].pieceOnTop == null)
                    r.Add(squares[x, y]);
                else
                {
                    if (squares[x, y].pieceOnTop.isWhite != this.isWhite)
                        r.Add(squares[x, y]);

                    break;
                }
            }

            // Bottom Right
            for (int x = currentSquare.X + 1, y = currentSquare.Y - 1; x <= 8 && y >= 1; x++, y--)
            {
                if (squares[x, y].pieceOnTop == null)
                    r.Add(squares[x, y]);
                else
                {
                    if (squares[x, y].pieceOnTop.isWhite != this.isWhite)
                        r.Add(squares[x, y]);

                    break;
                }
            }

            // Bottom Left
            for (int x = currentSquare.X - 1, y = currentSquare.Y - 1; x >= 1 && y >= 1; x--, y--)
            {
                if (squares[x, y].pieceOnTop == null)
                    r.Add(squares[x, y]);
                else
                {
                    if (squares[x, y].pieceOnTop.isWhite != this.isWhite)
                        r.Add(squares[x, y]);

                    break;
                }
            }
        }
        
        if (pieceType == PieceType.King)
        {
            // Up 
            if (currentSquare.Y + 1 <= 8)
            {
                ChessboardSquare candidateSquare = squares[currentSquare.X, currentSquare.Y + 1];
                if (candidateSquare.pieceOnTop == null)
                    r.Add(candidateSquare);
                else if (candidateSquare.pieceOnTop.isWhite != this.isWhite)
                    r.Add(candidateSquare);
            }

            // Down
            if (currentSquare.Y - 1 >= 1)
            {
                ChessboardSquare candidateSquare = squares[currentSquare.X, currentSquare.Y - 1];
                if (candidateSquare.pieceOnTop == null)
                    r.Add(candidateSquare);
                else if (candidateSquare.pieceOnTop.isWhite != this.isWhite)
                    r.Add(candidateSquare);
            }

            // Right (and diagonals)
            if (currentSquare.X + 1 <= 8)
            {
                // Right
                ChessboardSquare candidateSquare = squares[currentSquare.X + 1, currentSquare.Y];
                if (candidateSquare.pieceOnTop == null)
                    r.Add(candidateSquare);
                else if (candidateSquare.pieceOnTop.isWhite != this.isWhite)
                    r.Add(candidateSquare);

                // Top Right
                if (currentSquare.Y + 1 <= 8)
                {
                    candidateSquare = squares[currentSquare.X + 1, currentSquare.Y + 1];
                    if (candidateSquare.pieceOnTop == null)
                        r.Add(candidateSquare);
                    else if (candidateSquare.pieceOnTop.isWhite != this.isWhite)
                        r.Add(candidateSquare);
                }

                // Bottom Right
                if (currentSquare.Y - 1 >= 1)
                {
                    candidateSquare = squares[currentSquare.X + 1, currentSquare.Y - 1];
                    if (candidateSquare.pieceOnTop == null)
                        r.Add(candidateSquare);
                    else if (candidateSquare.pieceOnTop.isWhite != this.isWhite)
                        r.Add(candidateSquare);
                }
            }

            // Left (and diagonals) 
            if (currentSquare.X - 1 >= 1)
            {
                // Left
                ChessboardSquare candidateSquare = squares[currentSquare.X - 1, currentSquare.Y];
                if (candidateSquare.pieceOnTop == null)
                    r.Add(candidateSquare);
                else if (candidateSquare.pieceOnTop.isWhite != this.isWhite)
                    r.Add(candidateSquare);

                // Top Left
                if (currentSquare.Y + 1 <= 8)
                {
                    candidateSquare = squares[currentSquare.X - 1, currentSquare.Y + 1];
                    if (candidateSquare.pieceOnTop == null)
                        r.Add(candidateSquare);
                    else if (candidateSquare.pieceOnTop.isWhite != this.isWhite)
                        r.Add(candidateSquare);
                }

                // Bottom Left
                if (currentSquare.Y - 1 >= 1)
                {
                    candidateSquare = squares[currentSquare.X - 1, currentSquare.Y - 1];
                    if (candidateSquare.pieceOnTop == null)
                        r.Add(candidateSquare);
                    else if (candidateSquare.pieceOnTop.isWhite != this.isWhite)
                        r.Add(candidateSquare);
                }
            }
        }
        
        return r;
    }
}
