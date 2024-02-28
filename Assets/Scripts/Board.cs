using UnityEngine;

public class Board : MonoBehaviour
{
    const int ROW_COUNT = 4;
    const int COLUMN_COUNT = 3;

    public static bool IsOutOfBounds(Vector2 position)
    {
        return position.x < 0 || position.x >= ROW_COUNT || position.y < 0 || position.y >= COLUMN_COUNT;
    }
}