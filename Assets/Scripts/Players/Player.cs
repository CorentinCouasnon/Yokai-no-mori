using System.Collections;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    [field: SerializeField] public int IndexPlayer { get; private set; }
    
    public abstract IEnumerator Play(GameContext context);
}