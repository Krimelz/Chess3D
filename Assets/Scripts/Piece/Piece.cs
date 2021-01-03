using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public Vector2Int Position { get; set; }
    public bool isWhite;
    public bool wasWalking = false;

    public virtual bool[,] PossibleMove(in Piece[,] chessboard)
    {
        return new bool[8, 8];
    }

    public virtual bool[,] AttackedSpaces(in Piece[,] chessboard)
    {
        return new bool[8, 8];
    }
}
