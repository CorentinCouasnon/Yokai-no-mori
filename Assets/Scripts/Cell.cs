using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    [field: SerializeField] public Vector2 Position { get; private set; }

    public static Action<Cell> CellClicked { get; set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Cell " + Position);
        CellClicked?.Invoke(this);
    }
    
}