using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChessGameManager : MonoBehaviour
{
    [SerializeField] private ChessEngineIntegration chessEngineIntegration;
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
    public bool isGameOver = true;

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

        chessEngineIntegration.ResetGame();
        chessEngineIntegration.SetDifficulty();

        chessboard.moveList.Clear();

        if (isPlayerWhite)
        {
            chessboard.transform.eulerAngles = Vector3.zero;
            leftInteractor.interactionLayers = InteractionLayerMask.GetMask(new string[] { "WhitePiece", "ChessClock" });
            rightInteractor.interactionLayers = InteractionLayerMask.GetMask(new string[] { "WhitePiece", "ChessClock" });
        }
        else
        {
            chessboard.transform.eulerAngles = new Vector3(0, 180, 0);
            leftInteractor.interactionLayers = InteractionLayerMask.GetMask(new string[] { "BlackPiece", "ChessClock" });
            rightInteractor.interactionLayers = InteractionLayerMask.GetMask(new string[] { "BlackPiece", "ChessClock" });
            StartCoroutine(HandleEngineTurn());
        }

        EnablePlayerInteractionWithPieces(isPlayerWhite);

        chessboard.transform.position = chessboardInitialPosition;
        chessboard.InstantiatePieces();
    }

    public void WinGame(bool isWhiteTeam, bool isCheckmate)
    {
        isGameOver = true;
        isFirstMoveMade = false;

        // TEMPORARY WHILE CHESS ENGINE ISN'T IMPLEMENTED
        //leftInteractor.enabled = false;
        //rightInteractor.enabled = false;

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

    public void SwitchTurn()
    {
        if (isWhiteTurn)
        {
            isWhiteTurn = false;
            if (isPlayerWhite)
            {
                StartCoroutine(HandleEngineTurn());
            }
            else
            {
                EnablePlayerInteractionWithPieces(true);
            }
            
        }
        else
        {
            isWhiteTurn = true;
            if (isPlayerWhite)
            {
                EnablePlayerInteractionWithPieces(true);
            }
            else
            {
                StartCoroutine(HandleEngineTurn());
            }
        }

    }

    private IEnumerator HandleEngineTurn()
    {
        if (isFirstMoveMade)
            chessEngineIntegration.ProvidePlayerMove(chessboard.moveList[chessboard.moveList.Count - 1]);
        else
            chessEngineIntegration.SearchFirstMove();

        yield return new WaitUntil(() => chessEngineIntegration.nextMoveReady);
    
        ChessboardSquare[] nextMove = chessEngineIntegration.FetchNextMove();
        
        if (nextMove == null)
        {
            Debug.LogWarning("Engine's next move was a nullmove");
            yield break;
        }

        ChessboardSquare startSquare = nextMove[0];
        ChessboardSquare endSquare = nextMove[1];

        // Simulate engine thinking time
        float waitTime = Random.Range(2.0f, 8.0f);
        yield return new WaitForSeconds(waitTime);

        startSquare.pieceOnTop.SetEngineSpecialMove();
        startSquare.pieceOnTop.MoveTo(endSquare);

        startSquare.SetSquareHighlight(HighlightColour.EngineLastMove);
        endSquare.SetSquareHighlight(HighlightColour.EngineLastMove);

        SwitchTurn();
    }

    public void EnablePlayerInteractionWithPieces(bool shouldEnable)
    {
        if (shouldEnable)
            if (isPlayerWhite)
            {
                leftInteractor.interactionLayers = InteractionLayerMask.GetMask(new string[] { "WhitePiece", "ChessClock" });
                rightInteractor.interactionLayers = InteractionLayerMask.GetMask(new string[] { "WhitePiece", "ChessClock" });
            }
            else
            {
                leftInteractor.interactionLayers = InteractionLayerMask.GetMask(new string[] { "BlackPiece", "ChessClock" });
                rightInteractor.interactionLayers = InteractionLayerMask.GetMask(new string[] { "BlackPiece", "ChessClock" });
            }
        else
        {
            leftInteractor.interactionLayers = InteractionLayerMask.GetMask("ChessClock");
            rightInteractor.interactionLayers = InteractionLayerMask.GetMask("ChessClock");
        }
    }
}
