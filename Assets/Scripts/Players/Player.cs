using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    [field: SerializeField] public List<Piece> Pieces { get; set; }

    public abstract IEnumerator Play(GameManager gameManager, List<Piece> opponentPieces);
}