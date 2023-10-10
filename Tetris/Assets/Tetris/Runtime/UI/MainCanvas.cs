using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
        [SerializeField]
        RectTransform[] _touchUI;
        [SerializeField]
        ToggleButton _toggleButton;

        void Start()
        {
            // キーボードが接続されているかを検出
            InputDevice keyboard = InputSystem.GetDevice<Keyboard>();
            bool isKeyboardConnected = (keyboard != null);
            _toggleButton.SetToggle(!isKeyboardConnected);
            TriggerTouchUI(!isKeyboardConnected);

            _gameManager.StartGame();
        }

        void TriggerTouchUI(bool isOn)
        {
            foreach (RectTransform ui in _touchUI)
            {
                ui.gameObject.SetActive(isOn);
            }
        }

        void Update()
        {
            _restartButton.gameObject.SetActive(!_gameManager.IsPlaying);
            UpdateScoreText();

            TriggerTouchUI(_toggleButton.IsOn);
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
