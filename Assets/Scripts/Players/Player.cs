using System.Collections;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public abstract IEnumerator Play(GameContext context);
}