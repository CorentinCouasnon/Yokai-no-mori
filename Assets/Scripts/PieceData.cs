using UnityEngine;

public struct PieceData
{
    public Piece Piece { get; set; }
    public PlayerData Owner { get; set; }
    public PiecesType Type { get; set; }
    public Vector2[] Directions { get; set; }
    public Vector2 Position { get; set; }
    public bool IsParachuted { get; set; }
    public bool IsCaptured { get; set; }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return (Piece != null ? Piece.GetHashCode() : 0);
    }

    public bool Equals(PieceData other)
    {
        return Equals(Piece, other.Piece);
    }

    public static bool operator ==(PieceData left, PieceData right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PieceData left, PieceData right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return
            $"{nameof(Piece)}: {Piece}, " +
            $"{nameof(Owner)}: {Owner}, " +
            $"{nameof(Type)}: {Type}, " +
            //$"{nameof(Directions)}: {Directions}, " +
            $"{nameof(Position)}: {Position}, " +
            $"{nameof(IsParachuted)}: {IsParachuted}, " +
            $"{nameof(IsCaptured)}: {IsCaptured}";
    }
}