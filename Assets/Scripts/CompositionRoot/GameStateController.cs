using AsteroidsGame.Logic;

namespace AsteroidsGame.CompositionRoot
{
    using AsteroidsGame.Presentation;
    using UnityEngine;

    public sealed class GameStateController : MonoBehaviour
    {
        [SerializeField] private SessionUIController _sessionUI;
        [SerializeField] private CompositionRoot _compositionRoot;
        private IGameStatePresenter _gameStatePresenter;

        private void Awake()
        {
            _sessionUI.OnGameStartedEvent += OnGameStarted;
            _sessionUI.OnGameRestartedEvent += OnGameRestarted;
            _compositionRoot.TogglePause(true);
        }

        private void OnGameStarted()
        {
            _compositionRoot.TogglePause(false);

            _gameStatePresenter = _compositionRoot.GameStatePresenter;
            _gameStatePresenter.OnGameOverEvent += OnGameOverEvent;
        }


        private void OnGameOverEvent(int score)
        {
            _sessionUI.OnGameOver(score);
        }


        private void OnGameRestarted()
        {
            _compositionRoot.TogglePause(true);

            if (_gameStatePresenter != null)
                _gameStatePresenter.OnGameOverEvent -= OnGameOverEvent;

            _compositionRoot.RestartGame();
            _gameStatePresenter = _compositionRoot.GameStatePresenter;
            _gameStatePresenter.OnGameOverEvent += OnGameOverEvent;
            _compositionRoot.TogglePause(false);
        }

        private void OnDestroy()
        {
            _sessionUI.OnGameStartedEvent -= OnGameStarted;
            _sessionUI.OnGameRestartedEvent -= OnGameRestarted;
            _gameStatePresenter.OnGameOverEvent -= OnGameOverEvent;
        }
    }
}