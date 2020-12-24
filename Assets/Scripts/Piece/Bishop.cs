using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public override bool[,] PossibleMove()
    {
        bool[,] moves = new bool[8, 8];
        Piece p;

        int i = Position.x;
        int j = Position.y;

        while (true)
        {
            i--;
            j--;

            if (i < 0 || j < 0)
                break;

            p = BoardController.Instance.chessboard[i, j];

            if (p == null)
            {
                moves[i, j] = true;
            }
            else
            {
                if (isWhite != p.isWhite)
                {
                    moves[i, j] = true;
                }

                break;
            }
        }

        i = Position.x;
        j = Position.y;

        while (true)
        {
            i++;
            j++;

            if (i > 7 || j > 7)
                break;

            p = BoardController.Instance.chessboard[i, j];

            if (p == null)
            {
                moves[i, j] = true;
            }
            else
            {
                if (isWhite != p.isWhite)
                {
                    moves[i, j] = true;
                }

                break;
            }
        }

        i = Position.x;
        j = Position.y;

        while (true)
        {
            i++;
            j--;

            if (i > 7 || j < 0)
                break;

            p = BoardController.Instance.chessboard[i, j];

            if (p == null)
            {
                moves[i, j] = true;
            }
            else
            {
                if (isWhite != p.isWhite)
                {
                    moves[i, j] = true;
                }

                break;
            }
        }

        i = Position.x;
        j = Position.y;

        while (true)
        {
            i--;
            j++;

            if (i < 0 || j > 7)
                break;

            p = BoardController.Instance.chessboard[i, j];

            if (p == null)
            {
                moves[i, j] = true;
            }
            else
            {
                if (isWhite != p.isWhite)
                {
                    moves[i, j] = true;
                }

                break;
            }
        }

        moves[Position.x, Position.y] = false;

        return moves;
    }

    public override bool[,] AttackedSpaces()
    {
        bool[,] attackedSpaces = new bool[8, 8];

        Piece p;

        int i = Position.x;
        int j = Position.y;

        while (true)
        {
            i--;
            j--;

            if (i < 0 || j < 0)
                break;

            p = BoardController.Instance.chessboard[i, j];

            if (p == null)
            {
                attackedSpaces[i, j] = true;
            }
            else
            {
                attackedSpaces[i, j] = true;
                break;
            }
        }

        i = Position.x;
        j = Position.y;

        while (true)
        {
            i++;
            j++;

            if (i > 7 || j > 7)
                break;

            p = BoardController.Instance.chessboard[i, j];

            if (p == null)
            {
                attackedSpaces[i, j] = true;
            }
            else
            {
                attackedSpaces[i, j] = true;
                break;
            }
        }

        i = Position.x;
        j = Position.y;

        while (true)
        {
            i++;
            j--;

            if (i > 7 || j < 0)
                break;

            p = BoardController.Instance.chessboard[i, j];

            if (p == null)
            {
                attackedSpaces[i, j] = true;
            }
            else
            { 
                attackedSpaces[i, j] = true;
                break;
            }
        }

        i = Position.x;
        j = Position.y;

        while (true)
        {
            i--;
            j++;

            if (i < 0 || j > 7)
                break;

            p = BoardController.Instance.chessboard[i, j];

            if (p == null)
            {
                attackedSpaces[i, j] = true;
            }
            else
            {
                attackedSpaces[i, j] = true;
                break;
            }
        }

        attackedSpaces[Position.x, Position.y] = false;

        return attackedSpaces;
    }
}
