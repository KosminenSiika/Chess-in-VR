using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UIElements;

public class ChessboardSquare : MonoBehaviour
{
    [SerializeField] private ReferencesSO referencesSO;
    [SerializeField] private Chessboard chessboard;
    [SerializeField] private GameObject pieceToInstantiate;

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
        if (pieceToInstantiate != null)
        {
            GameObject instantiatedPiece = Instantiate(pieceToInstantiate, placePiecePos, transform.localRotation);
            pieceOnTop = instantiatedPiece.GetComponent<ChessPieceBase>();
            pieceOnTop.currentSquare = this;
            pieceOnTop.chessboard = chessboard;
        }
    }

    public void PlaceChessPiece(ChessPieceBase piece)
    {
        // Kill enemy piece if present
        if (pieceOnTop != null && pieceOnTop != piece)
        {
            Destroy(pieceOnTop.gameObject);
        }

        // Place piece onto square
        pieceOnTop = piece;
        piece.transform.position = placePiecePos;
        piece.transform.rotation = transform.localRotation;
    }

    public void EmptySquare()
    {
        pieceOnTop = null;
    }

}
