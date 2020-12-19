using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public override bool[,] PossibleMove()
    {
        bool[,] moves = new bool[8, 8];

        Move(Position.x + 2, Position.y + 1, ref moves);
        Move(Position.x + 2, Position.y - 1, ref moves);
        Move(Position.x - 2, Position.y + 1, ref moves);
        Move(Position.x - 2, Position.y - 1, ref moves);
        Move(Position.x + 1, Position.y + 2, ref moves);
        Move(Position.x + 1, Position.y - 2, ref moves);
        Move(Position.x - 1, Position.y + 2, ref moves);
        Move(Position.x - 1, Position.y - 2, ref moves);

        return moves;
    }

    private void Move(int x, int y, ref bool[,] moves)
    {
        if (x >= 0 && x <= 7 && y >= 0 && y <= 7)
        {
            moves[x, y] = true;
        }
    }
}
