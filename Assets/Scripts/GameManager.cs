using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<Piece> _allPieces;
    [SerializeField] List<Cell> _cells;
    [SerializeField] List<CapturedCell> _capturedCells;
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
            
            if (IsGameOver(_gameContext))
                break;
            
            _gameContext.IsFirstPlayerTurn = !_gameContext.IsFirstPlayerTurn;

        }

        if (!IsThreeFoldRepetition(_gameContext))
            Debug.Log("Winner is " + (_gameContext.IsFirstPlayerTurn ? "Player 1" : "Player 2") + " !");
        else
            Debug.Log("Game ends in a draw !");
    }

    private void Start()
    {
        StartCoroutine(Play());
    }

    GameContext CreateContext()
    {
        return new GameContext(this, _allPieces, _player1, _player2);
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
            var isPlayer1Piece = koropokkuru.Owner == context.Player1;
            var promotingRow = isPlayer1Piece ? 3 : 0;
            var opponentPieces = context.AllPieces.Where(piece => piece.Owner == isPlayer1Piece ? context.Player2 : context.Player1);
            
            if (koropokkuru.IsCaptured || !Mathf.Approximately(koropokkuru.Position.y, promotingRow))
                return false;

            return opponentPieces
                .All(opponentPiece => 
                    !opponentPiece
                        .GetAllowedMoves(context)
                        .Select(move => move.Item1)
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

    public CapturedCell GetRemainingCapturedCell(Player player)
    {
        return _capturedCells.First(cell => cell.Owner == player);
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
    public List<Piece> AllPieces { get; }
    public Player Player1 { get; }
    public Player Player2 { get; }
    
    public List<Piece> Player1Pieces => AllPieces.Where(piece => piece.Owner == Player1).ToList();
    public List<Piece> Player2Pieces => AllPieces.Where(piece => piece.Owner == Player2).ToList();
    public List<Piece> OwnPieces => AllPieces.Where(piece => piece.Owner == (IsFirstPlayerTurn ? Player1 : Player2)).ToList();
    public List<Piece> OpponentPieces => AllPieces.Where(piece => piece.Owner != (IsFirstPlayerTurn ? Player1 : Player2)).ToList();

    public bool IsFirstPlayerTurn { get; set; } = true;
    public List<(Piece piece, Vector2 position)> Actions { get; set; } = new List<(Piece piece, Vector2 position)>();

    public GameContext(GameManager gameManager, List<Piece> allPieces, Player player1, Player player2)
    {
        GameManager = gameManager;
        AllPieces = allPieces;
        Player1 = player1;
        Player2 = player2;
    }
}