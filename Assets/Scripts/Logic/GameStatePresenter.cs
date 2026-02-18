namespace AsteroidsGame.Logic
{
    using System;
    using Leopotam.EcsProto;
    using System.Diagnostics;

    public sealed class GameStatePresenter : IGameStatePresenter
    {
        private readonly ProtoIt _playerIterator;
        private IScoreService _scoreService;
        private bool _isGameOver;
        private int _score;

        public bool IsGameOver => _isGameOver;
        public int Score => _score;

        public event Action<int> OnGameOverEvent;

        public GameStatePresenter(IProtoSystems systems)
        {
            _playerIterator = new ProtoIt(new[] { typeof(PlayerCmp) });
            _playerIterator.Init(systems.World());

            _scoreService = systems.Services()[typeof(IScoreService)] as IScoreService;
        }

        public void UpdateState()
        {
            if (_isGameOver)
                return;

            foreach (var _ in _playerIterator)
                return;

            _isGameOver = true;
            OnGameOverEvent?.Invoke(_scoreService.currentScore);
        }
    }

    public interface IGameStatePresenter
    {
        bool IsGameOver { get; }
        int Score { get; }

        event Action<int> OnGameOverEvent;

        void UpdateState();
    }
}