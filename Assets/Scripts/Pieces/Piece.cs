using ChessBoard;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pieces
{
    public abstract class Piece : MonoBehaviour
    {
        public Vector2Int Position { get; set; }
        public Team team;
        public bool wasWalking;

        public virtual bool[,] PossibleMove()
        {
            return new bool[8, 8];
        }

        public virtual bool[,] AttackedSpaces()
        {
            return new bool[8, 8];
        }
    }
}