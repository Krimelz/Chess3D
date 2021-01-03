using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _piecesPrefabs;

    public static int[] EnPassant { get; private set; } // <--
    public static bool EnPassantColor { get; private set; } // <--
    private bool[,] _allowedMoves { get; set; }
    private bool[,] _allSpacesUnderAttack { get; set; }

    private Camera _camera;
    private Ray _ray;
    private RaycastHit _hit = new RaycastHit();
    private bool _isHit;
    private Vector2Int _selection;

    private Piece[,] _chessboard;
    private Piece _selectedPiece;
    private List<GameObject> _pieces;
    private bool _isWhiteTurn = true;
    private bool _gameIsPaused = false;

    private Piece _whiteKing;
    private Piece _blackKing;
    private bool _kingIsAttacked = false;

    void Start()
    {
        _camera = Camera.main;

        SpawnAllPieces();

        EventManager.AddEvent("RestartGame", RestartGame);
        EventManager<int>.AddEvent("ChangePawn", ChangePawn);
        EventManager<bool>.AddEvent("Pause", Pause);

    }

    void Update()
    {
        if (_gameIsPaused)
            return;

        UpdateSelection();

        if (Input.GetMouseButtonDown(0))
        {
            if (_isHit)
            {
                if (_selectedPiece == null)
                {
                    SelectPiece(_selection.x, _selection.y);
                }
                else
                {
                    MovePiece(_selection.x, _selection.y);
                    CheckKing();
                    isCheckmate();
                }
            }
        }
    }

    void UpdateSelection()
    {
        _ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (_isHit = Physics.Raycast(_ray, out _hit))
        {
            _selection = new Vector2Int(Mathf.RoundToInt(_hit.point.x), Mathf.RoundToInt(_hit.point.z));
        }

        EventManager<Vector2Int, bool>.Broadcast("UpdateSelectionHighlight", _selection, _isHit);
    }

    bool HasMoves()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (_allowedMoves[i, j])
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void isCheckmate()
    {
        bool hasMoves = false;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece p = _chessboard[i, j];

                if (p != null && p.isWhite == _isWhiteTurn)
                {
                    if (_kingIsAttacked)
                    {
                        if (p.GetType() == typeof(King))
                        {
                            _allowedMoves = p.PossibleMove(in _chessboard);
                            KingMoves();
                        }
                        else
                        {
                            _allowedMoves = p.PossibleMove(in _chessboard);
                            PieceMoves(p);
                        }
                    }
                    else
                    {
                        if (p.GetType() == typeof(King))
                        {
                            _allowedMoves = p.PossibleMove(in _chessboard);
                            KingMoves();
                        }
                        else
                        {
                            _allowedMoves = p.PossibleMove(in _chessboard);
                        }
                    }

                    hasMoves |= HasMoves();
                }
            }
        }

        if (!hasMoves)
            EndGame();
    }
    void SelectPiece(int x, int y)
    {
        if (_chessboard[x, y] == null)
            return;

        if (_chessboard[x, y].isWhite != _isWhiteTurn)
            return;

        _selectedPiece = _chessboard[x, y];
        

        if (_selectedPiece.GetType() == typeof(King))
        {
            _allowedMoves = _selectedPiece.PossibleMove(in _chessboard);
            KingMoves();
        }
        else
        {
            _allowedMoves = _selectedPiece.PossibleMove(in _chessboard);
            PieceMoves(_selectedPiece);
        }   


        if (!HasMoves())
        {
            _selectedPiece = null;
            return;
        }

        EventManager<bool[,]>.Broadcast("EnableMoveHighligts", _allowedMoves);
    }

    void MovePiece(int x, int y)
    {
        if (_allowedMoves[x, y])
        {
            Piece p = _chessboard[x, y];

            if (p != null && p.isWhite != _isWhiteTurn)
            {
                _pieces.Remove(p.gameObject);
                Destroy(p.gameObject);
                p = null;
            }

            // TODO: Вынести в функцию
            if (_selectedPiece.GetType() == typeof(Pawn))
            {
                if (p == null && y == 7 || y == 0)
                {
                    EventManager.Broadcast("EnableChangePawn");
                    Pause(true);
                    return;
                }

                if (x == EnPassant[0] && y == EnPassant[1])
                {
                    if (_isWhiteTurn)
                    {
                        p = _chessboard[x, y - 1];
                    }
                    else
                    {
                        p = _chessboard[x, y + 1];
                    }

                    _pieces.Remove(p.gameObject);
                    Destroy(p.gameObject);
                }

                EnPassant[0] = -1;
                EnPassant[1] = -1;
                if (_selectedPiece.Position.y == 1 && y == 3)
                {
                    EnPassant[0] = x;
                    EnPassant[1] = y - 1;
                    EnPassantColor = true;
                }
                else if (_selectedPiece.Position.y == 6 && y == 4)
                {
                    EnPassant[0] = x;
                    EnPassant[1] = y + 1;
                    EnPassantColor = false;
                }
            }

            _chessboard[_selectedPiece.Position.x, _selectedPiece.Position.y] = null;
            _selectedPiece.transform.position = CalcSpaceCoords(x, y);
            _selectedPiece.Position = new Vector2Int(x, y);

            if (_selectedPiece.GetType() == typeof(King))
            {
                if (!_selectedPiece.wasWalking)
                {
                    _selectedPiece.wasWalking = true;
                    Castling();
                }

                if (_isWhiteTurn)
                {
                    _whiteKing.Position = _selectedPiece.Position;
                }
                else
                {
                    _blackKing.Position = _selectedPiece.Position;
                }
            }

            _selectedPiece.wasWalking = true;
            _chessboard[x, y] = _selectedPiece;
            _isWhiteTurn = !_isWhiteTurn;
            _kingIsAttacked = false;
        }

        EventManager.Broadcast("DisableMoveHighligts");
        _selectedPiece = null;
    }

    private void Castling()
    {
        if (_isWhiteTurn && !_kingIsAttacked)
        {
            if (_selection.x == 6)
            {
                _chessboard[5, 0] = _chessboard[7, 0];
                _chessboard[5, 0].Position = new Vector2Int(5, 0);
                _chessboard[5, 0].wasWalking = true;
                _chessboard[5, 0].transform.position = CalcSpaceCoords(5, 0);
                _chessboard[7, 0] = null;
            }
            else if (_selection.x == 2)
            {
                _chessboard[3, 0] = _chessboard[0, 0];
                _chessboard[3, 0].Position = new Vector2Int(3, 0);
                _chessboard[3, 0].wasWalking = true;
                _chessboard[3, 0].transform.position = CalcSpaceCoords(3, 0);
                _chessboard[0, 0] = null;
            }
        }
        else
        {
            if (_selection.x == 6)
            {
                _chessboard[5, 7] = _chessboard[7, 7];
                _chessboard[5, 7].Position = new Vector2Int(5, 7);
                _chessboard[5, 7].wasWalking = true;
                _chessboard[5, 7].transform.position = CalcSpaceCoords(5, 7);
                _chessboard[7, 7] = null;
            }
            else if (_selection.x == 2)
            {
                _chessboard[3, 7] = _chessboard[0, 7];
                _chessboard[3, 7].Position = new Vector2Int(3, 7);
                _chessboard[3, 7].wasWalking = true;
                _chessboard[3, 7].transform.position = CalcSpaceCoords(3, 7);
                _chessboard[0, 7] = null;
            }
        }
    }

    private void AllSpacesUnderAttack()
    {
        _allSpacesUnderAttack = new bool[8, 8];

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Piece p = _chessboard[x, y];

                if (p != null && p.isWhite != _isWhiteTurn)
                {
                    bool[,] attackedSpaces = p.AttackedSpaces(in _chessboard);

                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            _allSpacesUnderAttack[i, j] |= attackedSpaces[i, j];
                        }
                    }
                }
            }
        }
    }

    private void CheckKing()
    {
        AllSpacesUnderAttack();

        if (_isWhiteTurn)
        {
            if (_allSpacesUnderAttack[_whiteKing.Position.x, _whiteKing.Position.y])
            {
                _kingIsAttacked = true;
            }
            else
            {
                _kingIsAttacked = false;
            }
        }
        else
        {
            if (_allSpacesUnderAttack[_blackKing.Position.x, _blackKing.Position.y])
            {
                _kingIsAttacked = true;
            }
            else
            {
                _kingIsAttacked = false;
            }
        }  
    }

    private void KingMoves()
    {
        Piece p;

        if (_isWhiteTurn)
        {
            p = _whiteKing;
        }
        else
        {
            p = _blackKing;
        }

        AllSpacesUnderAttack();

        for (int i = p.Position.x - 2; i <= p.Position.x + 2; i++)
        {
            for (int j = p.Position.y - 1; j <= p.Position.y + 1; j++)
            {
                if (i >= 0 && i < 8 && j >= 0 && j < 8)
                {
                    if (_allSpacesUnderAttack[i, j])
                    {
                        _allowedMoves[i, j] = false;
                    }
                }
            }
        }
    }

    private void PieceMoves(Piece piece)
    {
        bool[,] pm = piece.PossibleMove(in _chessboard);
        Piece tmp;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pm[i, j])
                {
                    tmp = null;
                    if (_chessboard[i, j] != null)
                    {
                        tmp = _chessboard[i, j];
                    }

                    _chessboard[i, j] = piece;
                    _chessboard[piece.Position.x, piece.Position.y] = null;
                    CheckKing();

                    if (_kingIsAttacked)
                    {
                        _allowedMoves[i, j] = false;
                    }
                    else
                    {
                        _allowedMoves[i, j] = true;
                    }

                    _chessboard[i, j] = tmp;
                    _chessboard[piece.Position.x, piece.Position.y] = piece;
                }
            }
        }
    }

    void ChangePawn(int pieceNumber)
    {
        _gameIsPaused = false;

        if (!_selectedPiece.isWhite)
        {
            pieceNumber += 6;
        }

        // удаляю фигуру из списка
        _pieces.Remove(_selectedPiece.gameObject);
        // спавню новую фигуру
        SpawnPiece(pieceNumber, _selectedPiece.Position.x, _selectedPiece.Position.y);
        // уничтожаю объект старой фигуры
        Destroy(_selectedPiece.gameObject);

        // выбираю новую фигуру с доски
        _selectedPiece = _chessboard[_selectedPiece.Position.x, _selectedPiece.Position.y];
        // обнуляю клетку, из которой ходит фигрура
        _chessboard[_selectedPiece.Position.x, _selectedPiece.Position.y] = null;
        // перемещаю
        _selectedPiece.transform.position = CalcSpaceCoords(_selection.x, _selection.y);
        // присваиваю объекту фигуры новые координаты
        _selectedPiece.Position = new Vector2Int(_selection.x, _selection.y);
        _selectedPiece.wasWalking = true;
        // присваиваю фигуру клетке, в которую походил
        _chessboard[_selection.x, _selection.y] = _selectedPiece; 

        _isWhiteTurn = !_isWhiteTurn;

        EventManager.Broadcast("DisableMoveHighligts");
        _selectedPiece = null; // обнуляю выбранную фигуру

        CheckKing();
    }

    void SpawnPiece(int index, int x, int y)
    {
        GameObject piece = Instantiate(_piecesPrefabs[index], CalcSpaceCoords(x, y), Quaternion.identity);
        piece.transform.SetParent(transform);
        _chessboard[x, y] = piece.GetComponent<Piece>();
        _chessboard[x, y].Position = new Vector2Int(x, y);
        _pieces.Add(piece);

        if (index == 5)
        {
            _whiteKing = _chessboard[x, y];
        }
        else if (index == 11)
        {
            _blackKing = _chessboard[x, y];
        }
    }

    void SpawnAllPieces()
    {
        _pieces = new List<GameObject>();
        _chessboard = new Piece[8, 8];
        EnPassant = new int[] { -1, -1 };
        // --- White --- //

        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnPiece(0, i, 1);
        }

        // Knigts
        SpawnPiece(1, 1, 0);
        SpawnPiece(1, 6, 0);

        //Rooks
        SpawnPiece(2, 0, 0);
        SpawnPiece(2, 7, 0);

        //Bishops
        SpawnPiece(3, 2, 0);
        SpawnPiece(3, 5, 0);

        //Queen
        SpawnPiece(4, 3, 0);

        //King
        SpawnPiece(5, 4, 0);

        // --- Black --- //

        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnPiece(6, i, 6);
        }

        // Knigts
        SpawnPiece(7, 1, 7);
        SpawnPiece(7, 6, 7);

        //Rooks
        SpawnPiece(8, 0, 7);
        SpawnPiece(8, 7, 7);

        //Bishops
        SpawnPiece(9, 2, 7);
        SpawnPiece(9, 5, 7);

        //Queen
        SpawnPiece(10, 3, 7);

        //King
        SpawnPiece(11, 4, 7);
    }

    private void DestroyAllPieces()
    {
        foreach (GameObject p in _pieces)
        {
            Destroy(p.gameObject);
        }
    }

    Vector3 CalcSpaceCoords(int x, int y)
    {
        return new Vector3(x, 0f, y);
    }

    private void Pause(bool isPaused)
    {
        _gameIsPaused = isPaused;
    }

    private void RestartGame()
    {
        DestroyAllPieces();
        _gameIsPaused = false;
        _isWhiteTurn = true;
        SpawnAllPieces();
        EventManager.Broadcast("DisableMoveHighligts");
    }

    private void EndGame()
    {
        Pause(true);
        EventManager.Broadcast("DisableMoveHighligts");
        EventManager<bool>.Broadcast("PrintWinner", !_isWhiteTurn);
    }
}
