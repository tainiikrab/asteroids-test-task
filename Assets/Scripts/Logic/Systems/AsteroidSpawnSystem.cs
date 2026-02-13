namespace AsteroidsGame.Logic
{
    using System;
    using Leopotam.EcsProto;
    using AsteroidsGame.Contracts;
    
    using System.Diagnostics;

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
        private IGameViewSizeService _viewSizeService;
        private float DeltaTime => _deltaTimeService.DeltaTime;


        private float _spawnTimer;
        public void Init(IProtoSystems systems)
        {
            _world = systems.World();

            var svc = systems.Services();
            _deltaTimeService = svc[typeof(IDeltaTimeService)] as IDeltaTimeService;
            _idGeneratorService = svc[typeof(IIdGeneratorService)] as IIdGeneratorService;
            _configService = svc[typeof(IConfigService)] as IConfigService;
            _viewSizeService = svc[typeof(IGameViewSizeService)] as IGameViewSizeService;
            
            _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));
            _positionAspect = (PositionAspect)_world.Aspect(typeof(PositionAspect));
        
            _spawnTimer = _configService.AsteroidSpawnInterval;
            
        }


        private float RandomSign => _random.Next(0, 2) * 2f - 1f;
        private float RandomNormalizedFloat => ((float)_random.NextDouble() - 0.5f) * 2f;
        public void Run()
        {
            var spawnInterval = _configService.AsteroidSpawnInterval;
            var spawnAmount = _configService.AsteroidSpawnAmount;
            
            if (spawnInterval == 0 || spawnAmount == 0) return;
            
            _spawnTimer += DeltaTime;
            
            if (_spawnTimer >= spawnInterval)
            {
                var baseSpeed = _configService.AsteroidSpeed;
                var rotationSpeed = _configService.AsteroidRotationSpeed;
                var randomnessWeight = _configService.AsteroidRandomnessWeight;
                _spawnTimer = 0;
                for (var i = 0; i < spawnAmount; i++)
                {
                    ref var teleportCounter = ref _entityAspect.TeleportCounterPool.NewEntity(out var asteroidEntity);
                    teleportCounter.teleportationLimit = _configService.AsteroidTeleportationLimit;

                    ref var positionData = ref _positionAspect.PositionPool.Add(asteroidEntity);
                    
                    var randomAngle = MathF.PI * 2f * (float)_random.NextDouble();

                    (positionData.x, positionData.y) = CalculateRandomPosition();
                    
                    ref var rotationData = ref _positionAspect.RotationPool.Add(asteroidEntity);
                    rotationData.angle = randomAngle;

                    var dirX = MathF.Cos(randomAngle);
                    var dirY = MathF.Sin(randomAngle);
                    ref var velocityData = ref _positionAspect.VelocityPool.Add(asteroidEntity);

                    var finalSpeed = baseSpeed * (1 + RandomNormalizedFloat * randomnessWeight);
                    velocityData.vx = dirX * finalSpeed;
                    velocityData.vy = dirY * finalSpeed;

                    velocityData.deceleration = 0f;

                    ref var angularVelocityData = ref _positionAspect.AngularVelocityPool.Add(asteroidEntity);
                    angularVelocityData.omega = RandomSign * rotationSpeed * (1 + RandomNormalizedFloat * randomnessWeight);

                    ref var entityIdData = ref _entityAspect.EntityIdPool.Add(asteroidEntity);
                    entityIdData.type = EntityType.Asteroid;
                    entityIdData.id = _idGeneratorService.GetNextId();
                }
            }
        }

        private (float, float) CalculateRandomPosition()
        {
            float halfWidth = _viewSizeService.HalfWidth;
            float halfHeight = _viewSizeService.HalfHeight;
            float margin = _configService.ScreenWrapMargin;

            bool horizontal = (_random.NextDouble() < 0.5);

            float x, y;
            
            if (horizontal)
            {
                float signX = RandomSign;
                x = signX * (halfWidth + margin);
                
                float t = (float)_random.NextDouble();
                y = -halfHeight + t * (halfHeight - (-halfHeight));
            }
            else
            {
                float signY = RandomSign;
                y = signY * (halfHeight + margin);
                
                float t = (float)_random.NextDouble();
                x = -halfWidth + t * (halfWidth - (-halfWidth));
            }
            return (x, y);

        }
    }
}