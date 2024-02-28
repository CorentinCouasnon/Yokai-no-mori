using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerDownHandler
{
    [field: SerializeField] public Vector2 Position { get; private set; }

    public static Action<Cell> CellClicked { get; set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        CellClicked?.Invoke(this);
    }
}