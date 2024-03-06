using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
    readonly WaitForSeconds _waitOneSec = new WaitForSeconds(1);
    
    public override IEnumerator Play(GameContext context)
    {
        var allMoves = new List<(Piece piece, (Vector2 position, Vector2 rotation) move)>();

        foreach (var piece in context.OwnPieces)
        {
            foreach (var allowedMove in piece.GetAllowedMoves(context))
            {
                allMoves.Add((piece, allowedMove));
            }
        }

        var randomMove = allMoves.GetRandom();
        randomMove.piece.Move(context, context.GameManager.GetCellFromPosition(randomMove.move.position), randomMove.move.rotation);
        yield return _waitOneSec;
    }
}