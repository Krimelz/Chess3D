using ChessBoard;

namespace Pieces
{
    public class Queen : Piece
    {
        public override bool[,] PossibleMove()
        {
            bool[,] moves = new bool[8, 8];
            Piece p;
            int i, j;

            // right
            for (i = Position.y + 1; i < 8; i++)
            {
                p = Board.Instance.chessboard[Position.x, i];

                if (p == null)
                {
                    moves[Position.x, i] = true;
                }
                else
                {
                    if (team != p.team)
                    {
                        moves[Position.x, i] = true;
                    }

                    break;
                }
            }

            // left
            for (i = Position.y - 1; i >= 0; i--)
            {
                p = Board.Instance.chessboard[Position.x, i];

                if (p == null)
                {
                    moves[Position.x, i] = true;
                }
                else
                {
                    if (team != p.team)
                    {
                        moves[Position.x, i] = true;
                    }

                    break;
                }
            }

            // up
            for (i = Position.x - 1; i >= 0; i--)
            {
                p = Board.Instance.chessboard[i, Position.y];

                if (p == null)
                {
                    moves[i, Position.y] = true;
                }
                else
                {
                    if (team != p.team)
                    {
                        moves[i, Position.y] = true;
                    }

                    break;
                }
            }

            // down
            for (i = Position.x + 1; i < 8; i++)
            {
                p = Board.Instance.chessboard[i, Position.y];

                if (p == null)
                {
                    moves[i, Position.y] = true;
                }
                else
                {
                    if (team != p.team)
                    {
                        moves[i, Position.y] = true;
                    }

                    break;
                }
            }

            i = Position.x;
            j = Position.y;

            // left top
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

            // right down
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

            // left down
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

            // right top
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
            int i, j;

            // right
            for (i = Position.y + 1; i < 8; i++)
            {
                p = Board.Instance.chessboard[Position.x, i];

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
            for (i = Position.y - 1; i >= 0; i--)
            {
                p = Board.Instance.chessboard[Position.x, i];

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
            for (i = Position.x - 1; i >= 0; i--)
            {
                p = Board.Instance.chessboard[i, Position.y];

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
            for (i = Position.x + 1; i < 8; i++)
            {
                p = Board.Instance.chessboard[i, Position.y];

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

            i = Position.x;
            j = Position.y;

            // left top
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

            // right down
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

            // left down
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

            // right top
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
