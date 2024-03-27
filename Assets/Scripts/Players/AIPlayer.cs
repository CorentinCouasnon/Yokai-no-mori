using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameOverScore
{
    P1Win = 1000,
    P2Win = -1000,
    Tie = 0,
}

public class AIPlayer : Player
{
    readonly WaitForSeconds _waitOneSec = new WaitForSeconds(1);
    
    readonly Dictionary<PiecesType, (int onBoard, int inHand)> PiecesValue = new Dictionary<PiecesType, (int onBoard, int inHand)>()
    {
        { PiecesType.Kodama, (2, 1) },
        { PiecesType.Kitsune, (6, 3) },
        { PiecesType.Tanuki, (6, 3) },
        { PiecesType.KodamaSamurai, (12, 0) },
        { PiecesType.Koropokkuru, (0, 0) },
    };
    
    public override IEnumerator Play(GameContext context)
    {
        yield return _waitOneSec;
        
        float bestScore = -Mathf.Infinity;
        (Piece piece, (Vector2 position, Vector2 rotation) move) moveToPlay = (null, (Vector2.zero, Vector2.zero));
        
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
                    moveToPlay = (piece, allowedMove);
                }
            }
        }

        Debug.LogError(moveToPlay.piece.name + " MoveToPlay " + moveToPlay.move.position + " " + moveToPlay.move.rotation);
        moveToPlay.piece.Move(context, context.GameManager.GetCellFromPosition(moveToPlay.move.position), moveToPlay.move.rotation);
    }
    
    private int Minimax(GameContext context, int depth, bool isMaximizing)
    {
        if (depth > 2)
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

        return Evaluate(context);
    }
    
    int Evaluate(GameContext context)
    {
        if (context.GameManager.IsGameOver(context))
            return (int) context.GameManager.CheckWinner(context);
        
        return GetTotalPieceValue(context.Player1Pieces) - GetTotalPieceValue(context.Player2Pieces);
    }

    int GetTotalPieceValue(List<Piece> pieces)
    {
        var total = 0;

        foreach (var piece in pieces)
        {
            total += piece.IsCaptured ? PiecesValue[piece.Type].inHand : PiecesValue[piece.Type].onBoard;
        }
        
        return total;
    }
}