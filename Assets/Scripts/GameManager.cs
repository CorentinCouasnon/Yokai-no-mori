using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Board _board;
    [SerializeField] List<Cell> _cells;
    [SerializeField] List<Piece> _pieces;
    [SerializeField] Player _player1;
    [SerializeField] Player _player2;

    bool _isFirstPlayerTurn = true;
    
    public IEnumerator Play()
    {
        ResetAllPieces();
        
        while (true)
        {
            if (_isFirstPlayerTurn)
                yield return StartCoroutine(_player1.Play(this, _player2.Pieces));
            else
                yield return StartCoroutine(_player2.Play(this, _player1.Pieces));

            if (IsGameOver())
                break;
            
            _isFirstPlayerTurn = !_isFirstPlayerTurn;
        }

        Debug.Log("Winner is " + (_isFirstPlayerTurn ? "Player 1" : "Player 2") + " !");
    }

    void ResetAllPieces()
    {
        foreach (var piece in _pieces)
        {
            piece.ResetPiece();
        }
    }

    public Cell GetCellFromPosition(Vector2 pos)
    {
        return _cells.Find(cell => cell.Position == pos);
    }

    bool IsGameOver()
    {
        return _pieces.Where(piece => piece.Type == PiecesType.Koropokkuru).Any(piece => piece.IsCaptured);
    }

    public void MovePiece(Piece piece, Vector2 newPosition)
    {
        // Déplacement
        piece.Position = newPosition;

        // Capture
        var capturedPiece = _pieces.FirstOrDefault(piece => piece.Position == newPosition);
        if (capturedPiece != null)
        {
            capturedPiece.IsCaptured = true;
            return;
        }

        // Parachutage
        if (piece.IsCaptured)
            piece.IsCaptured = false;
    }

    [ContextMenu("Play Game")]
    private void PlayManager()
    {
        StartCoroutine(Play());
    }
}