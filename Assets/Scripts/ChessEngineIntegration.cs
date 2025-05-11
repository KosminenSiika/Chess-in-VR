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

    private int depth;
    public string UCIMoveList = "position startpos moves";
    public string lastLine;

    public bool nextMoveReady = false;

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
        StartChessEngine();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void StartChessEngine()
    {
        chessEngineProcess = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Application.streamingAssetsPath + "/fairy-stockfish",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        chessEngineProcess.OutputDataReceived += new DataReceivedEventHandler(ChessEngineProcess_OutputDataReceived);
        chessEngineProcess.Start();
        chessEngineProcess.BeginOutputReadLine();

        SendLine("uci");
        SendLine("isready");
    }

    public void ResetGame()
    {
        SendLine("ucinewgame");
        UCIMoveList = "position startpos moves";
    }

    public void SetDifficulty()
    {
        // Emulating lichess AI levels: https://lichess.org/forum/lichess-feedback/how-strong-are-the-stockfish-levels
        if (gameManager.difficulty == 1)
        {
            SendLine("setoption name Skill Level value -9");
            depth = 5;
        }
        else if (gameManager.difficulty == 2)
        {
            SendLine("setoption name Skill Level value -5");
            depth = 5;
        }
        else if (gameManager.difficulty == 3)
        {
            SendLine("setoption name Skill Level value -1");
            depth = 5;
        }
        else if (gameManager.difficulty == 4)
        {
            SendLine("setoption name Skill Level value 3");
            depth = 5;
        }
        else if (gameManager.difficulty == 5)
        {
            SendLine("setoption name Skill Level value 7");
            depth = 5;
        }
        else if (gameManager.difficulty == 6)
        {
            SendLine("setoption name Skill Level value 11");
            depth = 8;
        }
        else if (gameManager.difficulty == 7)
        {
            SendLine("setoption name Skill Level value 15");
            depth = 13;
        }
        else if (gameManager.difficulty == 8)
        {
            SendLine("setoption name Skill Level value 20");
            depth = 22;
        }
    }

    // Get next move from the engine
    public ChessboardSquare[] FetchNextMove()
    {
        string UCINextMove = lastLine.Substring(9, 4);

        UCIMoveList = UCIMoveList + " " + UCINextMove;

        if (UCINextMove != "0000")
            return UCINotationToChessboardSquares(UCINextMove);
        else
            return null;
        
    }
    public void ProvidePlayerMove(ChessboardSquare[] move)
    {
        string UCIMove = ChessboardSquaresToUCINotation(move);

        // Add player move to engine's movelist
        UCIMoveList = UCIMoveList + " " + UCIMove;

        // Tell the position
        SendLine(UCIMoveList);
        // Start looking for move
        SendLine("go depth " + depth.ToString());
    }

    public void SearchFirstMove()
    {
        // Tell the position
        SendLine(UCIMoveList);
        // Start looking for move
        SendLine("go depth " + depth.ToString());
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
    private void ChessEngineProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        lastLine = e.Data;

        if (lastLine.StartsWith("bestmove"))
            nextMoveReady = true;
        else 
            nextMoveReady = false;
    }
    private void OnApplicationQuit()
    {
        chessEngineProcess?.Kill();
    }
}
