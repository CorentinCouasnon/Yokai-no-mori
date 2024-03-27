using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameContext
{
    public GameManager GameManager { get; }
    public List<PieceData> AllPieces { get; private set; }
    public PlayerData Player1 { get; private set; }
    public PlayerData Player2 { get; private set; }
    
    public List<PieceData> Player1Pieces => AllPieces.Where(piece => piece.Owner == Player1).ToList();
    public List<PieceData> Player2Pieces => AllPieces.Where(piece => piece.Owner == Player2).ToList();
    public List<PieceData> OwnPieces => AllPieces.Where(piece => piece.Owner == (IsFirstPlayerTurn ? Player1 : Player2)).ToList();
    public List<PieceData> OpponentPieces => AllPieces.Where(piece => piece.Owner != (IsFirstPlayerTurn ? Player1 : Player2)).ToList();

    public bool IsFirstPlayerTurn { get; set; } = true;
    public List<(PiecesType piece, Vector2 position)> Actions { get; set; } = new List<(PiecesType piece, Vector2 position)>();

    public GameContext(GameManager gameManager)
    {
        GameManager = gameManager;
    }
    
    public GameContext(GameManager gameManager, List<PieceData> allPieces, PlayerData player1, PlayerData player2)
    {
        GameManager = gameManager;
        AllPieces = allPieces;
        Player1 = player1;
        Player2 = player2;
    }

    public GameContext Clone()
    {
        var gameContext = new GameContext(GameManager)
        {
            Player1 = Player1,
            Player2 = Player2,
            AllPieces = new List<PieceData>(AllPieces),
            IsFirstPlayerTurn = IsFirstPlayerTurn,
            Actions = new List<(PiecesType piece, Vector2 position)>(Actions),
        };

        return gameContext;
    }

    public void Debug()
    {
        UnityEngine.Debug.Log("---");
        //UnityEngine.Debug.Log("IsFirstPlayerTurn = " + IsFirstPlayerTurn);
        //UnityEngine.Debug.Log("Player1.Index = " + Player1.Index);
        //UnityEngine.Debug.Log("Player2.Index = " + Player2.Index);
        UnityEngine.Debug.Log("Actions.ToDisplayString() = " + Actions.ToDisplayString());
        UnityEngine.Debug.Log("AllPieces.ToDisplayString() = " + AllPieces.ToDisplayString());
    }
}