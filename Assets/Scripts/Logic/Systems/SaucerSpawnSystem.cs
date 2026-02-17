using Leopotam.EcsProto.QoL;

namespace AsteroidsGame.Logic
{
    using System;
    using Leopotam.EcsProto;
    using Contracts;
    using System.Diagnostics;

    public sealed class SaucerSpawnSystem : IProtoInitSystem, IProtoRunSystem
    {
        private IConfigService _configService;
        private IDeltaTimeService _deltaTimeService;
        private IObstacleSpawnService _obstacleSpawnService;
        private IRandomService _randomService;

        private ProtoWorld _world;

        private float _spawnTimer;
        private IProtoIt _playerIterator;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();
            var src = systems.Services();
            _deltaTimeService = src[typeof(IDeltaTimeService)] as IDeltaTimeService;
            _configService = src[typeof(IConfigService)] as IConfigService;
            _obstacleSpawnService = src[typeof(IObstacleSpawnService)] as IObstacleSpawnService;
            _randomService = src[typeof(IRandomService)] as IRandomService;

            _playerIterator = new ProtoIt(new[] { typeof(PlayerCmp) });
            _playerIterator.Init(_world);

            // _spawnTimer = _configService.SaucerConfig.SpawnInterval;
        }

        public void Run()
        {
            var spawnInterval = _configService.SaucerConfig.SpawnInterval;
            var spawnAmount = _configService.SaucerConfig.SpawnAmount;


            if (spawnInterval == 0 || spawnAmount == 0)
                return;

            _spawnTimer += _deltaTimeService.DeltaTime;

            if (_spawnTimer < spawnInterval)
                return;

            _spawnTimer = 0;
            ProtoPackedEntity packedPlayerEntity = default;
            var foundPlayer = false;
            foreach (var playerEntity in _playerIterator)
            {
                packedPlayerEntity = _world.PackEntity(playerEntity);
                foundPlayer = true;
            }

            if (!foundPlayer)
                return;


            for (var i = 0; i < spawnAmount; i++)
            {
                var (x, y) = _randomService.CalculateRandomSpawnPosition();
                _obstacleSpawnService.SpawnSaucer(x, y, packedPlayerEntity);
            }
        }
    }
}