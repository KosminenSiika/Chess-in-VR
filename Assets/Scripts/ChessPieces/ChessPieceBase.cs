using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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

public enum SpecialMove
{
    None,
    EnPassant,
    Castling,
    Queening,
}

public class ChessPieceBase : MonoBehaviour
{
    [SerializeField] private GameObject model;

    public Chessboard chessboard;
    public ChessboardSquare currentSquare;

    // TEMPORARY WHILE CHESS ENGINE ISN'T IMPLEMENTED
    private ChessGameManager gameManager;

    public PieceType pieceType;
    public bool isWhite;
    private bool hasMoved = false;
    private SpecialMove specialMove = SpecialMove.None;

    private List<ChessboardSquare> availableMoves = new List<ChessboardSquare>();

    // Start is called before the first frame update
    void Start()
    {
        model.transform.Rotate(0, chessboard.transform.eulerAngles.y, 0);
        gameManager = FindFirstObjectByType<ChessGameManager>();
    }

    // Interaction with player's interactors
    public void OnPickedUp()
    {
        availableMoves.Clear();
        availableMoves = GetAvailableMoves(pieceType, ref chessboard.squares);
        specialMove = GetSpecialMoves(pieceType, ref chessboard.squares, ref chessboard.moveList);
        PreventCheck(ref chessboard.squares);

        // Highlight availableMoves squares
        foreach (ChessboardSquare square in availableMoves)
            square.SetSquareHighlight(HighlightColour.Available); 
    }
    public void OnDropped()
    {
        if (gameManager.isGameOver)
        {
            currentSquare.PlaceChessPiece(this);
        }
        else
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

            if (targetSquare != currentSquare)
            {
                MoveTo(targetSquare);
            }
            else
            {
                // Place piece back onto currentSquare
                currentSquare.PlaceChessPiece(this);
            }
        }

