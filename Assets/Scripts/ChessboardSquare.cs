using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UIElements;

public class ChessboardSquare : MonoBehaviour
{
    [SerializeField] private ReferencesSO referencesSO;

    public int X { get { return x; } private set { x = value; } }
    public int Y { get { return y; } private set { y = value; } }
    [SerializeField] private int x;
    [SerializeField] private int y;

    public ChessPieceBase pieceOnTop;

    [SerializeField] private float piecePlaceOffset = 0.0055f;
    private Vector3 placePiecePos;

    private void Awake()
    {
        Vector3 worldPos = transform.position;
        placePiecePos = new Vector3(worldPos.x, worldPos.y + piecePlaceOffset, worldPos.z);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Function for instantiating pieces at game start/reset, all pieces should be deleted before calling these
    public void InstantiateChessPiece()
    {
        GameObject pieceToInstantiate = null;

        if (X == 1 && (Y == 1 || Y == 8))
        {
            pieceToInstantiate = referencesSO.whiteRookPrefab;
        }
        else if (X == 1 && (Y == 2 || Y == 7))
        {
            pieceToInstantiate = referencesSO.whiteKnightPrefab;
        }
        else if (X == 1 && (Y == 3 || Y == 6))
        {
            pieceToInstantiate = referencesSO.whiteBishopPrefab;
        }
        else if (X == 1 && Y == 4)
        {
            pieceToInstantiate = referencesSO.whiteQueenPrefab;
        }
        else if (X == 1 && Y == 5)
        {
            pieceToInstantiate = referencesSO.whiteKingPrefab;
        }
        else if (X == 2)
        {
            pieceToInstantiate = referencesSO.whitePawnPrefab;
        }

        else if (X == 8 && (Y == 1 || Y == 8))
        {
            pieceToInstantiate = referencesSO.blackRookPrefab;
        }
        else if (X == 8 && (Y == 2 || Y == 7))
        {
            pieceToInstantiate = referencesSO.blackKnightPrefab;
        }
        else if (X == 8 && (Y == 3 || Y == 6))
        {
            pieceToInstantiate = referencesSO.blackBishopPrefab;
        }
        else if (X == 8 && Y == 4)
        {
            pieceToInstantiate = referencesSO.blackQueenPrefab;
        }
        else if (X == 8 && Y == 5)
        {
            pieceToInstantiate = referencesSO.blackKingPrefab;
        }
        else if (X == 7)
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
