public struct PlayerData
{
    public int Index { get; set; }

    public override string ToString()
    {
        return $"{nameof(Index)}: {Index}";
    }

    public bool Equals(PlayerData other)
    {
        return Index == other.Index;
    }

    public override bool Equals(object obj)
    {
        return obj is PlayerData other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Index;
    }

    public static bool operator ==(PlayerData left, PlayerData right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PlayerData left, PlayerData right)
    {
        return !left.Equals(right);
    }
}