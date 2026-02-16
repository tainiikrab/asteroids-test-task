namespace AsteroidsGame.Logic
{
    using System;
    using Leopotam.EcsProto;
    using Contracts;
    using System.Diagnostics;

    public sealed class AsteroidSpawnSystem : IProtoInitSystem, IProtoRunSystem
    {
        private IConfigService _configService;
        private IDeltaTimeService _deltaTimeService;
        private IAsteroidSpawnService _asteroidSpawnService;
        private IGameViewSizeService _viewSizeService;
        private IRandomService _randomService;

        private float _spawnTimer;

        public void Init(IProtoSystems systems)
        {
            var src = systems.Services();
            _deltaTimeService = src[typeof(IDeltaTimeService)] as IDeltaTimeService;
            _configService = src[typeof(IConfigService)] as IConfigService;
            _asteroidSpawnService = src[typeof(IAsteroidSpawnService)] as IAsteroidSpawnService;
            _viewSizeService = src[typeof(IGameViewSizeService)] as IGameViewSizeService;
            _randomService = src[typeof(IRandomService)] as IRandomService;

            _spawnTimer = _configService.AsteroidConfig.SpawnInterval;
        }

        public void Run()
        {
            var spawnInterval = _configService.AsteroidConfig.SpawnInterval;
            var spawnAmount = _configService.AsteroidConfig.SpawnAmount;

            if (spawnInterval == 0 || spawnAmount == 0)
                return;

            _spawnTimer += _deltaTimeService.DeltaTime;

            if (_spawnTimer < spawnInterval)
                return;

            _spawnTimer = 0;

            for (var i = 0; i < spawnAmount; i++)
            {
                var (x, y) = CalculateRandomSpawnPosition();
                _asteroidSpawnService.SpawnAsteroid(x, y);
            }
        }

        private (float, float) CalculateRandomSpawnPosition()
        {
            var halfWidth = _viewSizeService.HalfWidth;
            var halfHeight = _viewSizeService.HalfHeight;
            var margin = _configService.WorldConfig.ScreenWrapMargin;

            var horizontal = _randomService.NextFloat < 0.5;

            float x, y;

            if (horizontal)
            {
                var signX = _randomService.RandomSign;
                x = signX * (halfWidth + margin);

                var t = _randomService.NextFloat;
                y = -halfHeight + t * (halfHeight - -halfHeight);
            }
            else
            {
                var signY = _randomService.RandomSign;
                y = signY * (halfHeight + margin);

                var t = _randomService.NextFloat;
                x = -halfWidth + t * (halfWidth - -halfWidth);
            }

            return (x, y);
        }
    }
}