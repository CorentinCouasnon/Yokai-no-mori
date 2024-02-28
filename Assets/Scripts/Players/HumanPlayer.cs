using System.Collections;
using System.Collections.Generic;

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
        yield break;
    }

    void OnCellClicked(Cell cell)
    {
        
    }
}