using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public Vector2Int Position { get; set; }
    public bool isWhite;

    public virtual bool PossibleMove(int x, int y)
    {
        return true;
    }
}
