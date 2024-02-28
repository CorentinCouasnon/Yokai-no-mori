using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Board _board;
    [SerializeField] List<Piece> _allPieces;
    [SerializeField] List<Piece> _player1Pieces;
    [SerializeField] List<Piece> _player2Pieces;
    [SerializeField] Player _player1;
    [SerializeField] Player _player2;
    
    public IEnumerator Play()
    {
        ResetAllPieces();
        var context = CreateContext();
        
        while (true)
        {
            if (context.IsFirstPlayerTurn)
                yield return StartCoroutine(_player1.Play(context));
            else
                yield return StartCoroutine(_player2.Play(context));

            if (IsGameOver())
                break;
            
            context.IsFirstPlayerTurn = !context.IsFirstPlayerTurn;
        }

        Debug.Log("Winner is " + (context.IsFirstPlayerTurn ? "Player 1" : "Player 2") + " !");
    }

    public void MovePiece(Piece piece, Vector2 newPosition)
    {
        // Déplacement
        piece.Position = newPosition;

        // Capture
        var capturedPiece = _allPieces.FirstOrDefault(piece => piece.Position == newPosition);
        if (capturedPiece != null)
        {
            capturedPiece.IsCaptured = true;
            return;
        }

        // Parachutage
        if (piece.IsCaptured)
            piece.IsCaptured = false;
    }

    GameContext CreateContext()
    {
        return new GameContext(this, _allPieces, _player1Pieces, _player2Pieces);
    }

    bool IsGameOver()
    {
        return _allPieces.Where(piece => piece.Type == PiecesType.Koropokkuru).Any(piece => piece.IsCaptured);
    }

    void ResetAllPieces()
    {
        foreach (var piece in _allPieces)
        {
            piece.ResetPiece();
        }
    }
}

public class GameContext
{
    public GameManager GameManager { get; }
    public IReadOnlyList<Piece> AllPieces { get; }
    public IReadOnlyList<Piece> Player1Pieces { get; }
    public IReadOnlyList<Piece> Player2Pieces { get; }
    public bool IsFirstPlayerTurn { get; set; } = true;
    
    public IReadOnlyList<Piece> OwnPieces => IsFirstPlayerTurn ? Player1Pieces : Player2Pieces;
    public IReadOnlyList<Piece> OpponentPieces => IsFirstPlayerTurn ? Player2Pieces : Player1Pieces;
    
    public GameContext(GameManager gameManager, IReadOnlyList<Piece> allPieces, IReadOnlyList<Piece> player1Pieces, IReadOnlyList<Piece> player2Pieces)
    {
        GameManager = gameManager;
        AllPieces = allPieces;
        Player1Pieces = player1Pieces;
        Player2Pieces = player2Pieces;
    }
}