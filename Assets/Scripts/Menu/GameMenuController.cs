using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour
{
    public static GameMenuController Instance { get; set; }
    [SerializeField]
    private GameObject              _gameMenu;
    [SerializeField]
    private GameObject              _newPieces;
    [SerializeField]
    private Text                    _winText;

    public delegate void DelVoid();
    public delegate void DelBool(bool isPaused);
    public delegate void DelInt(int pieceNumber);
    public event DelVoid restartGame;
    public event DelBool gamePause;
    public event DelInt changePawn;

    private bool _pause = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        BoardController.Instance.win += PrintWinner;
        BoardController.Instance.changePawn += EnableChangePawn;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _pause = !_pause;
            _gameMenu.SetActive(_pause);
            gamePause.Invoke(_pause);
        }
    }

    public void EnableChangePawn()
    {
        _newPieces.SetActive(true);
    }

    public void GetNewPieceNumber(int pieceNumber)
    {
        changePawn?.Invoke(pieceNumber);
        _newPieces.SetActive(false);
    }

    public void PrintWinner(bool turn)
    {
        _gameMenu.SetActive(true);

        if (turn)
        {
            _winText.text = "WHITE WINS";
        }
        else
        {
            _winText.text = "BLACK WINS";
        }
    }

    public void RestartGame()
    {
        _gameMenu.SetActive(false);
        restartGame?.Invoke();
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
