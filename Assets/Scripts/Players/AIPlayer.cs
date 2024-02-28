using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIPlayer : Player
{
    readonly WaitForSeconds _waitOneSec = new WaitForSeconds(1);
    
    public override IEnumerator Play(GameContext context)
    {
        var allMoves = new List<(Piece piece, Vector2 position)>();

        foreach (var piece in context.OwnPieces)
        {
            foreach (var allowedMove in piece.GetAllowedMoves(context))
            {
                allMoves.Add((piece, allowedMove.Item1));
            }
        }

        var randomMove = allMoves.GetRandom();
        randomMove.piece.Move(context, randomMove.position, Vector2.zero);
        yield return _waitOneSec;
    }
}