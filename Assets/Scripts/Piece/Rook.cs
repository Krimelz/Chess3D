using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    public override bool[,] PossibleMove()
    {
        bool[,] moves = new bool[8, 8];
        Piece p;

        // right
        for (int i = Position.y + 1; i < 8; i++)
        {
            p = BoardController.Instance.chessboard[Position.x, i];

            if (p == null)
            {
                moves[Position.x, i] = true;
            }
            else
            {
                if (isWhite != p.isWhite)
                {
                    moves[Position.x, i] = true;
                }

                break;
            }
        }

        // left
        for (int i = Position.y - 1; i >= 0; i--)
        {
            p = BoardController.Instance.chessboard[Position.x, i];

            if (p == null)
            {
                moves[Position.x, i] = true;
            }
            else
            {
                if (isWhite != p.isWhite)
                {
                    moves[Position.x, i] = true;
                }

                break;
            }
        }

        // up
        for (int i = Position.x - 1; i >= 0; i--)
        {
            p = BoardController.Instance.chessboard[i, Position.y];

            if (p == null)
            {
                moves[i, Position.y] = true;
            }
            else
            {
                if (isWhite != p.isWhite)
                {
                    moves[i, Position.y] = true;
                }

                break;
            }
        }

        // down
        for (int i = Position.x + 1; i < 8; i++)
        {
            p = BoardController.Instance.chessboard[i, Position.y];

            if (p == null)
            {
                moves[i, Position.y] = true;
            }
            else
            {
                if (isWhite != p.isWhite)
                {
                    moves[i, Position.y] = true;
                }

                break;
            }
        }

        return moves;
    }

    public override bool[,] AttackedSpaces()
    {
        bool[,] attackedSpaces = new bool[8, 8];
        Piece p;

        // right
        for (int i = Position.y + 1; i < 8; i++)
        {
            p = BoardController.Instance.chessboard[Position.x, i];

            if (p == null || p.GetType() == typeof(King))
            {
                attackedSpaces[Position.x, i] = true;
            }
            else
            {
                attackedSpaces[Position.x, i] = true;
                break;
            }
        }

        // left
        for (int i = Position.y - 1; i >= 0; i--)
        {
            p = BoardController.Instance.chessboard[Position.x, i];

            if (p == null || p.GetType() == typeof(King))
            {
                attackedSpaces[Position.x, i] = true;
            }
            else
            {
                attackedSpaces[Position.x, i] = true;
                break;
            }
        }

        // up
        for (int i = Position.x - 1; i >= 0; i--)
        {
            p = BoardController.Instance.chessboard[i, Position.y];

            if (p == null || p.GetType() == typeof(King))
            {
                attackedSpaces[i, Position.y] = true;
            }
            else
            {
                attackedSpaces[i, Position.y] = true;
                break;
            }
        }

        // down
        for (int i = Position.x + 1; i < 8; i++)
        {
            p = BoardController.Instance.chessboard[i, Position.y];

            if (p == null || p.GetType() == typeof(King))
            {
                attackedSpaces[i, Position.y] = true;
            }
            else
            {
                attackedSpaces[i, Position.y] = true;
                break;
            }
        }

        attackedSpaces[Position.x, Position.y] = false;

        return attackedSpaces;
    }
}
