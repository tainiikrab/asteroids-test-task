namespace AsteroidsGame.Logic
{
    using System;
    using Leopotam.EcsProto;
    using System.Diagnostics;

    public sealed class GameStatePresenter : IGameStatePresenter
    {
        private readonly ProtoIt _playerIterator;

        private bool _isGameOver;
        private int _score;

        public bool IsGameOver => _isGameOver;
        public int Score => _score;

        public event Action<int> OnGameOverEvent;

        public GameStatePresenter(ProtoWorld world)
        {
            _playerIterator = new ProtoIt(new[]
            {
                typeof(PlayerCmp)
            });
            _playerIterator.Init(world);
        }

        public void SetScore(int score)
        {
            _score = score;
        }

        public void UpdateState()
        {
            if (_isGameOver)
                return;

            foreach (var _ in _playerIterator)
                return;

            _isGameOver = true;
            OnGameOverEvent?.Invoke(_score);
        }
    }

    public interface IGameStatePresenter
    {
        bool IsGameOver { get; }
        int Score { get; }

        event Action<int> OnGameOverEvent;

        void SetScore(int score);
        void UpdateState();
    }
}