using System.Collections;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    [field: SerializeField] public int IndexPlayer { get; protected set; }
    
    public abstract IEnumerator Play(GameContext context);

    public PlayerData PlayerData { get; set; }

    public void ResetPlayer()
    {
        PlayerData = new PlayerData { Index = IndexPlayer };
    }
}