using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public override bool[,] PossibleMove(in Piece[,] chessboard)
    {
        bool[,] moves = new bool[8, 8];

        Move(Position.x + 2, Position.y + 1, ref moves, in chessboard);
        Move(Position.x + 2, Position.y - 1, ref moves, in chessboard);
        Move(Position.x - 2, Position.y + 1, ref moves, in chessboard);
        Move(Position.x - 2, Position.y - 1, ref moves, in chessboard);
        Move(Position.x + 1, Position.y + 2, ref moves, in chessboard);
        Move(Position.x + 1, Position.y - 2, ref moves, in chessboard);
        Move(Position.x - 1, Position.y + 2, ref moves, in chessboard);
        Move(Position.x - 1, Position.y - 2, ref moves, in chessboard);

        return moves;
    }

    public override bool[,] AttackedSpaces(in Piece[,] chessboard)
    {
        bool[,] attackedSpaces = new bool[8, 8];

        AttackedSpace(Position.x + 2, Position.y + 1, ref attackedSpaces);
        AttackedSpace(Position.x + 2, Position.y - 1, ref attackedSpaces);
        AttackedSpace(Position.x - 2, Position.y + 1, ref attackedSpaces);
        AttackedSpace(Position.x - 2, Position.y - 1, ref attackedSpaces);
        AttackedSpace(Position.x + 1, Position.y + 2, ref attackedSpaces);
        AttackedSpace(Position.x + 1, Position.y - 2, ref attackedSpaces);
        AttackedSpace(Position.x - 1, Position.y + 2, ref attackedSpaces);
        AttackedSpace(Position.x - 1, Position.y - 2, ref attackedSpaces);

        return attackedSpaces;
    }

    private void Move(int x, int y, ref bool[,] moves, in Piece[,] chessboard)
    {
        if (x >= 0 && x <= 7 && y >= 0 && y <= 7)
        {
            Piece p = chessboard[x, y];

            if (p == null)
            {
                moves[x, y] = true;
            }
            else if (isWhite != p.isWhite)
            {
                moves[x, y] = true;
            }
        }
    }

    private void AttackedSpace(int x, int y, ref bool[,] attackedSpaces)
    {
        if (x >= 0 && x <= 7 && y >= 0 && y <= 7)
        {
            attackedSpaces[x, y] = true;
        }
    }
}
