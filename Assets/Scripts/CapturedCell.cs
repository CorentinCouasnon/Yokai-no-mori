using System.Linq;
using UnityEngine;

public class CapturedCell : Cell
{
    [field: SerializeField] public Player Owner { get; set; }
    [field: SerializeField] public int Limit { get; private set; }

    public bool IsOccupied(GameContext context)
    {
        return context.AllPieces.Where(pieceData => pieceData.Owner.Index == Owner.IndexPlayer).Count(piece => piece.IsCaptured) + 1 <= Limit;
    }
}