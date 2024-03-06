using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    [field: SerializeField] public Image Image { get; private set; }
    [field: SerializeField] public Vector2 Position { get; private set; }

    public static Action<Cell> CellClicked { get; set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        CellClicked?.Invoke(this);
    }

    public void DisplayColor(bool value)
    {
        Color newColor = Image.color;
        newColor.a = value ? 0.5f : 0;
        Image.color = newColor;
    }
}