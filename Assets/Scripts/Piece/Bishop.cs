using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public override bool[,] PossibleMove()
    {
        bool[,] moves = new bool[8, 8];

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (Mathf.Abs(i - Position.x) == Mathf.Abs(j - Position.y))
                {
                    moves[i, j] = true;
                }
            }
        }

        moves[Position.x, Position.y] = false;

        return moves;
    }
}
