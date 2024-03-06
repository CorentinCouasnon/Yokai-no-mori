using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HumanPlayer : Player
{
    bool _canPlay = false;
    bool _turnIsEnded = false;
    
    List<(Cell,Vector2)> _selectableCells = new List<(Cell,Vector2)>();
    Piece _selectedPiece;
    
    GameContext _gameContext;
    

    void OnEnable()
    {
        Cell.CellClicked += OnCellClicked;
    }

    void OnDisable()
    {
        Cell.CellClicked -= OnCellClicked;
    }

    public override IEnumerator Play(GameContext context)
    {
        Debug.Log("Play " + gameObject.name);
        _canPlay = true;
        _turnIsEnded = false;
        _selectableCells.Clear();
        _selectedPiece = null;
        _gameContext = context;
        yield return new WaitUntil(()=> _turnIsEnded);
        _canPlay = false;
        Debug.Log("End Turn " + gameObject.name);
    }

    void OnCellClicked(Cell cell)
    {
        if (_canPlay)
        {
            Piece piece = _gameContext.OwnPieces.Find(piece => piece.Position == cell.Position);

            if (!_selectedPiece)
            {
                if (piece != null)
                {
                    List<(Vector2, Vector2)> moveList = piece.GetAllowedMoves(_gameContext);

                    foreach (var pos in moveList)
                    {
                        Cell newCell = _gameContext.GameManager.GetCellFromPosition(pos.Item1);
                        if (newCell != null)
                        {
                            _selectableCells.Add((newCell, pos.Item2));
                            newCell.DisplayColor(true);
                        }
                    }
                    if (_selectableCells.Count > 0)
                    {
                        _selectedPiece = piece;
                    }
                }
            }
            else
            {
                bool hasMoved = false;
                foreach (var tuple in _selectableCells)
                {
                    tuple.Item1.DisplayColor(false);
                    
                    if (_selectedPiece != null && tuple.Item1 == cell)
                    {
                        _selectedPiece.Move(_gameContext, cell,tuple.Item2);
                        hasMoved = true;
                    }
                }
                
                if (piece != null)
                {
                    _selectedPiece = null;
                    _selectableCells.Clear();
                    OnCellClicked(cell);
                }
                else
                {
                    if (hasMoved)
                    {
                        _turnIsEnded = true;
                    }
                }
            }
        }
    }
}