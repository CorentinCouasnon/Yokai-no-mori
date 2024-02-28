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
    
    public void Move(GameContext context, Vector2 newPosition)
    {
        // Enregistrement de l'action
        context.Actions.Add((this, newPosition));
        
        // Capture
        var capturedPiece = context.AllPieces.FirstOrDefault(piece => piece.Position == newPosition);
        if (capturedPiece != null)
        {
            capturedPiece.IsCaptured = true;
            return;
        }

        // Parachutage
        if (IsCaptured)
            IsCaptured = false;
        
        // Déplacement
        Position = newPosition;
    }

    public List<Vector2> GetAllowedMoves(GameContext context)
    {
        var moves = new List<Vector2>();

        if (IsCaptured)
        {
            for (var i = 0; i < Board.COLUMN_COUNT; i++)
            {
                for (var j = 0; j < Board.ROW_COUNT; j++)
                {
                    var pos = new Vector2(j, i);
                    
                    if (context.AllPieces.All(piece => piece.Position != pos))
                        moves.Add(pos);
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
            
                if (!Board.IsOutOfBounds(newPosition))
                    moves.Add(newPosition);
            }   
        }

        return moves;
    }

    public void ResetPiece()
    {
        Position = StartPosition;
        IsCaptured = false;
    }
}