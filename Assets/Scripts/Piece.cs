using System;
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

    public List<Vector2> GetAllowedMoves(List<Piece> ownPieces)
    {
        var moves = new List<Vector2>();

        foreach (var direction in Directions)
        {
            var newPosition = Position + direction;
            
            if (ownPieces.Any(piece => piece.Position == newPosition))
                continue;
            
            if (!Board.IsOutOfBounds(newPosition))
                moves.Add(newPosition);
        }
        
        return moves;
    }

    public void ResetPiece()
    {
        Position = StartPosition;
    }
}