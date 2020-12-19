using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public GameObject           highlihgt;
    public List<GameObject>     piecesPrefabs;

    public GameObject moveHighlightPrefab;
    private GameObject[,] _moveHighlights = new GameObject[8, 8];

    private Camera              _camera;
    private Ray                 _ray;
    private RaycastHit          _hit = new RaycastHit();

    [SerializeField]
    private Vector2Int          _selection;
    private Piece               _selectedPiece;
    private List<GameObject>    _pieces = new List<GameObject>();
    private Piece[,]            _chessboard = new Piece[8, 8];
    private bool                _isWhiteTurn = true;

    void Start()
    {
        _camera = Camera.main;

        SpawnAllPieces();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                _moveHighlights[i, j] = Instantiate(moveHighlightPrefab, new Vector3(i, 0.05f, j), Quaternion.identity);
                _moveHighlights[i, j].SetActive(false);
            }
        }
    }

    void Update()
    {
        UpdateSelection();

        if (Input.GetMouseButtonDown(0))
        {
            if (highlihgt.activeSelf)
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
            highlihgt.SetActive(true);
            _selection = new Vector2Int(Mathf.RoundToInt(_hit.point.x), Mathf.RoundToInt(_hit.point.z));
            highlihgt.transform.position = new Vector3(_selection.x, 0.025f, _selection.y);
        }
        else
        {
            highlihgt.SetActive(false);
        }
    }

    void SelectPiece(int x, int y)
    {
        if (_chessboard[x, y] == null)
            return;

        if (_chessboard[x, y].isWhite != _isWhiteTurn)
            return;

        _selectedPiece = _chessboard[x, y];

        var moves = _selectedPiece.PossibleMove();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                _moveHighlights[i, j].SetActive(moves[i, j]);
            }
        }
    }

    void MovePiece(int x, int y)
    {
        var moves = _selectedPiece.PossibleMove();

        if (moves[x, y])
        {
            _chessboard[_selectedPiece.Position.x, _selectedPiece.Position.y] = null;
            _selectedPiece.transform.position = CalcSpaceCoords(x, y);
            _selectedPiece.Position = new Vector2Int(x, y);
            _chessboard[x, y] = _selectedPiece;
            _isWhiteTurn = !_isWhiteTurn;
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                _moveHighlights[i, j].SetActive(false);
            }
        }

        _selectedPiece = null;
    }

    void SpawnPiece(int index, int x, int y)
    {
        GameObject piece = Instantiate(piecesPrefabs[index], CalcSpaceCoords(x, y), Quaternion.identity);
        piece.transform.SetParent(transform);
        _chessboard[x, y] = piece.GetComponent<Piece>();
        _chessboard[x, y].Position = new Vector2Int(x, y);
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

    Vector3 CalcSpaceCoords(int x, int y)
    {
        return new Vector3(x, 0f, y);
    }
}
