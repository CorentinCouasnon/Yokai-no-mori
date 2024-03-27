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
        (Piece piece, (Vector2 position, Vector2 rotation) move) moveToPlay = (null, (Vector2.zero, Vector2.zero));
        //
        // // String tabs [x][y]
        // string[][] boardTab = new string[Board.COLUMN_COUNT][];
        // for (int index = 0; index < boardTab.Length; index++)
        // {
        //     boardTab[index] = new string[Board.ROW_COUNT];
        // }
        //
        // // String Captured tabs [2][3]
        // string[][] capturedTab = new string[2][];
        // for (int index = 0; index < capturedTab.Length; index++)
        // {
        //     capturedTab[index] = new string[3];
        // }
        //
        // foreach (var piece in context.AllPieces)
        // {
        //     boardTab[(int)piece.Position.x][(int)piece.Position.y] = piece.GetFENName(context);
        // }
        
        foreach (var piece in context.OwnPieces)
        {
            foreach (var allowedMove in piece.GetAllowedMoves(context))
            {
                Debug.LogError(piece.name + " " + allowedMove);
                piece.MoveAI(context, context.GameManager.GetCellFromPosition(allowedMove.position), allowedMove.rotation);
                Debug.LogError("Piece : " + piece.Type + " " + piece.Position);
                float score = Minimax(context, 0, false);
                piece.MoveAI(context, context.GameManager.GetCellFromPosition(allowedMove.position-allowedMove.rotation), -allowedMove.rotation, true);
                Debug.LogError("Undo Piece : " + piece.Type + " " + piece.Position);
                if (score > bestScore)
                {
                    bestScore = score;
                    moveToPlay = (piece,allowedMove);
                }
            }
        }

        Debug.LogError(moveToPlay.piece.name + " MoveToPlay " + moveToPlay.move.position + " " + moveToPlay.move.rotation);
        moveToPlay.piece.Move(context, context.GameManager.GetCellFromPosition(moveToPlay.move.position), moveToPlay.move.rotation);
        
        yield return _waitOneSec;
    }
    
    private int Minimax(GameContext context, int depth, bool isMaximizing)
    {
        if (depth > 1)
        {
            return 0;
        }
        
        if (context.GameManager.IsGameOver(context))
        {
            return (int) context.GameManager.CheckWinner(context);
        }

        float bestScore;

        //AI
        if (isMaximizing)
        {
            bestScore = -Mathf.Infinity;
            foreach (var pieceP1 in context.Player1Pieces)
            {
                foreach (var allowedMove in pieceP1.GetAllowedMoves(context))
                {
                    pieceP1.MoveAI(context, context.GameManager.GetCellFromPosition(allowedMove.position), allowedMove.rotation);
                    float score = Minimax(context, depth + 1, false);
                    pieceP1.MoveAI(context, context.GameManager.GetCellFromPosition(allowedMove.position-allowedMove.rotation), -allowedMove.rotation, true);
                    bestScore = Mathf.Max(score, bestScore);
                }
            }
        }
        //Player
        else
        {
            bestScore = Mathf.Infinity;
            foreach (var pieceP2 in context.Player2Pieces)
            {
                foreach (var allowedMove in pieceP2.GetAllowedMoves(context))
                {
                    pieceP2.MoveAI(context, context.GameManager.GetCellFromPosition(allowedMove.position), allowedMove.rotation);
                    float score = Minimax(context, depth + 1, true);
                    pieceP2.MoveAI(context, context.GameManager.GetCellFromPosition(allowedMove.position-allowedMove.rotation), -allowedMove.rotation, true);
                    bestScore = Mathf.Min(score, bestScore);
                }
            }
        }

        return (int) Random.Range(0,100);
    }
}