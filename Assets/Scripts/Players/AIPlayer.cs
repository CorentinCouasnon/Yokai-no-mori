using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
    readonly WaitForSeconds _waitOneSec = new WaitForSeconds(1);
    
    public override IEnumerator Play(GameManager gameManager, List<Piece> opponentPieces)
    {
        var allMoves = new List<(Piece piece, Vector2 position)>();

        foreach (var piece in Pieces)
        {
            foreach (var allowedMove in piece.GetAllowedMoves(Pieces))
            {
                allMoves.Add((piece, allowedMove));
            }
        }

        var randomMove = allMoves.GetRandom();
        gameManager.MovePiece(randomMove.piece, randomMove.position);
        yield return _waitOneSec;
    }
}