using Shooter.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shooter.UI
{
    public class UIControl : MonoBehaviour
    {
        [SerializeField] GameObject gameOverScreen;
        void Start()
        {
            FindObjectOfType<Player>().OnDeath += OnGameOver;
        }
        void OnGameOver()
        {
            gameOverScreen.SetActive(true);
        }



        //UI input section
        public void StartNewGame()
        {
            SceneManager.LoadScene("Game");
        }
    }
}
