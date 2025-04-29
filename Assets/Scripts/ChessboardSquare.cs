using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UIElements;

public enum SquareID
{
    A1, A2, A3, A4, A5, A6, A7, A8,
    B1, B2, B3, B4, B5, B6, B7, B8,
    C1, C2, C3, C4, C5, C6, C7, C8,
    D1, D2, D3, D4, D5, D6, D7, D8,
    E1, E2, E3, E4, E5, E6, E7, E8,
    F1, F2, F3, F4, F5, F6, F7, F8,
    G1, G2, G3, G4, G5, G6, G7, G8,
    H1, H2, H3, H4, H5, H6, H7, H8,
}

public class ChessboardSquare : MonoBehaviour
{
    [SerializeField] private ReferencesSO referencesSO;

    public SquareID squareID;
    public ChessPieceBase pieceOnTop;

    [SerializeField] private float piecePlaceOffset = 0.0055f;
    private Vector3 placePiecePos;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 worldPos = transform.position;
        placePiecePos = new Vector3(worldPos.x, worldPos.y + piecePlaceOffset, worldPos.z);

        InstantiateChessPiece();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Function for instantiating pieces at game start/reset, all pieces should be deleted before calling these
    public void InstantiateChessPiece()
    {
        GameObject pieceToInstantiate = null;

        if (squareID == SquareID.A1 || squareID == SquareID.H1)
        {
            pieceToInstantiate = referencesSO.whiteRookPrefab;
        }
        else if (squareID == SquareID.B1 || squareID == SquareID.G1)
        {
            pieceToInstantiate = referencesSO.whiteKnightPrefab;
        }
        else if (squareID == SquareID.C1 || squareID == SquareID.F1)
        {
            pieceToInstantiate = referencesSO.whiteBishopPrefab;
        }
        else if (squareID == SquareID.D1)
        {
            pieceToInstantiate = referencesSO.whiteQueenPrefab;
        }
        else if (squareID == SquareID.E1)
        {
            pieceToInstantiate = referencesSO.whiteKingPrefab;
        }
        else if (squareID == SquareID.A2 || squareID == SquareID.B2 || squareID == SquareID.C2 || squareID == SquareID.D2 ||
            squareID == SquareID.E2 || squareID == SquareID.F2 || squareID == SquareID.G2 || squareID == SquareID.H2)
        {
            pieceToInstantiate = referencesSO.whitePawnPrefab;
        }

        else if (squareID == SquareID.A8 || squareID == SquareID.H8)
        {
            pieceToInstantiate = referencesSO.blackRookPrefab;
        }
        else if (squareID == SquareID.B8 || squareID == SquareID.G8)
        {
            pieceToInstantiate = referencesSO.blackKnightPrefab;
        }
        else if (squareID == SquareID.C8 || squareID == SquareID.F8)
        {
            pieceToInstantiate = referencesSO.blackBishopPrefab;
        }
        else if (squareID == SquareID.D8)
        {
            pieceToInstantiate = referencesSO.blackQueenPrefab;
        }
        else if (squareID == SquareID.E8)
        {
            pieceToInstantiate = referencesSO.blackKingPrefab;
        }
        else if (squareID == SquareID.A7 || squareID == SquareID.B7 || squareID == SquareID.C7 || squareID == SquareID.D7 ||
            squareID == SquareID.E7 || squareID == SquareID.F7 || squareID == SquareID.G7 || squareID == SquareID.H7)
        {
            pieceToInstantiate = referencesSO.blackPawnPrefab;
        }

        if (pieceToInstantiate != null)
        {
            GameObject instantiatedPiece = Instantiate(pieceToInstantiate, placePiecePos, transform.localRotation);
            pieceOnTop = instantiatedPiece.GetComponent<ChessPieceBase>();
            pieceOnTop.currentSquare = this;
        }
    }

}
