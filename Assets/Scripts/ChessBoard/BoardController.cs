using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Мат

public class BoardController : MonoBehaviour
{
    public static BoardController Instance { get; set; }

    public delegate void DelBool(bool turn);
    public delegate void DelVoid();

    public event DelBool win;
    public event DelVoid changePawn;

    [SerializeField]
    private GameObject _highlihgt;
    [SerializeField]
    private List<GameObject> _piecesPrefabs;

    public int[] EnPassant { get; set; }
    public bool EnPassantColor { get; set; }
    [SerializeField]
    private GameObject _moveHighlightPrefab;
    private GameObject[,] _moveHighlights;
    private bool[,] _allowedMoves { get; set; }
    private bool[,] _allSpacesUnderAttack { get; set; }

    private Camera _camera;
    private Ray _ray;
    private RaycastHit _hit = new RaycastHit();

    public Piece[,] chessboard;
    [SerializeField]
    private Vector2Int _selection;
    private Piece _selectedPiece;
    private List<GameObject> _pieces;
    private bool _isWhiteTurn = true;
    private bool _GameIsPaused = false;

    private Piece _whiteKing;
    private Piece _blackKing;
    private bool _kingIsAttacked = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _camera = Camera.main;

        GameMenuController.Instance.restartGame += RestartGame;
        GameMenuController.Instance.changePawn += ChangePawn;
        GameMenuController.Instance.gamePause += Pause;

