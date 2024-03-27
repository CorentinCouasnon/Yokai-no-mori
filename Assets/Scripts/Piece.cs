using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] Vector2 _startPosition;
    [SerializeField] public Player _startingOwner;
    
    static Dictionary<PiecesType, Sprite> _typeSprites;
    
    [field: SerializeField] public PiecesType Type { get; set; }
    [field: SerializeField] public Vector2[] Directions { get; set; }
    [field: SerializeField] public SpriteRenderer SpriteRenderer { get; set; }
    [field: SerializeField] public List<Sprite> PieceSprites { get; set; }
    
    public PieceData PieceData { get; set; }
    
    void Awake()
    {
        if (_typeSprites == null)
        {
            _typeSprites = new Dictionary<PiecesType, Sprite>
            {
                { PiecesType.Kodama, PieceSprites[0] },
                { PiecesType.Kitsune, PieceSprites[1] },
                { PiecesType.Tanuki, PieceSprites[2] },
                { PiecesType.KodamaSamurai, PieceSprites[3] },
                { PiecesType.Koropokkuru, PieceSprites[4] },
            };
        }
    }

    public void Move(GameContext context, Cell cell, Vector2 direction)
    {
        var pieceData = context.AllPieces.First(pieceData => pieceData.Piece == this);
        var index = context.AllPieces.IndexOf(pieceData);
        
        // Enregistrement de l'action
        var newPosition = cell.Position;
        context.Actions.Add((Type, newPosition));

        // Capture
        var capturedPiece = context.AllPieces.FirstOrDefault(piece => piece.Position == newPosition);
        
        if (capturedPiece.Type != PiecesType.None)
        {
            var capturedPieceIndex = context.AllPieces.IndexOf(capturedPiece);
            capturedPiece.IsCaptured = true;
            if (capturedPiece.Type != PiecesType.Koropokkuru)
            {
                capturedPiece.Owner = context.IsFirstPlayerTurn ? context.Player1 : context.Player2;
                if (capturedPiece.Type == PiecesType.KodamaSamurai)
                {
                    pieceData = context.GameManager.ChangePieceToArchetype(capturedPiece, PiecesType.Kodama);
                }
                
                capturedPiece.Position = context.GameManager.GetCaptureCellPosition(context, pieceData.Owner);
            }

            context.AllPieces[capturedPieceIndex] = capturedPiece;
        }
        
        // Parachutage
        if (pieceData.IsCaptured)
        {
            transform.position = cell.transform.position;
            pieceData.IsCaptured = false;
            pieceData.IsParachuted = true;
        }
        else
        {
            pieceData.IsParachuted = false;
            transform.position += new Vector3(direction.x, direction.y);
        }
        
        // Déplacement
        pieceData.Position = newPosition;
        context.AllPieces[index] = pieceData;
    }

    public void UpdatePiece(GameContext context)
    {
        PieceData = context.AllPieces.First(pieceData => pieceData.Piece == this);
        var cell = context.GameManager.GetAnyCellFromPosition(PieceData.Position);
        transform.position = cell.transform.position;
        transform.rotation = Quaternion.Euler(0, 0, PieceData.Owner == context.Player1 ? 180 : 0);
        SpriteRenderer.sprite = _typeSprites[PieceData.Type];
    }

    public List<(Vector2 position, Vector2 rotation)> GetAllowedMoves(GameContext context)
    {
        var moves = new List<(Vector2, Vector2)>();

        var pieceData = context.AllPieces.First(pieceData => pieceData.Piece == this);
        
        if (pieceData.IsCaptured)
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
            foreach (var direction in pieceData.Directions)
            {
                Vector2 newDirection = pieceData.Owner == context.Player1 ? new Vector2(direction.x, -direction.y) : direction;
                var newPosition = pieceData.Position + newDirection;

                var ownPieces = pieceData.Owner == context.Player1 ? context.Player1Pieces : context.Player2Pieces;

                if (ownPieces.Any(piece => piece.Position == newPosition))
                    continue;
                
                if (!Board.IsOutOfBounds(newPosition))
                    moves.Add((newPosition ,newDirection));
            }   
        }

        return moves;
    }

    public void ResetPiece(GameManager gameManager)
    {
        PieceData = new PieceData
        {
            Piece = this,
            Type = Type,
            IsCaptured = false,
            IsParachuted = false,
            Owner = _startingOwner.PlayerData,
            Position = _startPosition,
            Directions = Directions,
        };
        
        if (Type == PiecesType.KodamaSamurai)
        {
            PieceData = gameManager.ChangePieceToArchetype(PieceData, PiecesType.Kodama);
        }
    }
}