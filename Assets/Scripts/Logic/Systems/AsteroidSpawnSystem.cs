using System;
using Leopotam.EcsProto;
using AsteroidsGame.Contracts;

namespace AsteroidsGame.Logic
{
    public sealed class AsteroidSpawnSystem : IProtoInitSystem, IProtoRunSystem
    {
        private ProtoWorld _world;
        private ProtoIt _iterator;


        private EntityAspect _entityAspect;
        private PositionAspect _positionAspect;

        private readonly Random _random = new();

        private IDeltaTimeService _deltaTimeService;
        private IIdGeneratorService _idGeneratorService;
        private IConfigService _configService;
        private float DeltaTime => _deltaTimeService.DeltaTime;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();

            var svc = systems.Services();
            _deltaTimeService = svc[typeof(IDeltaTimeService)] as IDeltaTimeService;
            _idGeneratorService = svc[typeof(IIdGeneratorService)] as IIdGeneratorService;
            _configService = svc[typeof(IConfigService)] as IConfigService;


            _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));
            _positionAspect = (PositionAspect)_world.Aspect(typeof(PositionAspect));
            // _iterator.Init(_world);
        }

        private float RandomSign => _random.Next(0, 2) * 2f - 1f;
        private float RandomNormalizedFloat => ((float)_random.NextDouble() - 0.5f) * 2f;

        private float _spawnTimer = 0;

        public void Run()
        {
            var baseSpeed = _configService.AsteroidSpeed;
            var rotationSpeed = _configService.AsteroidRotationSpeed;
            var randomnessWeight = _configService.AsteroidRandomnessWeight;
            var spawnInterval = _configService.AsteroidSpawnInterval;
            var spawnAmount = _configService.AsteroidSpawnAmount;

            _spawnTimer += DeltaTime;

            if (_spawnTimer >= spawnInterval)
            {
                _spawnTimer = 0;
                for (var i = 0; i < spawnAmount; i++)
                {
                    ref var _ = ref _entityAspect.AsteroidPool.NewEntity(out var asteroidEntity);

                    ref var positionData = ref _positionAspect.PositionPool.Add(asteroidEntity);
                    positionData.x = RandomSign * RandomNormalizedFloat * 3f;
                    positionData.y = RandomSign * RandomNormalizedFloat * 3f;

                    ref var rotationData = ref _positionAspect.RotationPool.Add(asteroidEntity);
                    var angle = (float)_random.NextDouble() * 360f;
                    rotationData.angle = angle;

                    var dirX = MathF.Cos(angle);
                    var dirY = MathF.Sin(angle);
                    ref var velocityData = ref _positionAspect.VelocityPool.Add(asteroidEntity);

                    var finalSpeed = baseSpeed * (1 + RandomNormalizedFloat * randomnessWeight);
                    velocityData.vx = dirX * finalSpeed;
                    velocityData.vy = dirY * finalSpeed;

                    velocityData.deceleration = 0f;

                    ref var angularVelocityData = ref _positionAspect.AngularVelocityPool.Add(asteroidEntity);
                    angularVelocityData.omega = rotationSpeed * (1 + RandomNormalizedFloat * randomnessWeight);

                    ref var entityIdData = ref _entityAspect.EntityIdPool.Add(asteroidEntity);
                    entityIdData.type = EntityType.Asteroid;
                    entityIdData.id = _idGeneratorService.Next();
                }
            }
        }
    }
}