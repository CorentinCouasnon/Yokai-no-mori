using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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