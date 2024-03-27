using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public enum GameDifficulty
{
    Easy = 2,
    Medium = 4,
    Hard = 6,
}
public class GameManager : MonoBehaviour
{
    [Header("UI")] 
    [SerializeField] MainMenuUI _mainMenuUI;
    
    [Header("Game")]
    [SerializeField] List<Piece> _allPieces;
    [SerializeField] List<Cell> _cells;
    [SerializeField] List<CapturedCell> _capturedCells;
    [SerializeField] Player _player1;
    [SerializeField] Player _player2;

    [SerializeField] Piece _kodamaPrefab;
    [SerializeField] Piece _kodamaSamuraiPrefab;

    [Header("Game Setup")] 
    [SerializeField] Player _humanPlayer1; 
    [SerializeField] Player _humanPlayer2; 
    [SerializeField] Player _AIPlayer1; 
    [SerializeField] Player _AIPlayer2; 
    [SerializeField] List<CapturedCell> _capturedCellsPlayer1 = new List<CapturedCell>(); 
    [SerializeField] List<CapturedCell> _capturedCellsPlayer2 = new List<CapturedCell>(); 
    [SerializeField] List<Piece> _piecesPlayer1 = new List<Piece>(); 
    [SerializeField] List<Piece> _piecesPlayer2 = new List<Piece>(); 
    GameDifficulty _selectedDifficulty = GameDifficulty.Easy;

    GameContext _gameContext;
    
    public IEnumerator Play()
    {
        _gameContext = CreateContext();
        ResetAllPieces();
        ResetCapturedCells();
        _mainMenuUI.ChangePlayerTurnUI(_gameContext.IsFirstPlayerTurn ? _player1 : _player2);
        
        while (true)
        {
            if (_gameContext.IsFirstPlayerTurn)
                yield return StartCoroutine(_player1.Play(_gameContext));
            else
                yield return StartCoroutine(_player2.Play(_gameContext));

            if (IsGameOver(_gameContext))
                break;
            
            _gameContext.IsFirstPlayerTurn = !_gameContext.IsFirstPlayerTurn;
            _mainMenuUI.ChangePlayerTurnUI(_gameContext.IsFirstPlayerTurn ? _player1 : _player2);

        }

        // if (!IsThreeFoldRepetition(_gameContext))
        // {
        //     Debug.Log("Winner is " + (_gameContext.IsFirstPlayerTurn ? "Player 1" : "Player 2") + " !");
        //     _mainMenuUI.EndGame(_gameContext.IsFirstPlayerTurn ? _player1 : _player2);
        // }
        // else
        //     Debug.Log("Game ends in a draw !");
    }
    
    public void RestartGame()
    {
        StartCoroutine(Play());
    }

    public void SetGameDifficulty(int difficulty)
    {
        _selectedDifficulty = (GameDifficulty) difficulty;
    }

    public void SetupPlayerVSAI()
    {
        _player1 = _AIPlayer1;
        _player2 = _humanPlayer2;
        _AIPlayer1.gameObject.SetActive(true);
        _AIPlayer2.gameObject.SetActive(false);
        _humanPlayer1.gameObject.SetActive(false);
        _humanPlayer2.gameObject.SetActive(true);

        SetupPlayerPieces();
        RestartGame();
    }
    
    public void SetupPlayerVSPlayer()
    {
        _player1 = _humanPlayer1;
        _player2 = _humanPlayer2;
        _AIPlayer1.gameObject.SetActive(false);
        _AIPlayer2.gameObject.SetActive(false);
        _humanPlayer1.gameObject.SetActive(true);
        _humanPlayer2.gameObject.SetActive(true);

        SetupPlayerPieces();
        RestartGame();
    }

    void SetupPlayerPieces()
    {
        
        // Captured Cells
        foreach (var cell in _capturedCellsPlayer1)
        {
            cell.Owner = _player1;
        }

        foreach (var cell in _capturedCellsPlayer2)
        {
            cell.Owner = _player2;
        }

        // Pieces
        foreach (var piece in _piecesPlayer1)
        {
            piece.startingOwner = _player1;
        }
        
        foreach (var piece in _piecesPlayer2)
        {
            piece.startingOwner = _player2;
        }
    }
    GameContext CreateContext()
    {
        return new GameContext(this, _allPieces, _player1, _player2);
    }

