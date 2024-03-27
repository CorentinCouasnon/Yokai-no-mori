using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] Vector2 _startPosition;
    [SerializeField] public Player _startingOwner;
    
    [field: SerializeField] public PiecesType Type { get; set; }
    [field: SerializeField] public Vector2[] Directions { get; set; }
    [field: SerializeField] public SpriteRenderer SpriteRenderer { get; set; }
    
    public Player Owner { get; set; }
    [field: SerializeField] public Vector2 Position { get; set; }
    public bool IsCaptured { get; set; }
    public bool IsParachuted { get; set; }

    private Vector2 _lastPositionBeforeCaptured;
    private Piece _lastCapturedPiece;

    public void Move(GameContext context, Cell cell, Vector2 direction)
    {
        // Enregistrement de l'action
        Vector2 newPosition = cell.Position;
        context.Actions.Add((this, newPosition));
        
        // Capture
        var capturedPiece = context.AllPieces.FirstOrDefault(piece => piece.Position == newPosition);
        Debug.LogError(newPosition + " " +capturedPiece);
        if (capturedPiece != null)
        {
            capturedPiece.Owner = context.IsFirstPlayerTurn ? context.Player1 : context.Player2;
            capturedPiece.IsCaptured = true;
            if (capturedPiece.Type != PiecesType.Koropokkuru)
            {
                // Check Kodama Samurai
                if (capturedPiece.Type == PiecesType.KodamaSamurai)
                {
                    context.GameManager.ChangePieceToArchetype(capturedPiece, PiecesType.Kodama);
                }
                
                CapturedCell capturedCell = context.GameManager.GetRemainingCapturedCell(Owner);
                if (capturedCell != null)
                {
                    capturedPiece.transform.position = capturedCell.transform.position;
                    capturedPiece.Position = capturedCell.Position;
                    capturedCell.CapturedPiece = capturedPiece;
                    capturedPiece.transform.rotation = Quaternion.Euler(0, 0, Owner == context.Player1 ? 180 : 0); 
                }
            }
        }

        // Parachutage
        if (IsCaptured)
        {
            transform.position = cell.transform.position;
            IsCaptured = false;
            IsParachuted = true;
            context.GameManager.RemovePieceFromCapturedCell(this);
        }
        else
        {
            IsParachuted = false;
            transform.position += new Vector3(direction.x, direction.y);
        }
        
        // Déplacement
        Position = newPosition;
    }

    public void MoveAI(GameContext context, Cell cell, Vector2 direction, bool checkUndoMove = false)
    {
        // Enregistrement de l'action
        Vector2 newPosition = cell.Position;
        context.Actions.Add((this, newPosition));
        
        // Minimax Test
        if (checkUndoMove && _lastCapturedPiece != null)
        {
            _lastCapturedPiece.Position = _lastCapturedPiece._lastPositionBeforeCaptured;
            _lastCapturedPiece.transform.position = _lastCapturedPiece._lastPositionBeforeCaptured;
            _lastCapturedPiece.transform.rotation = Quaternion.Euler(0, 0, Owner == context.Player1 ? 180 : 0);
            _lastCapturedPiece.IsCaptured = false;
            _lastCapturedPiece.Owner = _lastCapturedPiece.Owner == context.Player1 ? context.Player2 : context.Player1;
            context.GameManager.RemovePieceFromCapturedCell(_lastCapturedPiece);
        }

        _lastCapturedPiece = null;
        
        // Capture
        var capturedPiece = context.AllPieces.FirstOrDefault(piece => piece.Position == newPosition);
        // Debug.LogError(newPosition + " " +capturedPiece);
        if (capturedPiece != null)
        {
            // Minimax Test
            {
                capturedPiece._lastPositionBeforeCaptured = cell.Position;
                _lastCapturedPiece = capturedPiece;
            }
            
            capturedPiece.Owner = context.IsFirstPlayerTurn ? context.Player1 : context.Player2;
            capturedPiece.IsCaptured = true;
            if (capturedPiece.Type != PiecesType.Koropokkuru)
            {
                CapturedCell capturedCell = context.GameManager.GetRemainingCapturedCell(Owner);
                if (capturedCell != null)
                {
                    // Debug.LogError("CaptureCell Transform Position " + capturedCell.transform.position);
                    // Debug.LogError("CaptureCell Position " + capturedCell.Position);
                    capturedPiece.transform.position = capturedCell.transform.position;
                    capturedPiece.Position = capturedCell.Position;
                    capturedCell.CapturedPiece = capturedPiece;
                    capturedPiece.transform.rotation = Quaternion.Euler(0, 0, Owner == context.Player1 ? 180 : 0); 
                }
            }
        }
        
        if (IsCaptured)
        {
            transform.position = cell.transform.position;
            IsCaptured = false;
            IsParachuted = true;
            context.GameManager.RemovePieceFromCapturedCell(this);
        }
        else
        {
            IsParachuted = false;
            transform.position += new Vector3(direction.x, direction.y);
        }
        
        // Déplacement
        Position = newPosition;
    }

    public List<(Vector2 position, Vector2 rotation)> GetAllowedMoves(GameContext context)
    {
        var moves = new List<(Vector2, Vector2)>();
        
        if (IsCaptured)
        {
            for (var i = 0; i < Board.COLUMN_COUNT; i++)
            {
                for (var j = 0; j < Board.ROW_COUNT; j++)
                {
                    var pos = new Vector2(i, j);
                    
                    if (context.AllPieces.All(piece => piece.Position != pos))
                        moves.Add((pos, Vector2.zero));
                }
            }
        }
        else
        {
            foreach (var direction in Directions)
            {
                Vector2 newDirection = (Owner == context.Player1 ? new Vector2(direction.x, -direction.y) : direction);
                var newPosition = Position + newDirection;

                var ownPieces = Owner == context.Player1 ? context.Player1Pieces : context.Player2Pieces;

                if (ownPieces.Any(piece => piece.Position == newPosition))
                    continue;
                
                if (!Board.IsOutOfBounds(newPosition))
                    moves.Add((newPosition ,newDirection));
            }   
        }

        return moves;
    }
    
    // public string GetFENName(GameContext context)
    // {
    //     string returnValue = "";
    //
    //     switch (Type)
    //     {
    //         case PiecesType.Kitsune :
    //             returnValue = Owner == context.Player1 ? "KI" : "ki";
    //             break;
    //         case PiecesType.Kodama : 
    //             returnValue = Owner == context.Player1 ? "KO" : "ko";
    //             break;
    //         case PiecesType.Koropokkuru : 
    //             returnValue = Owner == context.Player1 ? "KOR" : "kor";
    //             break;
    //         case PiecesType.Tanuki : 
    //             returnValue = Owner == context.Player1 ? "TA" : "ta";
    //             break;
    //         case PiecesType.KodamaSamurai : 
    //             returnValue = Owner == context.Player1 ? "KOS" : "kos";
    //             break;
    //     }
    //     
    //     return returnValue;
    // }

    public void ResetPiece(GameContext context)
    {
        Position = _startPosition;
        Owner = _startingOwner;
        transform.position = context.GameManager.GetCellFromPosition(Position).transform.position;
        transform.rotation = Quaternion.Euler(0, 0, Owner == context.Player1 ? 180 : 0);
        IsCaptured = false;
        IsParachuted = false;
        
        if (Type == PiecesType.KodamaSamurai)
        {
            context.GameManager.ChangePieceToArchetype(this, PiecesType.Kodama);
        }
    }
}