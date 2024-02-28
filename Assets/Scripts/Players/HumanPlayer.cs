using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    bool _canPlay = false;
    GameManager _gameManager;
    
    void OnEnable()
    {
        Cell.CellClicked += OnCellClicked;
    }

    void OnDisable()
    {
        Cell.CellClicked -= OnCellClicked;
    }

    public override IEnumerator Play(GameManager gameManager, List<Piece> opponentPieces)
    {
        _canPlay = true;
        _gameManager = gameManager;
        yield return new WaitForSeconds(20f);
        _canPlay = false;
        Debug.Log("End Turn " + gameObject.name);
    }

    void OnCellClicked(Cell cell)
    {
        if (_canPlay)
        {
            Piece piece = Pieces.Find(piece => piece.Position == cell.Position);
            Debug.Log(piece);
            if (piece != null)
            {
                Debug.Log("Clicked on own piece");
                List<Vector2> moveList = piece.GetAllowedMoves(Pieces);
                foreach (var move in moveList)
                {
                    Debug.Log("Can move to " + move);
                }
                List<Cell> selectableCells = new List<Cell>();
                    
                foreach (var pos in moveList)
                {
                    Cell newCell = _gameManager.GetCellFromPosition(pos);
                    if (newCell != null)
                    {
                        selectableCells.Add(newCell);
                        newCell.DisplayColor(true);
                    }
                }
            }
        }
    }
}