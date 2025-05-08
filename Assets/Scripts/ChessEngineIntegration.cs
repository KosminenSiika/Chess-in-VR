using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ChessEngineIntegration : MonoBehaviour
{
    // References
    [SerializeField] private ChessGameManager gameManager;
    [SerializeField] private Chessboard chessboard;

    private Process chessEngineProcess;

    private int depth = 2;
    private string UCIMoveList = "position startpos";
    public string lastLine;

    public ChessboardSquare[] nextMove = null;

    public Dictionary<int, string> ColumnsToLetters = new Dictionary<int, string>()
    {
        { 1, "a" },
        { 2, "b" },
        { 3, "c" },
        { 4, "d" },
        { 5, "e" },
        { 6, "f" },
        { 7, "g" },
        { 8, "h" },
    };

    public Dictionary<string, int> ColumnsToInts = new Dictionary<string, int>()
    {
        { "a", 1 },
        { "b", 2 },
        { "c", 3 },
        { "d", 4 },
        { "e", 5 },
        { "f", 6 },
        { "g", 7 },
        { "h", 8 },
    };

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartChessEngine());
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (chessEngineProcess != null)
        {
            chessEngineProcess.StandardOutput.ReadLine();
            
            if (lastReadLine != null)
            {
                UnityEngine.Debug.Log("Last line was: " + lastReadLine);
                lastLine = lastReadLine;
            }   
        }
        */
    }

    private IEnumerator StartChessEngine()
    {
        chessEngineProcess = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Application.streamingAssetsPath + "/stockfish",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        chessEngineProcess.Start();
        chessEngineProcess.StandardInput.WriteLine("uci");

        UnityEngine.Debug.Log("stockfish started");

        yield return CheckIfReady();
    }

    public IEnumerator ResetGame()
    {
        yield return CheckIfReady();

        chessEngineProcess.StandardInput.WriteLine("ucinewgame");
    }

    public IEnumerator SetDifficulty()
    {
        yield return CheckIfReady();

        // Somewhat emulating lichess AI levels: https://lichess.org/forum/lichess-feedback/how-strong-are-the-stockfish-levels
        if (gameManager.difficulty == 1)
        {
            chessEngineProcess.StandardInput.WriteLine("setoption name Skill Level value 0");
            depth = 1;
        }
        else if (gameManager.difficulty == 2)
        {
            chessEngineProcess.StandardInput.WriteLine("setoption name Skill Level value 0");
            depth = 3;
        }
        else if (gameManager.difficulty == 3)
        {
            chessEngineProcess.StandardInput.WriteLine("setoption name Skill Level value 0");
            depth = 5;
        }
        else if (gameManager.difficulty == 4)
        {
            chessEngineProcess.StandardInput.WriteLine("setoption name Skill Level value 3");
            depth = 5;
        }
        else if (gameManager.difficulty == 5)
        {
            chessEngineProcess.StandardInput.WriteLine("setoption name Skill Level value 6");
            depth = 5;
        }
        else if (gameManager.difficulty == 6)
        {
            chessEngineProcess.StandardInput.WriteLine("setoption name Skill Level value 10");
            depth = 8;
        }
        else if (gameManager.difficulty == 7)
        {
            chessEngineProcess.StandardInput.WriteLine("setoption name Skill Level value 15");
            depth = 12;
        }
        else if (gameManager.difficulty == 8)
        {
            chessEngineProcess.StandardInput.WriteLine("setoption name Skill Level value 20");
            depth = 15;
        }
    }

    // Get next move from the engine
    public IEnumerator FetchNextMove(ChessboardSquare[] move)
    {
        yield return CheckIfReady();

        yield return ProvidePlayerMove(move);

        StartCoroutine(lastLineReader());
        yield return new WaitUntil(() => lastLine.StartsWith("bestmove"));

        string UCINextMove = lastLine.Substring(9, 4);

        UCIMoveList = UCIMoveList + " " + UCINextMove;

        if (UCINextMove != "0000")
            nextMove = UCINotationToChessboardSquares(UCINextMove);
        else
            nextMove = null;
        
    }
    public IEnumerator ProvidePlayerMove(ChessboardSquare[] move)
    {
        string UCIMove = ChessboardSquaresToUCINotation(move);

        UCIMoveList = UCIMoveList + " " + UCIMove;

        yield return CheckIfReady();

        // Write the position
        chessEngineProcess.StandardInput.Write(UCIMoveList);

        yield return CheckIfReady();

        // Start looking for move
        chessEngineProcess.StandardInput.Write("go depth " + depth.ToString());

    }

    // Helper to check if chessEngine is ready to receive input
    public IEnumerator CheckIfReady()
    {
        chessEngineProcess.StandardInput.WriteLine("isready");
        UnityEngine.Debug.Log("CheckIfReady called");
        StartCoroutine(lastLineReader());
        yield return new WaitUntil(() => lastLine == "readyok");
        UnityEngine.Debug.Log("readyok received");
    }
    public IEnumerator lastLineReader()
    {
        while (chessEngineProcess.StandardOutput.ReadLine() != null)
        {
            lastLine = chessEngineProcess.StandardOutput.ReadLine();
            UnityEngine.Debug.Log("Last line: " + lastLine);
        }

        yield return null;
    }

    // Move notation exchangers
    private string ChessboardSquaresToUCINotation(ChessboardSquare[] move)
    {
        ChessboardSquare startSquare = move[0];
        ChessboardSquare endSquare = move[1];

        string r = ColumnsToLetters[startSquare.X] + startSquare.Y.ToString() + ColumnsToLetters[endSquare.X] + endSquare.Y.ToString();

        if (endSquare.justQueened)
        {
            r = r + "q";
            endSquare.justQueened = false;
        }

        return r;
    }
    private ChessboardSquare[] UCINotationToChessboardSquares(string move)
    {
        ChessboardSquare startSquare = chessboard.squares[ColumnsToInts[move[0].ToString()], int.Parse(move[1].ToString())];
        ChessboardSquare endSquare = chessboard.squares[ColumnsToInts[move[2].ToString()], int.Parse(move[3].ToString())];

        return new ChessboardSquare[] { startSquare, endSquare };
    }

    private void SendLine(string command)
    {
        chessEngineProcess.StandardInput.WriteLine(command);
        chessEngineProcess.StandardInput.Flush();
    }
    private void OnApplicationQuit()
    {
        chessEngineProcess?.Kill();
    }
}
