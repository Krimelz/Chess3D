using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public Vector2Int Position { get; set; }
    public bool isWhite;
    public bool wasWalking = false;

    public virtual bool[,] PossibleMove()
    {
        return new bool[8, 8];
    }

    public virtual bool[,] AttackedSpaces()
    {
        return new bool[8, 8];
    }
}