    public GameDifficulty GetGameDifficulty()
    {
        return _selectedDifficulty;
    }

    public bool IsGameOver(GameContext context)
    {
        var anyKoropokkuruCaptured = AnyKoropokkuruCaptured(context);
        var anyKoropokkuruPromoted = AnyKoropokkuruPromotedAndSafe(context);
        var threeFoldRepetition = IsThreeFoldRepetition(context);

        if (anyKoropokkuruCaptured)
            Debug.Log("Win by capture.");
        if (anyKoropokkuruPromoted)
            Debug.Log("Win by promotion.");
        
        return anyKoropokkuruCaptured || anyKoropokkuruPromoted || threeFoldRepetition;
    }

    public GameOverScore CheckWinner(GameContext context)
    {
        GameOverScore gameOverScore = GameOverScore.Tie;
        
        if (!IsThreeFoldRepetition(context))
        {
            bool isPlayer2Win = context.Player1Pieces.Where(piece => piece.Type == PiecesType.Koropokkuru)
                .Any(piece => piece.IsCaptured);
            bool isPlayer1Win = context.Player2Pieces.Where(piece => piece.Type == PiecesType.Koropokkuru)
                .Any(piece => piece.IsCaptured);

            if (isPlayer2Win || isPlayer1Win)
            {
                gameOverScore = isPlayer1Win ? GameOverScore.P1Win : GameOverScore.P2Win;
            }

        }

        return gameOverScore;
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
            var promotingRow = isPlayer1Piece ? 0 : 3;
            var opponentPieces = context.AllPieces.Where(piece => piece.Owner == (isPlayer1Piece ? context.Player2 : context.Player1)).ToList();
            
            if (koropokkuru.IsCaptured || !Mathf.Approximately(koropokkuru.Position.y, promotingRow))
                return false;

            return !opponentPieces
                .Any(opponentPiece => opponentPiece
                    .GetAllowedMoves(context)
                    .Select(move => move.Item1)
                    .Contains(koropokkuru.Position));
        }
    }

    public void CheckKodamaPromoted(GameContext context)
    {
        List<Piece> kodamas = context.AllPieces.FindAll(piece => piece.Type == PiecesType.Kodama);

        foreach (var kodama in kodamas)
        {
            var isPlayer1Piece = kodama.Owner == context.Player1;
            var promotingRow = isPlayer1Piece ? 0 : 3;

            if (Mathf.Approximately(kodama.Position.y, promotingRow) && !kodama.IsParachuted)
            {
                // Transforme en samourai
                ChangePieceToArchetype(kodama, PiecesType.KodamaSamurai);
            }
        }
    }

    public void ChangePieceToArchetype(Piece piece, PiecesType piecesType)
    {
        switch (piecesType)
        {
            case PiecesType.Kodama : 
                piece.Type = _kodamaPrefab.Type;
                piece.Directions = _kodamaPrefab.Directions;
                piece.SpriteRenderer.sprite = _kodamaPrefab.SpriteRenderer.sprite;
                break;
            case PiecesType.KodamaSamurai :
                piece.Type = _kodamaSamuraiPrefab.Type;
                piece.Directions = _kodamaSamuraiPrefab.Directions;
                piece.SpriteRenderer.sprite = _kodamaSamuraiPrefab.SpriteRenderer.sprite;
                break;
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

    [ContextMenu("ResetAllPieces")]
    public void ResetAllPieces()
    {
        _gameContext.IsFirstPlayerTurn = true;
        foreach (var piece in _allPieces)
        {
            piece.ResetPiece(_gameContext);
        }
    }

    public void ResetCapturedCells()
    {
        foreach (var capturedCell in _capturedCells)
        {
            capturedCell.CapturedPiece = null;
        }
    }

    public Cell GetCellFromPosition(Vector2 pos)
    {
        return _cells.Find(cell => cell.Position == pos);
    }

    public CapturedCell GetRemainingCapturedCell(Player player)
    {
        return _capturedCells.First(cell => cell.Owner == player && cell.CapturedPiece == null);
    }

    public void RemovePieceFromCapturedCell(Piece piece)
    {
        CapturedCell capturedCell = _capturedCells.FirstOrDefault(cell => cell.CapturedPiece == piece);
        if (capturedCell != null)
        {
            capturedCell.CapturedPiece = null;
        }
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