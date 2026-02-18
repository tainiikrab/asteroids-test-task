using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AsteroidsGame.Presentation
{
    public class SessionUIController : MonoBehaviour
    {
        public event Action OnGameStartedEvent;
        public event Action OnGameRestartedEvent;

        [SerializeField] private RectTransform _titlePanel;
        [SerializeField] private RectTransform _gameOverPanel;

        [SerializeField] private Button _startButton;
        [SerializeField] private Button _restartButton;

        [SerializeField] private TextMeshProUGUI _scoreLabel;

        private void Awake()
        {
            _startButton.onClick.AddListener(OnStartButtonClicked);
            _restartButton.onClick.AddListener(OnRestartButtonClicked);

            _titlePanel.gameObject.SetActive(true);
            _gameOverPanel.gameObject.SetActive(false);
        }

        public void OnGameOver(int score)
        {
            _gameOverPanel.gameObject.SetActive(true);
            _scoreLabel.SetText($"Score: {score}");
        }

        private void OnStartButtonClicked()
        {
            OnGameStartedEvent?.Invoke();
            _titlePanel.gameObject.SetActive(false);
        }

        private void OnRestartButtonClicked()
        {
            OnGameRestartedEvent?.Invoke();
            _gameOverPanel.gameObject.SetActive(false);
        }
    }
}