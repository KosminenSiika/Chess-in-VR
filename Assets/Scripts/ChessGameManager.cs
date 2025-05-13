using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChessGameManager : MonoBehaviour
{
    [SerializeField] private Chessbot5000Settings gameSettings;
    [SerializeField] private ChessEngineIntegration chessEngineIntegration;

    [SerializeField] private ChessClock chessClock;
    [SerializeField] private Chessboard chessboard;
    private Vector3 chessboardInitialPosition;

    [SerializeField] private XRDirectInteractor leftInteractor;
    [SerializeField] private XRDirectInteractor rightInteractor;

    // Chessbot Feedback
    [SerializeField] private GameObject chessbotMoustache;
    [SerializeField] private MeshRenderer chessbotScreenRenderer;
    private bool isMoustacheMoving = false;
    private float moustacheMoveInterval = 0.35f;
    private float moustacheMoveTimer = 0;
    [SerializeField] private Color winScreenColour = Color.green;
    [SerializeField] private Color loseScreenColour = Color.red;
    private Color defaultScreenColour;
    [SerializeField] private AudioSource winAudioSource;
    [SerializeField] private AudioSource loseAudioSource;

    public float whiteMaxTime = 600;
    public float blackMaxTime = 600;

    public float whiteCurrentTime;
    public float blackCurrentTime;

    public bool isWhiteTurn = true;
    public bool isPlayerWhite = true;
    public bool isFirstMoveMade = false;
    public bool isGameOver = true;
    public bool isPieceMovedThisTurn = false;

    public int difficulty = 0;

    // Start is called before the first frame update
    void Start()
    {
        EnablePlayerInteractionWithPieces(false);
        chessboardInitialPosition = chessboard.transform.position;
        defaultScreenColour = chessbotScreenRenderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
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

            if (whiteCurrentTime < 0)
            {
                WinGame(false, false);
            }
            if (blackCurrentTime < 0)
            {
                WinGame(true, false);
            }
        }

        if (isMoustacheMoving || chessbotMoustache.transform.localPosition.y == 1)
        {
            if (moustacheMoveTimer < moustacheMoveInterval)
                moustacheMoveTimer += Time.deltaTime;
            else
            {
                MoveChessbotMoustache();
                moustacheMoveTimer = 0;
            }
        } 
    }

    public void ResetGame()
    {
        StopAllCoroutines();

        isGameOver = false;
        isMoustacheMoving = false;
        chessEngineIntegration.nextMoveReady = false;
        isWhiteTurn = true;
        isFirstMoveMade = false;
        isPieceMovedThisTurn = false;
        isPlayerWhite = gameSettings.playerWhiteToggle.isOn;
        difficulty = (int)gameSettings.difficultySlider.value;
        whiteMaxTime =  (int)(gameSettings.whiteHourSlider.value * 3600 + gameSettings.whiteMinuteSlider.value * 60);
        blackMaxTime =  (int)(gameSettings.blackHourSlider.value * 3600 + gameSettings.blackMinuteSlider.value * 60);
        whiteCurrentTime = whiteMaxTime;
        blackCurrentTime = blackMaxTime;

        chessEngineIntegration.ResetGame();
        chessEngineIntegration.SetDifficulty();

        chessboard.ClearAllHighlights();
        chessboard.moveList.Clear();

        chessbotScreenRenderer.material.color = defaultScreenColour;

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
        chessboard.InstantiateNewPieces();
    }

    public void WinGame(bool isWhiteTeam, bool isCheckmate)
    {
        isGameOver = true;
        isFirstMoveMade = false;

        EnablePlayerInteractionWithPieces(false);
        chessClock.EnableChessClockInteractable(false);

        if (isCheckmate)
            Debug.Log("Checkmate!");

        if (isWhiteTeam == isPlayerWhite)
        {
            winAudioSource.Play();
            chessbotScreenRenderer.material.color = winScreenColour;
        }
        else
        {
            loseAudioSource.Play();
            chessbotScreenRenderer.material.color = loseScreenColour;
        }
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

        isPieceMovedThisTurn = false;
    }

    private IEnumerator HandleEngineTurn()
    {
        isMoustacheMoving = true;

        if (isFirstMoveMade)
            chessEngineIntegration.ProvidePlayerMove(chessboard.moveList[chessboard.moveList.Count - 1]);
        else
            chessEngineIntegration.SearchFirstMove();

        yield return new WaitUntil(() => chessEngineIntegration.nextMoveReady);
        chessEngineIntegration.nextMoveReady = false;
        
        ChessboardSquare[] nextMove = chessEngineIntegration.FetchNextMove();
        
        if (nextMove == null)
        {
            Debug.LogWarning("Engine's next move was a nullmove");
            yield break;
        }

        ChessboardSquare startSquare = nextMove[0];
        ChessboardSquare endSquare = nextMove[1];

        // Simulate engine thinking time
        float waitTime = Random.Range(1.0f, 7.0f);
        yield return new WaitForSeconds(waitTime);

        isMoustacheMoving = false;

        yield return new WaitForSeconds(1.0f);

        if (!isGameOver) // If Engine runs out of time, it doesn't move
        {   
            startSquare.pieceOnTop.SetEngineSpecialMove();
            startSquare.pieceOnTop.MoveTo(endSquare);

            startSquare.SetSquareHighlight(HighlightColour.EngineLastMove);
            endSquare.SetSquareHighlight(HighlightColour.EngineLastMove);

            SwitchTurn();
        }
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

    private void MoveChessbotMoustache()
    {
        if (chessbotMoustache.transform.localPosition.y == 0)
            RaiseChessbotMoustache();
        else if (chessbotMoustache.transform.localPosition.y == 1)
            LowerChessbotMoustache();
    }
    private void RaiseChessbotMoustache() { chessbotMoustache.transform.localPosition += new Vector3(0, 1, 0); }
    private void LowerChessbotMoustache() { chessbotMoustache.transform.localPosition -= new Vector3(0, 1, 0); }
}
