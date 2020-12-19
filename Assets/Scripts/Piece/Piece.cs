using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public Vector2Int Position { get; set; }
    public bool isWhite;

    public virtual bool[,] PossibleMove()
    {
        return new bool[8, 8];
    }
}