        SpawnAllPieces();
        CreateMoveHighligts();
    }

    void Update()
    {
        if (_GameIsPaused)
            return;

        UpdateSelection();

        if (Input.GetMouseButtonDown(0))
        {
            if (_highlihgt.activeSelf)
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

        if (Physics.Raycast(_ray, out _hit))
        {
            _highlihgt.SetActive(true);
            _selection = new Vector2Int(Mathf.RoundToInt(_hit.point.x), Mathf.RoundToInt(_hit.point.z));
            _highlihgt.transform.position = new Vector3(_selection.x, 0.015f, _selection.y);
        }
        else
        {
            _highlihgt.SetActive(false);
        }
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
                Piece p = chessboard[i, j];

                if (p != null && p.isWhite == _isWhiteTurn)
                {
                    if (_kingIsAttacked)
                    {
                        if (p.GetType() == typeof(King))
                        {
                            _allowedMoves = p.PossibleMove();
                            KingMoves();
                        }
                        else
                        {
                            _allowedMoves = p.PossibleMove();
                            PieceMoves(p);
                        }
                    }
                    else
                    {
                        if (p.GetType() == typeof(King))
                        {
                            _allowedMoves = p.PossibleMove();
                            KingMoves();
                        }
                        else
                        {
                            _allowedMoves = p.PossibleMove();
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
        if (chessboard[x, y] == null)
            return;

        if (chessboard[x, y].isWhite != _isWhiteTurn)
            return;

        _selectedPiece = chessboard[x, y];
        
        if (_kingIsAttacked)
        {
            if (_selectedPiece.GetType() == typeof(King))
            {
                _allowedMoves = _selectedPiece.PossibleMove();
                KingMoves();
            }
            else
            {
                _allowedMoves = _selectedPiece.PossibleMove();
                PieceMoves(_selectedPiece);
            }   
        }
        else
        {
            if (_selectedPiece.GetType() == typeof(King))
            {
                _allowedMoves = _selectedPiece.PossibleMove();
                KingMoves();
            }
            else
            {
                _allowedMoves = _selectedPiece.PossibleMove();
            }
        }

        if (!HasMoves())
        {
            _selectedPiece = null;
            return;
        }

        EnableMoveHighligts();
    }

    void MovePiece(int x, int y)
    {
        if (_allowedMoves[x, y])
        {
            Piece p = chessboard[x, y];

            if (p != null && p.isWhite != _isWhiteTurn)
            {
                if (p.GetType() == typeof(King))
                {
                    EndGame();
                }

                _pieces.Remove(p.gameObject);
                Destroy(p.gameObject);
                p = null;
            }

            // TODO: Вынести в функцию
            if (_selectedPiece.GetType() == typeof(Pawn))
            {
                if (p == null && y == 7 || y == 0)
                {
                    changePawn?.Invoke();
                    Pause(true);
                    return;
                }

                if (x == EnPassant[0] && y == EnPassant[1])
                {
                    if (_isWhiteTurn)
                    {
                        p = chessboard[x, y - 1];
                    }
                    else
                    {
                        p = chessboard[x, y + 1];
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

            chessboard[_selectedPiece.Position.x, _selectedPiece.Position.y] = null;
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
            chessboard[x, y] = _selectedPiece;
            _isWhiteTurn = !_isWhiteTurn;
            _kingIsAttacked = false;
        }

        //if (_kingIsAttacked)
        //    return;    

        DisableMoveHighligts();
        _selectedPiece = null;
    }

    private void Castling()
    {
        if (_isWhiteTurn)
        {
            if (_selection.x == 6)
            {
                chessboard[5, 0] = chessboard[7, 0];
                chessboard[5, 0].Position = new Vector2Int(5, 0);
                chessboard[5, 0].wasWalking = true;
                chessboard[5, 0].transform.position = CalcSpaceCoords(5, 0);
                chessboard[7, 0] = null;
            }
            else if (_selection.x == 2)
            {
                chessboard[3, 0] = chessboard[0, 0];
                chessboard[3, 0].Position = new Vector2Int(3, 0);
                chessboard[3, 0].wasWalking = true;
                chessboard[3, 0].transform.position = CalcSpaceCoords(3, 0);
                chessboard[0, 0] = null;
            }
        }
        else
        {
            if (_selection.x == 6)
            {
                chessboard[5, 7] = chessboard[7, 7];
                chessboard[5, 7].Position = new Vector2Int(5, 7);
                chessboard[5, 7].wasWalking = true;
                chessboard[5, 7].transform.position = CalcSpaceCoords(5, 7);
                chessboard[7, 7] = null;
            }
            else if (_selection.x == 2)
            {
                chessboard[3, 7] = chessboard[0, 7];
                chessboard[3, 7].Position = new Vector2Int(3, 7);
                chessboard[3, 7].wasWalking = true;
                chessboard[3, 7].transform.position = CalcSpaceCoords(3, 7);
                chessboard[0, 7] = null;
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
                Piece p = chessboard[x, y];

                if (p != null && p.isWhite != _isWhiteTurn)
                {
                    bool[,] attackedSpaces = p.AttackedSpaces();

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

        for (int i = p.Position.x - 1; i <= p.Position.x + 1; i++)
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
        bool[,] pm = piece.PossibleMove();
        Piece tmp;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pm[i, j])
                {
                    tmp = null;
                    if (chessboard[i, j] != null)
                    {
                        tmp = chessboard[i, j];
                    }

                    chessboard[i, j] = piece;
                    chessboard[piece.Position.x, piece.Position.y] = null;
                    CheckKing();

                    if (_kingIsAttacked)
                    {
                        _allowedMoves[i, j] = false;
                    }
                    else
                    {
                        _allowedMoves[i, j] = true;
                    }

                    chessboard[i, j] = tmp;
                    chessboard[piece.Position.x, piece.Position.y] = piece;
                }
            }
        }
    }

    void ChangePawn(int pieceNumber)
    {
        _GameIsPaused = false;

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
        _selectedPiece = chessboard[_selectedPiece.Position.x, _selectedPiece.Position.y];
        // обнуляю клетку, из которой ходит фигрура
        chessboard[_selectedPiece.Position.x, _selectedPiece.Position.y] = null;
        // перемещаю
        _selectedPiece.transform.position = CalcSpaceCoords(_selection.x, _selection.y);
        // присваиваю объекту фигуры новые координаты
        _selectedPiece.Position = new Vector2Int(_selection.x, _selection.y);
        _selectedPiece.wasWalking = true;
        // присваиваю фигуру клетке, в которую походил
        chessboard[_selection.x, _selection.y] = _selectedPiece; 

        _isWhiteTurn = !_isWhiteTurn;

        DisableMoveHighligts();
        _selectedPiece = null; // обнуляю выбранную фигуру

        CheckKing();
    }

    void SpawnPiece(int index, int x, int y)
    {
        GameObject piece = Instantiate(_piecesPrefabs[index], CalcSpaceCoords(x, y), Quaternion.identity);
        piece.transform.SetParent(transform);
        chessboard[x, y] = piece.GetComponent<Piece>();
        chessboard[x, y].Position = new Vector2Int(x, y);
        _pieces.Add(piece);

        if (index == 5)
        {
            _whiteKing = chessboard[x, y];
        }
        else if (index == 11)
        {
            _blackKing = chessboard[x, y];
        }
    }

    void SpawnAllPieces()
    {
        _pieces = new List<GameObject>();
        chessboard = new Piece[8, 8];
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

    private void CreateMoveHighligts()
    {
        _moveHighlights = new GameObject[8, 8];

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                _moveHighlights[i, j] = Instantiate(_moveHighlightPrefab, new Vector3(i, 0.025f, j), Quaternion.identity);
                _moveHighlights[i, j].SetActive(false);
            }
        }
    }

    private void EnableMoveHighligts()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                _moveHighlights[i, j].SetActive(_allowedMoves[i, j]);
            }
        }
    }

    private void DisableMoveHighligts()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                _moveHighlights[i, j].SetActive(false);
            }
        }
    }

    Vector3 CalcSpaceCoords(int x, int y)
    {
        return new Vector3(x, 0f, y);
    }

    private void Pause(bool isPaused)
    {
        _GameIsPaused = isPaused;
    }

    private void RestartGame()
    {
        DestroyAllPieces();
        _GameIsPaused = false;
        _isWhiteTurn = true;
        SpawnAllPieces();
        DisableMoveHighligts();
    }

    private void EndGame()
    {
        DisableMoveHighligts();
        Pause(true);
        win?.Invoke(!_isWhiteTurn);
    }
}
