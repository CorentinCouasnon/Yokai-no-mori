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
                yield return StartCoroutine(_player2.Play(context));
            
            if (IsGameOver(context))
                break;
            
            _gameContext.IsFirstPlayerTurn = !_gameContext.IsFirstPlayerTurn;

        }

        if (!IsThreeFoldRepetition(context))
            Debug.Log("Winner is " + (context.IsFirstPlayerTurn ? "Player 1" : "Player 2") + " !");
        else
            Debug.Log("Game ends in a draw !");
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

    bool IsGameOver(GameContext context)
    {
        var anyKoropokkuruCaptured = AnyKoropokkuruCaptured(context);
        var anyKoropokkuruPromoted = AnyKoropokkuruPromotedAndSafe(context);
        var threeFoldRepetition = IsThreeFoldRepetition(context);

        return anyKoropokkuruCaptured || anyKoropokkuruPromoted || threeFoldRepetition;
    }

    bool AnyKoropokkuruCaptured(GameContext context)
    {
        return _allPieces.Where(piece => piece.Type == PiecesType.Koropokkuru).Any(piece => piece.IsCaptured);
    }

    bool AnyKoropokkuruPromotedAndSafe(GameContext context)
    {
        return context.AllPieces
            .Where(piece => piece.Type == PiecesType.Koropokkuru)
            .Any(IsKoropokkuruPromotedAndSafe);

        bool IsKoropokkuruPromotedAndSafe(Piece koropokkuru)
        {
            var isPlayer1Piece = context.Player1Pieces.Contains(koropokkuru);
            var promotingRow = isPlayer1Piece ? 3 : 0;
            var opponentPieces = isPlayer1Piece ? context.Player2Pieces : context.Player1Pieces;
            
            if (koropokkuru.IsCaptured || !Mathf.Approximately(koropokkuru.Position.y, promotingRow))
                return false;

            return opponentPieces
                .All(opponentPiece => 
                    !opponentPiece
                        .GetAllowedMoves(context)
                        .Contains(koropokkuru.Position)
                    );
        }
    }

    bool IsThreeFoldRepetition(GameContext context)
    {
        /*
         * e4  ^8
         * e5  ^7
         * Nf3 ^6
         * Nf6 ^5
         * Ng1 ^4
         * Ng8 ^3
         * Nf3 ^2
         * Nf6 ^1
         * => Draw
         */
        
        if (context.Actions.Count < 6)
            return false;

        return context.Actions[^1] == context.Actions[^5] && context.Actions[^2] == context.Actions[^6];
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
    
    public IReadOnlyList<Piece> OwnPieces => IsFirstPlayerTurn ? Player1Pieces : Player2Pieces;
    public IReadOnlyList<Piece> OpponentPieces => IsFirstPlayerTurn ? Player2Pieces : Player1Pieces;
    
    public bool IsFirstPlayerTurn { get; set; } = true;
    public List<(Piece piece, Vector2 position)> Actions { get; set; } = new List<(Piece piece, Vector2 position)>();

    public GameContext(GameManager gameManager, IReadOnlyList<Piece> allPieces, IReadOnlyList<Piece> player1Pieces, IReadOnlyList<Piece> player2Pieces)
    {
        GameManager = gameManager;
        AllPieces = allPieces;
        Player1Pieces = player1Pieces;
        Player2Pieces = player2Pieces;
    }
}