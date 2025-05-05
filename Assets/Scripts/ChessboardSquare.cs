using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UIElements;

public enum HighlightColour
{
    None,
    Available,
    Hovering,
}

public class ChessboardSquare : MonoBehaviour
{
    // References
    [SerializeField] private ReferencesSO referencesSO;
    [SerializeField] private Chessboard chessboard;
    [SerializeField] private GameObject pieceToInstantiate;

    // Position
    public int X { get { return x; } private set { x = value; } }
    public int Y { get { return y; } private set { y = value; } }
    [SerializeField] private int x;
    [SerializeField] private int y;

    public ChessPieceBase pieceOnTop;

    // Piece placing
    [SerializeField] private float piecePlaceOffset = 0.0055f;
    private Vector3 placePiecePos;

    // Square Highlighting
    public HighlightColour currentHighlight;
    private MaterialPropertyBlock m_TintPropertyBlock;
    [SerializeField] private Color AvailableColour = Color.cyan;
    [SerializeField] private Color HoveringColour = Color.green;

    private void Awake()
    {
        Vector3 worldPos = transform.position;
        placePiecePos = new Vector3(worldPos.x, worldPos.y + piecePlaceOffset, worldPos.z);

        m_TintPropertyBlock = new MaterialPropertyBlock();
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

    public void SetSquareHighlight(HighlightColour highlightColour)
    {
        Color emissionColour;
        if (highlightColour == HighlightColour.None)
            emissionColour = Color.black;
        else if (highlightColour == HighlightColour.Available)
            emissionColour = AvailableColour * Mathf.LinearToGammaSpace(1f);
        else if (highlightColour == HighlightColour.Hovering)
            emissionColour = HoveringColour * Mathf.LinearToGammaSpace(1f);
        else
            emissionColour = Color.black;

        foreach (MeshRenderer render in GetComponents<MeshRenderer>())
        {
            if (render == null)
                continue;

            // Emissions don't really show on white materials, so we'll darken the material while it's highlighted
            if (render.material.color == Color.gray && highlightColour == HighlightColour.None)
                render.material.color = Color.white;
            else if (render.material.color == Color.white && highlightColour != HighlightColour.None)
                render.material.color = Color.gray;
            

            render.GetPropertyBlock(m_TintPropertyBlock);
            m_TintPropertyBlock.SetColor(Shader.PropertyToID("_EmissionColor"), emissionColour);
            render.SetPropertyBlock(m_TintPropertyBlock);
        }

        currentHighlight = highlightColour;
    }

}
