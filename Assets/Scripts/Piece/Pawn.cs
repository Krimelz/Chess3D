using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override bool[,] PossibleMove()
    {
        bool[,] moves = new bool[8, 8];
        int[] passant = BoardController.Instance.Passant;
        Piece p1, p2;

        if (isWhite)
        {
            // left
            if (Position.x != 0 && Position.y != 7)
            {
                if (passant[0] == Position.x - 1 && passant[1] == Position.y + 1)
                {
                    moves[Position.x - 1, Position.y + 1] = true;
                }

                p1 = BoardController.Instance.chessboard[Position.x - 1, Position.y + 1];

                if (p1 != null && !p1.isWhite)
                {
                    moves[Position.x - 1, Position.y + 1] = true;
                }
            }

            // right
            if (Position.x != 7 && Position.y != 7)
            {
                if (passant[0] == Position.x + 1 && passant[1] == Position.y + 1)
                {
                    moves[Position.x + 1, Position.y + 1] = true;
                }

                p1 = BoardController.Instance.chessboard[Position.x + 1, Position.y + 1];

                if (p1 != null && !p1.isWhite)
                {
                    moves[Position.x + 1, Position.y + 1] = true;
                }
            }

            if (Position.y == 1)
            {
                p1 = BoardController.Instance.chessboard[Position.x, Position.y + 1];
                p2 = BoardController.Instance.chessboard[Position.x, Position.y + 2];

                if (p1 == null && p2 == null)
                {
                    moves[Position.x, Position.y + 2] = true;
                }
            }

            if (Position.y != 7)
            {
                p1 = BoardController.Instance.chessboard[Position.x, Position.y + 1];

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
                if (passant[0] == Position.x + 1 && passant[1] == Position.y - 1)
                {
                    moves[Position.x + 1, Position.y + 1] = true;
                }

                p1 = BoardController.Instance.chessboard[Position.x - 1, Position.y - 1];

                if (p1 != null && p1.isWhite)
                {
                    moves[Position.x - 1, Position.y - 1] = true;
                }
            }

            // left
            if (Position.x != 7 && Position.y != 0)
            {
                if (passant[0] == Position.x - 1 && passant[1] == Position.y - 1)
                {
                    moves[Position.x - 1, Position.y - 1] = true;
                }

                p1 = BoardController.Instance.chessboard[Position.x + 1, Position.y - 1];

                if (p1 != null && p1.isWhite)
                {
                    moves[Position.x + 1, Position.y - 1] = true;
                }
            }

            if (Position.y == 6)
            {
                p1 = BoardController.Instance.chessboard[Position.x, Position.y - 1];
                p2 = BoardController.Instance.chessboard[Position.x, Position.y - 2];

                if (p1 == null && p2 == null)
                {
                    moves[Position.x, Position.y - 2] = true;
                }
            }

            if (Position.y != 0)
            {
                p1 = BoardController.Instance.chessboard[Position.x, Position.y - 1];

                if (p1 == null)
                {
                    moves[Position.x, Position.y - 1] = true;
                }
            }
        }

        return moves;
    }
}
