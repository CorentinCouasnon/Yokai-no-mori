using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
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
        yield return new WaitForSeconds(2f);
    }

    void OnCellClicked(Cell cell)
    {
        
    }
}