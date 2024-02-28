using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [field: SerializeField] public PiecesType Type { get; set; }
    [field: SerializeField] public Vector2[] Directions { get; set; }
    [field: SerializeField] public Vector2 StartPosition { get; private set; }

    public Vector2 Position { get; set; }
    public bool IsCaptured { get; set; }

    public List<(Vector2, Vector2)> GetAllowedMoves(GameContext context)
    {
        var moves = new List<(Vector2, Vector2)>();

        Vector2 offsetRotation = Vector2.one;
        if (IsCaptured)
        {
            for (var i = 0; i < Board.COLUMN_COUNT; i++)
            {
                for (var j = 0; j < Board.ROW_COUNT; j++)
                {
                    var pos = new Vector2(j, i);

                    offsetRotation = GameManager.Rotate(pos, context.Player2Pieces.Contains(this) ? 180 : 0);
                    //Debug.Log("offsetRotation " + offsetRotation.normalized);
                    if (context.AllPieces.All(piece => piece.Position != pos))
                        moves.Add((pos, Vector2.zero));
                }
            }
        }
        else
        {
            foreach (var direction in Directions)
            {
                var newPosition = Position + direction;

                if (context.OwnPieces.Any(piece => piece.Position == newPosition))
                    continue;

                offsetRotation = GameManager.Rotate(newPosition, context.Player2Pieces.Contains(this)? 180 : 0);
                //Debug.Log("offsetRotation " + offsetRotation.normalized);
                if (!Board.IsOutOfBounds(newPosition))
                    moves.Add((newPosition,direction));
            }   
        }

        return moves;
    }

    public void ResetPiece()
    {
        Position = StartPosition;
    }
}