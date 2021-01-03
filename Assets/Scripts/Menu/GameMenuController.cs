using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject              _gameMenu;
    [SerializeField]
    private GameObject              _newPieces;
    [SerializeField]
    private Text                    _winText;

    private bool _pause = false;

    void Start()
    {
        EventManager<bool>.AddEvent("PrintWinner", PrintWinner);
        EventManager.AddEvent("EnableChangePawn", EnableChangePawn);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _pause = !_pause;
            _gameMenu.SetActive(_pause);
            EventManager<bool>.Broadcast("Pause", _pause);
        }
    }

    public void EnableChangePawn()
    {
        _newPieces.SetActive(true);
    }

    public void DisableChangePawn()
    {
        _newPieces.SetActive(false);
    }

    public void EnableGameMenu()
    {
        _gameMenu.SetActive(true);
    }

    public void DisableGameMenu()
    {
        _gameMenu.SetActive(false);
    }

    public void GetNewPieceNumber(int pieceNumber)
    {
        EventManager<int>.Broadcast("ChangePawn", pieceNumber);
        DisableChangePawn();
    }

    public void PrintWinner(bool isWhiteTurn)
    {
        EnableGameMenu();

        if (isWhiteTurn)
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
        _winText.text = "GAME MENU";
        DisableGameMenu();
        EventManager.Broadcast("RestartGame");
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
