using System;
using ChessBoard;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class GameMenu : MonoBehaviour
    {
        public static GameMenu Instance { get; private set; }
        
        [SerializeField] private GameObject gameMenu;
        [SerializeField] private GameObject changePawn;
        [SerializeField] private Text winText;

        [SerializeField] private Button knight;
        [SerializeField] private Button rook;
        [SerializeField] private Button bishop;
        [SerializeField] private Button queen;
    
        public event Action OnRestartGame;
        public event Action<bool> OnGamePaused;
        public event Action<PieceType> OnChangePawn;

        private bool _pause;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Board.Instance.OnWin += PrintWinner;
            Board.Instance.OnChangePawn += EnableChangePawn;
            
            knight.onClick.AddListener(() => GetNewPiece(PieceType.Knight));
            rook.onClick.AddListener(() => GetNewPiece(PieceType.Rook));
            bishop.onClick.AddListener(() => GetNewPiece(PieceType.Bishop));
            queen.onClick.AddListener(() => GetNewPiece(PieceType.Queen));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _pause = !_pause;
                gameMenu.SetActive(_pause);
                OnGamePaused?.Invoke(_pause);
            }
        }

        private void EnableChangePawn()
        {
            changePawn.SetActive(true);
        }

        private void GetNewPiece(PieceType type)
        {
            OnChangePawn?.Invoke(type);
            changePawn.SetActive(false);
        }

        private void PrintWinner(Team turn)
        {
            gameMenu.SetActive(true);
            winText.text = $"{turn.ToString().ToUpper()} WINS";
        }

        public void RestartGame()
        {
            winText.text = "GAME MENU";
            gameMenu.SetActive(false);
            OnRestartGame?.Invoke();
        }

        public void ExitToMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
