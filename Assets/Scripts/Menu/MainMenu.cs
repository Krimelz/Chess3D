using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class MainMenu : MonoBehaviour
    {
        public string gameScene;
        public Button start;
        public Button exit;

        private void Start()
        {
            start.onClick.AddListener(StartGame);
            exit.onClick.AddListener(ExitGame);
        }

        private void StartGame()
        {
            SceneManager.LoadScene(gameScene);
        }

        private void ExitGame()
        {
            Application.Quit();
        }
    }
}