        // Clear highlights
        foreach (ChessboardSquare square in availableMoves)
            square.SetSquareHighlight(HighlightColour.None);
    }

    // ChessEngine helpers
    public void SetEngineSpecialMove()
    {
        specialMove = GetSpecialMoves(pieceType, ref chessboard.squares, ref chessboard.moveList);
    }

    // Get moves
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

    // Special moves
    private SpecialMove GetSpecialMoves(PieceType pieceType, ref ChessboardSquare[,] squares, ref List<ChessboardSquare[]> moveList)
    {
        SpecialMove r = SpecialMove.None;

        if (pieceType == PieceType.Pawn)
        {
            int direction = isWhite ? 1 : -1;

            // Queening
            if ((isWhite && currentSquare.Y == 7) || (!isWhite && currentSquare.Y == 2))
                return SpecialMove.Queening;

            // En Passant
            if (moveList.Count > 0)
            {
                ChessboardSquare[] lastMove = moveList[moveList.Count - 1];
                if (squares[lastMove[1].X, lastMove[1].Y].pieceOnTop.pieceType == PieceType.Pawn) // If the last piece moved was a pawn
                {
                    if (Mathf.Abs(lastMove[0].Y - lastMove[1].Y) == 2) // If the last move was a double forward
                    {
                        if (lastMove[1].Y == currentSquare.Y) // If pawns are on the same rank
                        {
                            if (lastMove[1].X == currentSquare.X - 1) // Landed to the left
                            {
                                availableMoves.Add(squares[currentSquare.X - 1, currentSquare.Y + direction]);
                                return SpecialMove.EnPassant;
                            }
                            if (lastMove[1].X == currentSquare.X + 1) // Landed to the right
                            {
                                availableMoves.Add(squares[currentSquare.X + 1, currentSquare.Y + direction]);
                                return SpecialMove.EnPassant;
                            }
                        }
                    }
                }
            }
        }

        if (pieceType == PieceType.King)
        {
            // Castling
            if (!hasMoved) // King hasn't moved
                if (isWhite) // White team
                {
                    if (squares[1, 1].pieceOnTop != null) // There is a piece on A1
                        if (!squares[1, 1].pieceOnTop.hasMoved) // That piece hasn't moved -> is a Rook
                            if (squares[2, 1].pieceOnTop == null)           //
                                if (squares[3, 1].pieceOnTop == null)       // The squares in between are empty
                                    if (squares[4, 1].pieceOnTop == null)   //
                                    {
                                        availableMoves.Add(squares[3, 1]);
                                        r = SpecialMove.Castling;
                                    }

                    if (squares[8, 1].pieceOnTop != null) // There is a piece on H1
                        if (!squares[8, 1].pieceOnTop.hasMoved) // That piece hasn't moved -> is a Rook
                            if (squares[7, 1].pieceOnTop == null)       // 
                                if (squares[6, 1].pieceOnTop == null)   // The squares in between are empty
                                {
                                    availableMoves.Add(squares[7, 1]);
                                    r = SpecialMove.Castling;
                                }
                }
                else // Black team
                {
                    if (squares[1, 8].pieceOnTop != null) // There is a piece on A8
                        if (!squares[1, 8].pieceOnTop.hasMoved) // That piece hasn't moved -> is a Rook
                            if (squares[2, 8].pieceOnTop == null)           //
                                if (squares[3, 8].pieceOnTop == null)       // The squares in between are empty
                                    if (squares[4, 8].pieceOnTop == null)   //
                                    {
                                        availableMoves.Add(squares[3, 8]);
                                        r = SpecialMove.Castling;
                                    }

                    if (squares[8, 8].pieceOnTop != null) // There is a piece on H8
                        if (!squares[8, 8].pieceOnTop.hasMoved) // That piece hasn't moved -> is a Rook
                            if (squares[7, 8].pieceOnTop == null)       // 
                                if (squares[6, 8].pieceOnTop == null)   // The squares in between are empty
                                {
                                    availableMoves.Add(squares[7, 8]);
                                    r = SpecialMove.Castling;
                                }
                }
        }

        return r;
    }
    private void ProcessSpecialMove()
    {
        if (specialMove == SpecialMove.Queening)
            if (currentSquare.Y == 8 || currentSquare.Y == 1)
                currentSquare.HandleQueening(); // Outsource the Queening because this gameObject is going to get destroyed :-)

        if (specialMove == SpecialMove.EnPassant)
        {
            ChessboardSquare targetPawnPosition = chessboard.moveList[chessboard.moveList.Count - 2][1];
            if (currentSquare.X ==  targetPawnPosition.X)
                targetPawnPosition.EliminatePieceOnTop();
        }

        if (specialMove == SpecialMove.Castling)
        {
            ChessboardSquare[] lastMove = chessboard.moveList[chessboard.moveList.Count - 1];

            // Left Rook
            if (lastMove[1].X == 3)
            {
                if (isWhite) // White team
                {
                    ChessboardSquare targetSquare = chessboard.squares[4, 1];
                    ChessPieceBase rook = chessboard.squares[1, 1].pieceOnTop;
                    targetSquare.PlaceChessPiece(rook);
                    rook.currentSquare.EmptySquare();
                    rook.currentSquare = targetSquare;
                }
                else // Black team
                {
                    ChessboardSquare targetSquare = chessboard.squares[4, 8];
                    ChessPieceBase rook = chessboard.squares[1, 8].pieceOnTop;
                    targetSquare.PlaceChessPiece(rook);
                    rook.currentSquare.EmptySquare();
                    rook.currentSquare = targetSquare;
                }
            }

            // Right Rook
            else if (lastMove[1].X == 7)
            {
                if (isWhite) // White team
                {
                    ChessboardSquare targetSquare = chessboard.squares[6, 1];
                    ChessPieceBase rook = chessboard.squares[8, 1].pieceOnTop;
                    targetSquare.PlaceChessPiece(rook);
                    rook.currentSquare.EmptySquare();
                    rook.currentSquare = targetSquare;
                }
                else // Black team
                {
                    ChessboardSquare targetSquare = chessboard.squares[6, 8];
                    ChessPieceBase rook = chessboard.squares[8, 8].pieceOnTop;
                    targetSquare.PlaceChessPiece(rook);
                    rook.currentSquare.EmptySquare();
                    rook.currentSquare = targetSquare;
                }
            }
        }
    }
    
    // Preventing checks
    private void PreventCheck(ref ChessboardSquare[,] squares)
    {
        // Find our king piece
        ChessPieceBase ourKing = null;
        foreach (ChessboardSquare square in squares)
            if (square != null) 
                if (square.pieceOnTop != null)
                    if (square.pieceOnTop.pieceType == PieceType.King)
                        if (square.pieceOnTop.isWhite == this.isWhite)
                            ourKing = square.pieceOnTop;

        SimulateMovesForCheck(ourKing, ref availableMoves, ref squares);
    }
    public void SimulateMovesForCheck(ChessPieceBase ourKing, ref List<ChessboardSquare> availableMoves, ref ChessboardSquare[,] squares)
    {
        // Save actual current square, to reset after simulation
        ChessboardSquare actualSquare = currentSquare;
        List<ChessboardSquare> movesToRemove = new List<ChessboardSquare>();

        // Simulate all moves and check if king is in check
        foreach (ChessboardSquare simSquare in availableMoves)
        {
            ChessboardSquare kingSquareThisSim = ourKing.currentSquare;
            
            // Did we simulate the king's move
            if (this.pieceType == PieceType.King)
                kingSquareThisSim = simSquare;

            // Get all attacking pieces
            List<ChessPieceBase> simAttackingPieces = new List<ChessPieceBase>();
            foreach (ChessboardSquare square in squares)
                if (square != null)
                    if (square.pieceOnTop != null)
                        if (square.pieceOnTop.isWhite != this.isWhite)
                            simAttackingPieces.Add(square.pieceOnTop);

            // If simulated move eliminates an enemy piece, remove it from attacking list and also save it for restoring
            ChessPieceBase eliminatedPiece = null;
            if (simSquare.pieceOnTop != null)
            {
                eliminatedPiece = simSquare.pieceOnTop;
                if (simAttackingPieces.Contains(eliminatedPiece))
                    simAttackingPieces.Remove(eliminatedPiece);
            }

            // Simulate that move
            actualSquare.pieceOnTop = null;
            simSquare.pieceOnTop = this;

            // Get all simulated attacking pieces' moves
            List<ChessboardSquare> simAttackingMoves = new List<ChessboardSquare>();
            foreach(ChessPieceBase attackingPiece in simAttackingPieces)
            {
                List<ChessboardSquare> pieceMoves = attackingPiece.GetAvailableMoves(attackingPiece.pieceType, ref squares);
                foreach(ChessboardSquare pieceMove in pieceMoves)
                    if (!simAttackingMoves.Contains(pieceMove))
                            simAttackingMoves.Add(pieceMove);
            }

            // Does the king's square appear in the attacking pieces' moves? If so, add that square to movesToRemove
            if (simAttackingMoves.Contains(kingSquareThisSim))
                movesToRemove.Add(simSquare);

            // Restore board
            actualSquare.pieceOnTop = this;
            if (eliminatedPiece != null)
                simSquare.pieceOnTop = eliminatedPiece;
            else 
                simSquare.pieceOnTop = null;
        }

        // Remove from the availableMoves list
        foreach (ChessboardSquare square in movesToRemove)
        {
            availableMoves.Remove(square);    
        }

        // Also remove castling middle square if applicable
        if (specialMove == SpecialMove.Castling)
        {
            // White team 
            // Right
            if (movesToRemove.Contains(squares[6, 1]))
                if (availableMoves.Contains(squares[7, 1]))
                    availableMoves.Remove(squares[7, 1]);

            // Left
            if (movesToRemove.Contains(squares[4, 1]))
                if (availableMoves.Contains(squares[3, 1]))
                    availableMoves.Remove(squares[3, 1]);

            // Black team
            // Right
            if (movesToRemove.Contains(squares[6, 8]))
                if (availableMoves.Contains(squares[7, 8]))
                    availableMoves.Remove(squares[7, 8]);

            // Left
            if (movesToRemove.Contains(squares[4, 8]))
                if (availableMoves.Contains(squares[3, 8]))
                    availableMoves.Remove(squares[3, 8]);
        }
    }

    // Checkmate
    private bool CheckForCheckmate()
    {
        List<ChessPieceBase> attackingPieces = new List<ChessPieceBase>();
        List<ChessPieceBase> defendingPieces = new List<ChessPieceBase>();
        ChessPieceBase targetKing = null;

        // Populate lists and target king
        foreach (ChessboardSquare square in chessboard.squares)
            if (square != null)
                if (square.pieceOnTop != null)
                    if (square.pieceOnTop.isWhite != this.isWhite)
                    {
                        defendingPieces.Add(square.pieceOnTop);
                        if (square.pieceOnTop.pieceType == PieceType.King)
                            targetKing = square.pieceOnTop;
                    }
                    else
                        attackingPieces.Add(square.pieceOnTop);

        // Get all the attacking moves
        List<ChessboardSquare> attackingMoves = new List<ChessboardSquare>();
        foreach (ChessPieceBase attackingPiece in attackingPieces)
        {
            List<ChessboardSquare> pieceMoves = attackingPiece.GetAvailableMoves(attackingPiece.pieceType, ref chessboard.squares);
            foreach (ChessboardSquare pieceMove in pieceMoves)
                if (!attackingMoves.Contains(pieceMove))
                    attackingMoves.Add(pieceMove);
        }

        // Is the king in check? (in the attacking moves list)
        if (attackingMoves.Contains(targetKing.currentSquare))
        {
            // TODO: Add red tint to king
            Debug.Log("Check!");

            // Can the king be defended?
            foreach (ChessPieceBase defendingPiece in defendingPieces)
            {
                // Get available moves
                List<ChessboardSquare> defendingMoves = defendingPiece.GetAvailableMoves(defendingPiece.pieceType, ref chessboard.squares);
                // Remove moves that result in check
                defendingPiece.SimulateMovesForCheck(targetKing, ref defendingMoves, ref chessboard.squares);

                // If there is any defending move that can be played -> no checkmate
                if (defendingMoves.Count > 0)
                    return false;
            }
            return true;
        }
        return false;
    }

    // Move piece on the board
    public void MoveTo(ChessboardSquare targetSquare)
    {
        targetSquare.PlaceChessPiece(this);

        chessboard.moveList.Add(new ChessboardSquare[] { currentSquare, targetSquare });

        currentSquare.EmptySquare();
        currentSquare = targetSquare;

        if (!hasMoved)
            hasMoved = true;

        ProcessSpecialMove();

        if (!gameManager.isFirstMoveMade)
            gameManager.isFirstMoveMade = true;

        if (CheckForCheckmate())
            gameManager.WinGame(isWhite, true);
        else
        {
            // Function call to prevent further interaction with pieces -> press chess clock to end turn
            gameManager.SwitchTurn();
        }
    }
}
