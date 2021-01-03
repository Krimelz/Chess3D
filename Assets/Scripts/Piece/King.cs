using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public override bool[,] PossibleMove(in Piece[,] chessboard)
    {
        bool[,] moves = new bool[8, 8];
        Piece p;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (Position.x + i >= 0 && Position.x + i <= 7 && Position.y + j >= 0 && Position.y + j <= 7)
                {
                    p = chessboard[Position.x + i, Position.y + j];

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
            if (!wasWalking) 
            {
                // Короткая рокировка
                bool empty = true;
                for (int i = Position.x + 1; i <= 6; i++)
                {
                    p = chessboard[i, 0];

                    if (p != null)
                    {
                        empty = false;
                        break;
                    }
                }

                p = chessboard[7, 0];

                if (p != null && p.GetType() == typeof(Rook) && !p.wasWalking && empty)
                {
                    moves[6, 0] = true;
                }

                empty = true;

                // Длинная рокировка
                for (int i = Position.x - 1; i >= 1; i--)
                {
                    p = chessboard[i, 0];

                    if (p != null)
                    {
                        empty = false;
                        break;
                    }
                }

                p = chessboard[0, 0];

                if (p != null && p.GetType() == typeof(Rook) && !p.wasWalking && empty)
                {
                    moves[2, 0] = true;
                }
            }
        }
        else
        {
            if (!wasWalking) 
            {
                // Короткая рокировка
                bool empty = true;
                for (int i = Position.x + 1; i <= 6; i++)
                {
                    p = chessboard[i, 7];

                    if (p != null)
                    {
                        empty = false;
                        break;
                    }
                }

                p = chessboard[7, 7];

                if (p != null && p.GetType() == typeof(Rook) && !p.wasWalking && empty)
                {
                    moves[6, 7] = true;
                }

                empty = true;

                // Длинная рокировка
                for (int i = Position.x - 1; i >= 1; i--)
                {
                    p = chessboard[i, 7];

                    if (p != null)
                    {
                        empty = false;
                        break;
                    }
                }

                p = chessboard[0, 7];

                if (p != null && p.GetType() == typeof(Rook) && !p.wasWalking && empty)
                {
                    moves[2, 7] = true;
                }
            }
        }
        

        moves[Position.x, Position.y] = false;

        return moves;
    }

    public override bool[,] AttackedSpaces(in Piece[,] chessboard)
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
