using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CapturedCell : Cell
{
    [field: SerializeField] public Piece CapturedPiece { get; set; }
    [field: SerializeField] public Player Owner { get; set; }
}
