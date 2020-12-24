using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: рокировка

public class King : Piece
{
    public override bool[,] PossibleMove()
    {
        bool[,] moves = new bool[8, 8];
        Piece p;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (Position.x + i >= 0 && Position.x + i <= 7 && Position.y + j >= 0 && Position.y + j <= 7)
                {
                    p = BoardController.Instance.chessboard[Position.x + i, Position.y + j];

                    if (p == null)
                    {
                        moves[Position.x + i, Position.y + j] = true;
                    }
                    else if (isWhite != p.isWhite)
                    {
                        moves[Position.x + i, Position.y + j] = true;
                    }
                }
            }
        }

        if (isWhite)
        {
            if (Position.x == 5 && Position.y == 1) // && ни разу не ходила
            {
                // Короткая рокировка
                // Длинная рокировка
            }
        }
        else
        {
            if (Position.x == 5 && Position.y == 7) // && ни разу не ходила
            {
                // Короткая рокировка
                // Длинная рокировка
            }
        }
        

        moves[Position.x, Position.y] = false;

        return moves;
    }

    public override bool[,] AttackedSpaces()
    {
        bool[,] attackedSpaces = new bool[8, 8];

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (Position.x + i >= 0 && Position.x + i <= 7 && Position.y + j >= 0 && Position.y + j <= 7)
                {
                    attackedSpaces[Position.x + i, Position.y + j] = true;
                }
            }
        }

        attackedSpaces[Position.x, Position.y] = false;

        return attackedSpaces;
    }
}
