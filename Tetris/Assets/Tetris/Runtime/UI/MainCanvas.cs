using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Tetris.UI
{
    public class MainCanvas : MonoBehaviour
    {
        [SerializeField]
        Core.GameManager _gameManager;
        [SerializeField]
        UnityEngine.UI.Button _restartButton;
        [SerializeField]
        TMP_Text _scoreText;

        void Start()
        {
            _gameManager.StartGame();
        }

        void Update()
        {
            _restartButton.gameObject.SetActive(!_gameManager.IsPlaying);
            UpdateScoreText();
        }

        void UpdateScoreText()
        {
            _scoreText.text = $"Score: {_gameManager.Score} / Level: {_gameManager.Level}";
        }

        public void OnClickedRestartButton()
        {
            _gameManager.StartGame();
        }
    }
}
