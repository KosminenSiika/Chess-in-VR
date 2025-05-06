using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChessGameManager : MonoBehaviour
{
    [SerializeField] private Chessboard chessboard;
    private Vector3 chessboardInitialPosition;
    [SerializeField] private XRDirectInteractor leftInteractor;
    [SerializeField] private XRDirectInteractor rightInteractor;


    public float whiteMaxTime = 600;
    public float blackMaxTime = 600;

    public float whiteCurrentTime;
    public float blackCurrentTime;

    public bool isWhiteTurn = true;
    public bool isPlayerWhite = true;
    public bool isFirstMoveMade = false;
    public bool isGameOver = false;

    public int difficulty = 0;

    private bool firstGameReset = false;

    // Start is called before the first frame update
    void Start()
    {
        whiteCurrentTime = whiteMaxTime;
        blackCurrentTime = blackMaxTime;
        EnablePlayerInteractionWithPieces(false);
        chessboardInitialPosition = chessboard.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!firstGameReset && Time.timeSinceLevelLoad >= 3)
        {
            firstGameReset = true;
            ResetGame();
        }

        if (isFirstMoveMade)
        {
            if (isWhiteTurn)
            {
                whiteCurrentTime -= Time.deltaTime;
            }
            else
            {
                blackCurrentTime -= Time.deltaTime;
            }

            if (whiteCurrentTime <= 0)
            {
                WinGame(false, false);
            }
            if (blackCurrentTime <= 0)
            {
                WinGame(true, false);
            }
        }
    }

    public void ResetGame()
    {
        isGameOver = false;
        isWhiteTurn = true;
        isFirstMoveMade = false;
        // isPlayerWhite = UI element toggle value
        // difficulty = UI element slider value
        // whiteMaxTime = UI element slider value
        // blackMaxTime = UI element slider value
        whiteCurrentTime = whiteMaxTime;
        blackCurrentTime = blackMaxTime;

        if (isPlayerWhite)
        {
            chessboard.transform.eulerAngles = Vector3.zero;
            leftInteractor.interactionLayers = InteractionLayerMask.GetMask("WhitePiece");
            rightInteractor.interactionLayers = InteractionLayerMask.GetMask("WhitePiece");
        }
        else
        {
            chessboard.transform.eulerAngles = new Vector3(0, 180, 0);
            leftInteractor.interactionLayers = InteractionLayerMask.GetMask("BlackPiece");
            rightInteractor.interactionLayers = InteractionLayerMask.GetMask("BlackPiece");
        }

        EnablePlayerInteractionWithPieces(isPlayerWhite);

        chessboard.transform.position = chessboardInitialPosition;
        chessboard.InstantiatePieces();
    }

    public void WinGame(bool isWhiteTeam, bool isCheckmate)
    {
        isGameOver = true;
        isFirstMoveMade = false;

        // Disable chess engine

        leftInteractor.enabled = false;
        rightInteractor.enabled = false;

        EnablePlayerInteractionWithPieces(false);

        if (isCheckmate)
            Debug.Log("Checkmate!");

        if (isWhiteTeam)
        {
            // Display white team win
            Debug.Log("White team wins!");
        }
        else
        {
            // Display black team win
            Debug.Log("Black team wins!");
        }

        Debug.Log("White team time left: " + whiteCurrentTime);
        Debug.Log("Black team time left: " + blackCurrentTime);
    }

    public void SwitchTurn(bool isPrevTurnWhite)
    {
        if (isPrevTurnWhite)
        {
            isWhiteTurn = false;
            EnablePlayerInteractionWithPieces(!isPlayerWhite);
        }
        else
        {
            isWhiteTurn = true;
            EnablePlayerInteractionWithPieces(isPlayerWhite);
        }

    }

    private void EnablePlayerInteractionWithPieces(bool shouldEnable)
    {
        /*
        leftInteractor.enabled = shouldEnable;
        rightInteractor.enabled = shouldEnable;
        */

        // TEMPORARY WHILE CHESS ENGINE ISN'T IMPLEMENTED
        if (shouldEnable)
        {
            leftInteractor.interactionLayers = InteractionLayerMask.GetMask("WhitePiece");
            rightInteractor.interactionLayers = InteractionLayerMask.GetMask("WhitePiece");
        }
        else
        {
            leftInteractor.interactionLayers = InteractionLayerMask.GetMask("BlackPiece");
            rightInteractor.interactionLayers = InteractionLayerMask.GetMask("BlackPiece");
        }

    }
}
