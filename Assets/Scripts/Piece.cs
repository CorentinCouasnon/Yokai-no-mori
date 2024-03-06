using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] Vector2 _startPosition;
    [SerializeField] public Player _startingOwner;
    
    [field: SerializeField] public PiecesType Type { get; set; }
    [field: SerializeField] public Vector2[] Directions { get; set; }
    
    public Player Owner { get; set; }
    public Vector2 Position { get; set; }
    public bool IsCaptured { get; set; }
    
    public void Move(GameContext context, Vector2 newPosition, Vector2 direction)
    {
        // Enregistrement de l'action
        context.Actions.Add((this, newPosition));
        
        // Capture
        var capturedPiece = context.AllPieces.FirstOrDefault(piece => piece.Position == newPosition);
        if (capturedPiece != null)
        {
            capturedPiece.Owner = context.IsFirstPlayerTurn ? context.Player1 : context.Player2;
            capturedPiece.IsCaptured = true;
            return;
        }

        // Parachutage
        if (IsCaptured)
            IsCaptured = false;
        
        // Déplacement
        Position = newPosition;
        transform.position += new Vector3(direction.x, -direction.y);
    }

    public List<(Vector2 position, Vector2 rotation)> GetAllowedMoves(GameContext context)
    {
        var moves = new List<(Vector2, Vector2)>();
        
        if (IsCaptured)
        {
            for (var i = 0; i < Board.COLUMN_COUNT; i++)
            {
                for (var j = 0; j < Board.ROW_COUNT; j++)
                {
                    var pos = new Vector2(j, i);
                    
                    if (context.AllPieces.All(piece => piece.Position != pos))
                        moves.Add((pos, Vector2.zero));
                }
            }
        }
        else
        {
            foreach (var direction in Directions)
            {
                Vector2 newDirection = (Owner == context.Player1 ? direction : -direction);
                var newPosition = Position + newDirection;
                
                if (context.OwnPieces.Any(piece => piece.Position == newPosition))
                    continue;

                if (!Board.IsOutOfBounds(newPosition))
                    moves.Add((newPosition ,newDirection));
            }   
        }

        return moves;
    }

    public void ResetPiece()
    {
        Position = _startPosition;
        Owner = _startingOwner;
        Debug.LogError(gameObject.name + " Owner = " + Owner);
        IsCaptured = false;
    }
}