namespace AsteroidsGame.CompositionRoot
{
    using AsteroidsGame.Contracts;
    using AsteroidsGame.Logic;
    using AsteroidsGame.Presentation;
    using UnityEngine;

    public class CompositionRoot : MonoBehaviour
    {
        private IEcsBootstrap _bootstrap;
        private IEcsRunner _runner;

        private IGamePresenter _gamePresenter;
        private IShipUiPresenter _shipUiPresenter;
        private IGameStatePresenter _gameStatePresenter;

        public IGameStatePresenter GameStatePresenter => _gameStatePresenter;

        private IInputReader _inputReader;

        [SerializeField] private UnityGlobalConfigService _unityGlobalConfigService;
        [SerializeField] private UnityGameView _gameView;
        [SerializeField] private UnityShipUiView _shipUiView;

        private float _cachedW, _cachedH;
        private Camera _camera;
        private const float Epsilon = 0.01f;

        private bool _isPaused = true;

        private void Awake()
        {
            Init();
            _camera = Camera.main;
        }

        private void Init()
        {
            _cachedW = 0;
            _cachedH = 0;

            _bootstrap = new EcsBootstrap(_unityGlobalConfigService);
            _bootstrap.Init();

            _inputReader = new UnityInputReader();
            _runner = new EcsRunner(_bootstrap.Systems, _inputReader);

            _gamePresenter = new EcsGamePresenter(_bootstrap.World, _gameView);
            _shipUiPresenter = new EcsShipUiPresenter(_bootstrap.World, _shipUiView,
                _unityGlobalConfigService.LaserConfig.ShotCooldown);

            _gameStatePresenter = new GameStatePresenter(_bootstrap.Systems);
        }

        public void TogglePause(bool isPaused)
        {
            _isPaused = isPaused;
        }

        private void Update()
        {
            if (_isPaused) return;

            TryUpdateScreenSize();
            _runner.Tick(Time.deltaTime);
            _gamePresenter.UpdateGame();
            _shipUiPresenter.UpdateUI();
            _gameStatePresenter.UpdateState();
        }


        private void TryUpdateScreenSize()
        {
            var h = 2f * _camera.orthographicSize;
            var w = h * _camera.aspect;

            if (Mathf.Abs(w - _cachedW) < Epsilon && Mathf.Abs(h - _cachedH) < Epsilon)
                return;

            _cachedW = w;
            _cachedH = h;

            _runner.UpdateScreenSize(w, h);
        }

        private void OnDestroy()
        {
            _inputReader.Disable();
            _inputReader = null;
            if (_bootstrap == null) return;
            _bootstrap.Destroy();
            _bootstrap = null;
        }

        public void RestartGame()
        {
            _bootstrap.Destroy();
            _gameView.Clear();

            Init();
        }
    }
}