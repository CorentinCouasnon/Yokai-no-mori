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

    public override IEnumerator Play(GameContext context)
    {
        yield return new WaitForSeconds(2f);
    }

    void OnCellClicked(Cell cell)
    {
        
    }
}