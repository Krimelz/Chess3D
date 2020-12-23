using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: взятие на проходе

public class BoardController : MonoBehaviour
{
    public static BoardController Instance { get; set; }

    public delegate void DelBool(bool turn);
    public delegate void DelVoid();

    public event DelBool        win;
    public event DelVoid        changePawn;

    [SerializeField]
    private GameObject          _highlihgt;
    [SerializeField]
    private List<GameObject>    _piecesPrefabs;

    [SerializeField]
    private GameObject          _moveHighlightPrefab;
    private GameObject[,]       _moveHighlights = new GameObject[8, 8];
    private bool[,]             _allowedMoves { get; set; }

    private Camera              _camera;
    private Ray                 _ray;
    private RaycastHit          _hit = new RaycastHit();

    public Piece[,]             chessboard = new Piece[8, 8];
    [SerializeField]
    private Vector2Int          _selection;
    private Piece               _selectedPiece;
    private List<GameObject>    _pieces = new List<GameObject>();
    private bool                _isWhiteTurn = true;
    private bool                _GameIsPaused = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _camera = Camera.main;

        GameMenuController.Instance.restartGame += RestartGame;
        GameMenuController.Instance.changePawn += ChangePawn;

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

    void SelectPiece(int x, int y)
    {
        if (chessboard[x, y] == null)
            return;

        if (chessboard[x, y].isWhite != _isWhiteTurn)
            return;

        _allowedMoves = chessboard[x, y].PossibleMove();
        _selectedPiece = chessboard[x, y];

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
                    return;
                }

                _pieces.Remove(p.gameObject);
                Destroy(p.gameObject);
            }

            if (_selectedPiece.GetType() == typeof(Pawn) && p == null && y == 7 || y == 0)
            {
                changePawn?.Invoke();
                _GameIsPaused = true;
                return;
            }

            chessboard[_selectedPiece.Position.x, _selectedPiece.Position.y] = null;
            _selectedPiece.transform.position = CalcSpaceCoords(x, y);
            _selectedPiece.Position = new Vector2Int(x, y);
            chessboard[x, y] = _selectedPiece;
            _isWhiteTurn = !_isWhiteTurn;
        }

        DisableMoveHighligts();

        _selectedPiece = null;
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
        // присваиваю фигуру клетке, в которую походил
        chessboard[_selection.x, _selection.y] = _selectedPiece; 

        _isWhiteTurn = !_isWhiteTurn;

        DisableMoveHighligts();
        _selectedPiece = null; // обнуляю выбранную фигуру
    }

    void SpawnPiece(int index, int x, int y)
    {
        GameObject piece = Instantiate(_piecesPrefabs[index], CalcSpaceCoords(x, y), Quaternion.identity);
        piece.transform.SetParent(transform);
        chessboard[x, y] = piece.GetComponent<Piece>();
        chessboard[x, y].Position = new Vector2Int(x, y);
        _pieces.Add(piece);
    }

    void SpawnAllPieces()
    {
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
        _GameIsPaused = true;
        win?.Invoke(_isWhiteTurn);
    }
}
