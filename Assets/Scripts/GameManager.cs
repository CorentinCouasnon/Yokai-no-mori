using System;
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
    [SerializeField] List<Cell> _cells;
    [SerializeField] Player _player1;
    [SerializeField] Player _player2;

    private GameContext _gameContext;
    
    public IEnumerator Play()
    {
        ResetAllPieces();
        _gameContext = CreateContext();
        
        while (true)
        {
            Debug.Log(_gameContext.IsFirstPlayerTurn);
            if (_gameContext.IsFirstPlayerTurn)
                yield return StartCoroutine(_player1.Play(_gameContext));
            else
                yield return StartCoroutine(_player2.Play(_gameContext));

            if (IsGameOver())
                break;
            
            _gameContext.IsFirstPlayerTurn = !_gameContext.IsFirstPlayerTurn;

        }

        Debug.Log("Winner is " + (_gameContext.IsFirstPlayerTurn ? "Player 1" : "Player 2") + " !");
    }

    public void MovePiece(Piece piece, Vector2 newPosition, Vector2 direction)
    {
        // Déplacement
        Debug.Log("Direction Piece " + direction.x + (_gameContext.Player2Pieces.Contains(piece) ? direction.y : -direction.y));
        piece.transform.position += new Vector3(direction.x, _gameContext.Player2Pieces.Contains(piece) ? direction.x : -direction.x); 
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

    private void Start()
    {
        StartCoroutine(Play());
    }

    [ContextMenu("Play Game")]
    private void PlayManager()
    {
        StartCoroutine(Play());
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

    public Cell GetCellFromPosition(Vector2 pos)
    {
        return _cells.Find(cell => cell.Position == pos);
    }
    
    public static Vector2 Rotate(Vector2 v, float delta) {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        ) * Mathf.Rad2Deg;
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