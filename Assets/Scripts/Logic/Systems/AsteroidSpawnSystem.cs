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
        private TransformAspect _transformAspect;
        private CollisionAspect _collisionAspect;

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
            _transformAspect = (TransformAspect)_world.Aspect(typeof(TransformAspect));
            _collisionAspect = (CollisionAspect)_world.Aspect(typeof(CollisionAspect));
        
            _spawnTimer = _configService.AsteroidConfig.SpawnInterval;
            
        }


        private float RandomSign => _random.Next(0, 2) * 2f - 1f;
        private float RandomNormalizedFloat => ((float)_random.NextDouble() - 0.5f) * 2f;
        public void Run()
        {
            var spawnInterval = _configService.AsteroidConfig.SpawnInterval;
            var spawnAmount = _configService.AsteroidConfig.SpawnAmount;
            
            if (spawnInterval == 0 || spawnAmount == 0) return;
            
            _spawnTimer += DeltaTime;
            
            if (_spawnTimer >= spawnInterval)
            {
                var baseSpeed = _configService.AsteroidConfig.Speed;
                var rotationSpeed = _configService.AsteroidConfig.RotationSpeed;
                var randomnessWeight = _configService.AsteroidConfig.RandomnessWeight;
                _spawnTimer = 0;
                for (var i = 0; i < spawnAmount; i++)
                {
                    ref var teleportCounter = ref _transformAspect.TeleportCounterPool.NewEntity(out var asteroidEntity);
                    teleportCounter.teleportationLimit = _configService.AsteroidConfig.TeleportationLimit;

                    ref var positionData = ref _transformAspect.PositionPool.Add(asteroidEntity);
                    
                    var randomAngle = MathF.PI * 2f * (float)_random.NextDouble();

                    (positionData.x, positionData.y) = CalculateRandomPosition();
                    
                    ref var rotationData = ref _transformAspect.RotationPool.Add(asteroidEntity);
                    rotationData.angle = randomAngle;

                    var dirX = MathF.Cos(randomAngle);
                    var dirY = MathF.Sin(randomAngle);
                    ref var velocityData = ref _transformAspect.VelocityPool.Add(asteroidEntity);

                    var finalSpeed = baseSpeed * (1 + RandomNormalizedFloat * randomnessWeight);
                    velocityData.x = dirX * finalSpeed;
                    velocityData.y = dirY * finalSpeed;

                    velocityData.deceleration = 0f;

                    ref var angularVelocityData = ref _transformAspect.AngularVelocityPool.Add(asteroidEntity);
                    angularVelocityData.omega = RandomSign * rotationSpeed * (1 + RandomNormalizedFloat * randomnessWeight);

                    ref var entityIdData = ref _entityAspect.EntityIdPool.Add(asteroidEntity);
                    entityIdData.type = EntityType.Asteroid;
                    entityIdData.id = _idGeneratorService.GetNextId();
                    
                    ref var collider = ref _collisionAspect.ColliderPool.Add(asteroidEntity);
                    collider.radius = _configService.AsteroidConfig.ColliderRadius;
                }
            }
        }

        private (float, float) CalculateRandomPosition()
        {
            float halfWidth = _viewSizeService.HalfWidth;
            float halfHeight = _viewSizeService.HalfHeight;
            float margin = _configService.WorldConfig.ScreenWrapMargin;

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