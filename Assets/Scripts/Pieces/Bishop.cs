using ChessBoard;

namespace Pieces
{
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

                p = Board.Instance.chessboard[i, j];

                if (p == null)
                {
                    moves[i, j] = true;
                }
                else
                {
                    if (team != p.team)
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

                p = Board.Instance.chessboard[i, j];

                if (p == null)
                {
                    moves[i, j] = true;
                }
                else
                {
                    if (team != p.team)
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

                p = Board.Instance.chessboard[i, j];

                if (p == null)
                {
                    moves[i, j] = true;
                }
                else
                {
                    if (team != p.team)
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

                p = Board.Instance.chessboard[i, j];

                if (p == null)
                {
                    moves[i, j] = true;
                }
                else
                {
                    if (team != p.team)
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

                p = Board.Instance.chessboard[i, j];

                if (p == null || p.GetType() == typeof(King))
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

                p = Board.Instance.chessboard[i, j];

                if (p == null || p.GetType() == typeof(King))
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

                p = Board.Instance.chessboard[i, j];

                if (p == null || p.GetType() == typeof(King))
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

                p = Board.Instance.chessboard[i, j];

                if (p == null || p.GetType() == typeof(King))
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
}
