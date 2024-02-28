using UnityEngine;

public class Board : MonoBehaviour
{
    public const int ROW_COUNT = 4;
    public const int COLUMN_COUNT = 3;

    public static bool IsOutOfBounds(Vector2 position)
    {
        return position.x < 0 || position.x >= COLUMN_COUNT || position.y < 0 || position.y >= ROW_COUNT;
    }
}