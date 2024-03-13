using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameOverScore
{
    P1Win = 10,
    P2Win = -10,
    Tie = 0,
}
public class AIPlayer : Player
{
    readonly WaitForSeconds _waitOneSec = new WaitForSeconds(1);
    
    public override IEnumerator Play(GameContext context)
    {
        float bestScore = -Mathf.Infinity;
        var allMoves = new List<(Piece piece, (Vector2 position, Vector2 rotation) move)>();
        (Piece piece, (Vector2 position, Vector2 rotation) move) moveToPlay = (null, (Vector2.zero, Vector2.zero));
        foreach (var piece in context.OwnPieces)
        {
            foreach (var allowedMove in piece.GetAllowedMoves(context))
            {
                allMoves.Add((piece, allowedMove));
            }
            
            var randomMove = allMoves.GetRandom();
            Debug.Log(randomMove);
            randomMove.piece.Move(context, context.GameManager.GetCellFromPosition(randomMove.move.position), randomMove.move.rotation);
            float score = Minimax(context, 0, false);
            randomMove.piece.Move(context, context.GameManager.GetCellFromPosition(-randomMove.move.position), -randomMove.move.rotation);

            if (score > bestScore)
            {
                bestScore = score;
                moveToPlay = randomMove;
            }
            else
            {
                allMoves.Remove(randomMove);
            }
        }

        moveToPlay.piece.Move(context, context.GameManager.GetCellFromPosition(moveToPlay.move.position), moveToPlay.move.rotation);
        
        yield return _waitOneSec;
    }
    
    private int Minimax(GameContext context, int depth, bool isMaximizing)
    {
        if (context.GameManager.IsGameOver(context))
        {
            return (int) context.GameManager.CheckWinner(context);
        }

        float bestScore;

        //AI
        if (isMaximizing)
        {
            bestScore = -Mathf.Infinity;
            var allMoves = new List<(Piece piece, (Vector2 position, Vector2 rotation) move)>();
            foreach (var pieceP1 in context.Player1Pieces)
            {
                foreach (var allowedMove in pieceP1.GetAllowedMoves(context))
                {
                    allMoves.Add((pieceP1, allowedMove));
                }
                
                var randomMove = allMoves.GetRandom();
                Debug.LogError("RandomMove " + randomMove.move.position + " " + randomMove.move.rotation);
                randomMove.piece.Move(context, context.GameManager.GetCellFromPosition(randomMove.move.position), randomMove.move.rotation);
                float score = Minimax(context, depth + 1, false);
                Debug.LogError("RandomMove " + -randomMove.move.position + " " + -randomMove.move.rotation);
                randomMove.piece.Move(context, context.GameManager.GetCellFromPosition(-randomMove.move.position), -randomMove.move.rotation);
                allMoves.Remove(randomMove);
                bestScore = Mathf.Max(score, bestScore);
            }
        }
        //Player
        else
        {
            bestScore = -Mathf.Infinity;
            var allMoves = new List<(Piece piece, (Vector2 position, Vector2 rotation) move)>();
            
            foreach (var pieceP2 in context.Player2Pieces)
            {
                foreach (var allowedMove in pieceP2.GetAllowedMoves(context))
                {
                    allMoves.Add((pieceP2, allowedMove));
                }
                
                var randomMove = allMoves.GetRandom();
                Debug.LogError("RandomMove " + randomMove.move.position + " " + randomMove.move.rotation);
                randomMove.piece.Move(context, context.GameManager.GetCellFromPosition(randomMove.move.position), randomMove.move.rotation);
                float score = Minimax(context, depth + 1, true);
                Debug.LogError("RandomMove " + -randomMove.move.position + " " + -randomMove.move.rotation);
                randomMove.piece.Move(context, context.GameManager.GetCellFromPosition(-randomMove.move.position), -randomMove.move.rotation);
                allMoves.Remove(randomMove);
                bestScore = Mathf.Max(score, bestScore);
            }
        }

        return (int) bestScore;
    }
}