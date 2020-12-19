using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    public override bool[,] PossibleMove()
    {
        bool[,] moves = new bool[8, 8];

        for (int i = 0; i < 8; i++)
        {
            moves[i, Position.y] = true;
        }

        for (int i = 0; i < 8; i++)
        {
            moves[Position.x, i] = true;
        }

        moves[Position.x, Position.y] = false;

        return moves;
    }
}
