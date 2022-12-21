using System;
using System.Collections.Generic;
using Menu;
using Pieces;
using Tweens;
using UnityEngine;

namespace ChessBoard
{
    public class Board : MonoBehaviour
    {
        public static Board Instance { get; private set; }

        public event Action<Team> OnWin;
        public event Action OnChangePawn;

        [SerializeField] private Tween tween;
        [SerializeField] private AnimationCurve movementCurve;
        [SerializeField] private AnimationCurve rotationCurve;
        [SerializeField] private float pieceMoveDuration;
        [SerializeField] private float cameraMoveDuration;
        [SerializeField] private GameObject highlight;
        [SerializeField] private GameObject[] whitePiecesPrefabs;
        [SerializeField] private GameObject[] blackPiecesPrefabs;
        [SerializeField] private Transform whiteView;
        [SerializeField] private Transform blackView;
        [SerializeField] private Transform target;

        public int[] EnPassant { get; set; }
        public bool EnPassantColor { get; set; }
        [SerializeField] private GameObject _moveHighlightPrefab;
        private GameObject[,] _moveHighlights;
        private bool[,] _allowedMoves { get; set; }
        private bool[,] _allSpacesUnderAttack { get; set; }

        [SerializeField] private Camera _camera;
        private Ray _ray;
        private RaycastHit _hit;

        public Piece[,] chessboard;
        private Vector2Int _selection;
        private Piece _selectedPiece;
        private List<GameObject> _pieces;
        private Team _teamTurn = Team.White;
        private bool _gameIsPaused;

        private Piece _whiteKing;
        private Piece _blackKing;
        private bool _kingIsAttacked;

        private bool _isMoving;
        private Vector2Int _prevSelection;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _camera = Camera.main;
            
            if (_camera != null)
            {
                _camera.transform.position = whiteView.position;
                _camera.transform.LookAt(target);
            }

            GameMenu.Instance.OnRestartGame += RestartGame;
            GameMenu.Instance.OnChangePawn += ChangePawn;
            GameMenu.Instance.OnGamePaused += Pause;

            SpawnAllPieces();
            CreateMoveHighlights();
        }

        private void Update()
        {
            if (_gameIsPaused)
                return;

            if (!_isMoving)
                UpdateSelection();
            
            if (Input.GetMouseButton(0) && !_isMoving)
            {
                if (highlight.activeSelf)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (_selectedPiece == null)
                        {
                            SelectPiece(_selection.x, _selection.y);
                        }
                        else
                        {
                            if (_allowedMoves[_selection.x, _selection.y])
                            {
                                var endPosition = new Vector3(_selection.x, 0f, _selection.y);

                                tween.MoveToPosition(
                                    _selectedPiece.transform,
                                    endPosition,
                                    pieceMoveDuration,
                                    movementCurve,
                                    OnMoveStarted,
                                    OnMoveCompleted
                                );
                            }
                        }

                        return;
                    }

                    if (_selectedPiece != null && _allowedMoves[_selection.x, _selection.y] && IsDragged())
                    {
                        var endPosition = new Vector3(_selection.x, 0.35f, _selection.y);
                        
                        tween.MoveToPosition(
                            _selectedPiece.transform,
                            endPosition,
                            pieceMoveDuration,
                            movementCurve,
                            () => _prevSelection = _selection
                        );
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                var endPosition = new Vector3(_selection.x, 0f, _selection.y);
                
                if (_selectedPiece != null && _allowedMoves[_selection.x, _selection.y] && !_isMoving)
                {
                    tween.MoveToPosition(
                        _selectedPiece.transform,
                        endPosition,
                        pieceMoveDuration,
                        movementCurve,
                        OnMoveStarted,
                        OnMoveCompleted
                    );
                }
            }
        }

        private bool IsDragged()
        {
            var delta = _selection - _prevSelection;
            return delta != Vector2Int.zero;
        }

        private void OnMoveStarted()
        {
            _isMoving = true;
        }

        private void OnMoveCompleted()
        {
            MovePiece(_selection.x, _selection.y);
            CheckKing();
            IsCheckmate();
            RotateBoardToTeam(_teamTurn);
        }

        private void RotateBoardToTeam(Team team)
        {
            var endPosition = team == Team.White ? whiteView.position : blackView.position;
            
            tween.RotateAroundPoint(_camera.transform, endPosition, target.position,
                cameraMoveDuration, rotationCurve, onCompleted: () => _isMoving = false);
        }

