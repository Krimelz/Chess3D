using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override bool[,] PossibleMove(in Piece[,] chessboard)
    {
        bool[,] moves = new bool[8, 8];
        int[] enPassant = BoardController.EnPassant;
        bool enPassantColor = BoardController.EnPassantColor;
        Piece p1, p2;

        if (isWhite)
        {
            // left
            if (Position.x != 0 && Position.y != 7)
            {
                if (enPassant[0] == Position.x - 1 && enPassant[1] == Position.y + 1 && !enPassantColor)
                {
                    moves[Position.x - 1, Position.y + 1] = true;
                }

                p1 = chessboard[Position.x - 1, Position.y + 1];

                if (p1 != null && !p1.isWhite)
                {
                    moves[Position.x - 1, Position.y + 1] = true;
                }
            }

            // right
            if (Position.x != 7 && Position.y != 7)
            {
                if (enPassant[0] == Position.x + 1 && enPassant[1] == Position.y + 1 && !enPassantColor)
                {
                    moves[Position.x + 1, Position.y + 1] = true;
                }

                p1 = chessboard[Position.x + 1, Position.y + 1];

                if (p1 != null && !p1.isWhite)
                {
                    moves[Position.x + 1, Position.y + 1] = true;
                }
            }

            if (Position.y == 1)
            {
                p1 = chessboard[Position.x, Position.y + 1];
                p2 = chessboard[Position.x, Position.y + 2];

                if (p1 == null && p2 == null)
                {
                    moves[Position.x, Position.y + 2] = true;
                }
            }

            if (Position.y != 7)
            {
                p1 = chessboard[Position.x, Position.y + 1];

                if (p1 == null)
                {
                    moves[Position.x, Position.y + 1] = true;
                }
            }
        }
        else
        {
            // right
            if (Position.x != 0 && Position.y != 0)
            {
                if (enPassant[0] == Position.x + 1 && enPassant[1] == Position.y - 1 && enPassantColor)
                {
                    moves[Position.x + 1, Position.y + 1] = true;
                }

                p1 = chessboard[Position.x - 1, Position.y - 1];

                if (p1 != null && p1.isWhite)
                {
                    moves[Position.x - 1, Position.y - 1] = true;
                }
            }

            // left
            if (Position.x != 7 && Position.y != 0)
            {
                if (enPassant[0] == Position.x - 1 && enPassant[1] == Position.y - 1 && enPassantColor)
                {
                    moves[Position.x - 1, Position.y - 1] = true;
                }

                p1 = chessboard[Position.x + 1, Position.y - 1];

                if (p1 != null && p1.isWhite)
                {
                    moves[Position.x + 1, Position.y - 1] = true;
                }
            }

            if (Position.y == 6)
            {
                p1 = chessboard[Position.x, Position.y - 1];
                p2 = chessboard[Position.x, Position.y - 2];

                if (p1 == null && p2 == null)
                {
                    moves[Position.x, Position.y - 2] = true;
                }
            }

            if (Position.y != 0)
            {
                p1 = chessboard[Position.x, Position.y - 1];

                if (p1 == null)
                {
                    moves[Position.x, Position.y - 1] = true;
                }
            }
        }

        return moves;
    }

    public override bool[,] AttackedSpaces(in Piece[,] chessboard)
    {
        bool[,] attackedSpaces = new bool[8, 8];

        if (isWhite)
        {
            if (Position.x > 0)
                attackedSpaces[Position.x - 1, Position.y + 1] = true;

            if (Position.x < 7)
                attackedSpaces[Position.x + 1, Position.y + 1] = true;
        }
        else
        {
            if (Position.x > 0)
                attackedSpaces[Position.x - 1, Position.y - 1] = true;

            if (Position.x < 7)
                attackedSpaces[Position.x + 1, Position.y - 1] = true;
        }

        return attackedSpaces;
    }
}
