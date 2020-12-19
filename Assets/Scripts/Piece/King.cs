using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public override bool[,] PossibleMove()
    {
        bool[,] moves = new bool[8, 8];

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (Position.x + i >= 0 && Position.x + i <= 7 && Position.y + j >= 0 && Position.y + j <= 7)
                {
                    moves[Position.x + i, Position.y + j] = true;
                }
            }
        }

        moves[Position.x, Position.y] = false;

        return moves;
    }
}