        private void UpdateSelection()
        {
            _ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_ray, out _hit))
            {
                highlight.SetActive(true);
                _selection = new Vector2Int(Mathf.RoundToInt(_hit.point.x), Mathf.RoundToInt(_hit.point.z));
                highlight.transform.position = new Vector3(_selection.x, 0.015f, _selection.y);
            }
            else
            {
                highlight.SetActive(false);
            }
        }

        private bool HasMoves()
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

        private void IsCheckmate()
        {
            bool hasMoves = false;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Piece p = chessboard[i, j];

                    if (p != null && p.team == _teamTurn)
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
            {
                EndGame();
            }
        }

        private void SelectPiece(int x, int y)
        {
            if (chessboard[x, y] == null)
                return;

            if (chessboard[x, y].team != _teamTurn)
                return;

            _selectedPiece = chessboard[x, y];

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


            if (!HasMoves())
            {
                _selectedPiece = null;
                return;
            }

            EnableMoveHighlights();
        }

        private void MovePiece(int x, int y)
        {
            if (_allowedMoves[x, y])
            {
                Piece p = chessboard[x, y];

                if (p != null && p.team != _teamTurn)
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
                        OnChangePawn?.Invoke();
                        Pause(true);
                        return;
                    }

                    if (x == EnPassant[0] && y == EnPassant[1])
                    {
                        if (_teamTurn == Team.White)
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

                    if (_teamTurn == Team.White)
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
                _teamTurn = _teamTurn == Team.White ? Team.Black : Team.White;
                _kingIsAttacked = false;
            }

            DisableMoveHighlights();
            _selectedPiece = null;
        }

        private void Castling()
        {
            if (_teamTurn == Team.White && !_kingIsAttacked)
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

                    if (p != null && p.team != _teamTurn)
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

            if (_teamTurn == Team.White)
            {
                _kingIsAttacked = _allSpacesUnderAttack[_whiteKing.Position.x, _whiteKing.Position.y];
            }
            else
            {
                _kingIsAttacked = _allSpacesUnderAttack[_blackKing.Position.x, _blackKing.Position.y];
            }
        }

        private void KingMoves()
        {
            Piece p;

            if (_teamTurn == Team.White)
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

        private void ChangePawn(PieceType type)
        {
            _gameIsPaused = false;

            _pieces.Remove(_selectedPiece.gameObject);
            SpawnPiece(_teamTurn, type, _selectedPiece.Position.x, _selectedPiece.Position.y);
            Destroy(_selectedPiece.gameObject);

            _selectedPiece = chessboard[_selectedPiece.Position.x, _selectedPiece.Position.y];
            chessboard[_selectedPiece.Position.x, _selectedPiece.Position.y] = null;
            _selectedPiece.transform.position = CalcSpaceCoords(_selection.x, _selection.y);
            _selectedPiece.Position = new Vector2Int(_selection.x, _selection.y);
            _selectedPiece.wasWalking = true;
            chessboard[_selection.x, _selection.y] = _selectedPiece;

            _teamTurn = _teamTurn == Team.White ? Team.Black : Team.White;

            DisableMoveHighlights();
            _selectedPiece = null;

            CheckKing();
            IsCheckmate();
            RotateBoardToTeam(_teamTurn);
        }

        private void SpawnPiece(Team team, PieceType type, int x, int y)
        {
            GameObject piece;

            if (team == Team.White)
            {
                piece = Instantiate(whitePiecesPrefabs[(int)type], CalcSpaceCoords(x, y), Quaternion.identity);
            }
            else
            {
                piece = Instantiate(blackPiecesPrefabs[(int)type], CalcSpaceCoords(x, y), Quaternion.identity);
            }

            piece.transform.SetParent(transform);
            chessboard[x, y] = piece.GetComponent<Piece>();
            chessboard[x, y].Position = new Vector2Int(x, y);
            _pieces.Add(piece);

            if (team == Team.White && type == PieceType.King)
            {
                _whiteKing = chessboard[x, y];
            }
            else if (team == Team.Black && type == PieceType.King)
            {
                _blackKing = chessboard[x, y];
            }
        }

        private void SpawnAllPieces()
        {
            _pieces = new List<GameObject>();
            chessboard = new Piece[8, 8];
            EnPassant = new[] { -1, -1 };

            for (int i = 0; i < 8; i++)
            {
                SpawnPiece(Team.White, PieceType.Pawn, i, 1);
            }

            SpawnPiece(Team.White, PieceType.Knight, 1, 0);
            SpawnPiece(Team.White, PieceType.Knight, 6, 0);
            SpawnPiece(Team.White, PieceType.Rook, 0, 0);
            SpawnPiece(Team.White, PieceType.Rook, 7, 0);
            SpawnPiece(Team.White, PieceType.Bishop, 2, 0);
            SpawnPiece(Team.White, PieceType.Bishop, 5, 0);
            SpawnPiece(Team.White, PieceType.Queen, 3, 0);
            SpawnPiece(Team.White, PieceType.King, 4, 0);

            for (int i = 0; i < 8; i++)
            {
                SpawnPiece(Team.Black, PieceType.Pawn, i, 6);
            }

            SpawnPiece(Team.Black, PieceType.Knight, 1, 7);
            SpawnPiece(Team.Black, PieceType.Knight, 6, 7);
            SpawnPiece(Team.Black, PieceType.Rook, 0, 7);
            SpawnPiece(Team.Black, PieceType.Rook, 7, 7);
            SpawnPiece(Team.Black, PieceType.Bishop, 2, 7);
            SpawnPiece(Team.Black, PieceType.Bishop, 5, 7);
            SpawnPiece(Team.Black, PieceType.Queen, 3, 7);
            SpawnPiece(Team.Black, PieceType.King, 4, 7);
        }

        private void DestroyAllPieces()
        {
            foreach (GameObject p in _pieces)
            {
                Destroy(p.gameObject);
            }
        }

        private void CreateMoveHighlights()
        {
            _moveHighlights = new GameObject[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    _moveHighlights[i, j] =
                        Instantiate(_moveHighlightPrefab, new Vector3(i, 0.025f, j), Quaternion.identity);
                    _moveHighlights[i, j].SetActive(false);
                }
            }
        }

        private void EnableMoveHighlights()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    _moveHighlights[i, j].SetActive(_allowedMoves[i, j]);
                }
            }
        }

        private void DisableMoveHighlights()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    _moveHighlights[i, j].SetActive(false);
                }
            }
        }

        private Vector3 CalcSpaceCoords(int x, int y)
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
            _teamTurn = Team.White;
            SpawnAllPieces();
            DisableMoveHighlights();
            RotateBoardToTeam(_teamTurn);
        }

        private void EndGame()
        {
            DisableMoveHighlights();
            Pause(true);
            Team winner = _teamTurn == Team.White ? Team.Black : Team.White;
            RotateBoardToTeam(winner);
            OnWin?.Invoke(winner);
        }
    }
}