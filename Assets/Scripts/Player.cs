using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    [field: SerializeField] public List<Piece> Pieces { get; set; }

    public abstract IEnumerator Play(GameManager gameManager, List<Piece> opponentPieces);
}

public class HumanPlayer : Player
{
    void OnEnable()
    {
        Cell.CellClicked += OnCellClicked;
    }

    void OnDisable()
    {
        Cell.CellClicked -= OnCellClicked;
    }

    public override IEnumerator Play(GameManager gameManager, List<Piece> opponentPieces)
    {
        yield break;
    }

    void OnCellClicked(Cell cell)
    {
        
    }
}

public class AIPlayer : Player
{
    public override IEnumerator Play(GameManager gameManager, List<Piece> opponentPieces)
    {
        var piece = Pieces[Random.Range(0, Pieces.Count)];
        var allowedMoves = piece.GetAllowedMoves(Pieces);
        gameManager.MovePiece(piece, allowedMoves[Random.Range(0, allowedMoves.Count)]);
        yield break;
    }
}