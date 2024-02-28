using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public abstract IEnumerator Play(GameContext context);
}