using Leopotam.EcsProto.QoL;

namespace AsteroidsGame.Logic
{
    using Leopotam.EcsProto;
    using System;
    using Contracts;

    public class ObstacleSpawnService : IObstacleSpawnService
    {
        private readonly EntityAspect _entityAspect;
        private readonly TransformAspect _transformAspect;
        private readonly CollisionAspect _collisionAspect;

        private readonly IIdGeneratorService _idGeneratorService;
        private readonly IConfigService _configService;
        private readonly IRandomService _randomService;

        private ProtoWorld _world;

        public ObstacleSpawnService(IProtoSystems systems)
        {
            var svc = systems.Services();
            _world = systems.World();

            _transformAspect = (TransformAspect)_world.Aspect(typeof(TransformAspect));
            _collisionAspect = (CollisionAspect)_world.Aspect(typeof(CollisionAspect));
            _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));

            _idGeneratorService = svc[typeof(IIdGeneratorService)] as IIdGeneratorService;
            _configService = svc[typeof(IConfigService)] as IConfigService;
            _randomService = svc[typeof(IRandomService)] as IRandomService;
        }

        public ProtoEntity SpawnAsteroid(float x, float y)
        {
            var config = _configService.AsteroidConfig;
            ref var teleportCounter =
                ref _transformAspect.TeleportCounterPool.NewEntity(out var asteroidEntity);
            teleportCounter.teleportationLimit = config.TeleportationLimit;

            ref var positionData = ref _transformAspect.PositionPool.Add(asteroidEntity);
            (positionData.x, positionData.y) = (x, y);


            ref var rotationData = ref _transformAspect.RotationPool.Add(asteroidEntity);
            var randomAngle = _randomService.RandomAngleDegrees;
            rotationData.angle = randomAngle;

            var dirX = MathF.Cos(randomAngle);
            var dirY = MathF.Sin(randomAngle);

            var finalSpeed = config.Speed * (1 + _randomService.RandomNormalizedFloat * config.RandomnessWeight);

            ref var velocityData = ref _transformAspect.VelocityPool.Add(asteroidEntity);
            velocityData.x = dirX * finalSpeed;
            velocityData.y = dirY * finalSpeed;
            velocityData.deceleration = 0f;

            ref var angularVelocityData = ref _transformAspect.AngularVelocityPool.Add(asteroidEntity);
            angularVelocityData.omega =
                _randomService.RandomSign * config.RotationSpeed *
                (1 + _randomService.RandomNormalizedFloat * config.RandomnessWeight);

            ref var entityIdData = ref _entityAspect.EntityIdPool.Add(asteroidEntity);
            entityIdData.type = EntityType.Asteroid;
            entityIdData.id = _idGeneratorService.GetNextId();

            ref var collider = ref _collisionAspect.ColliderPool.Add(asteroidEntity);
            collider.radius = config.ColliderRadius;

            ref var asteroidData = ref _entityAspect.AsteroidPool.Add(asteroidEntity);
            asteroidData.isFragment = false;

            return asteroidEntity;
        }

        public ProtoEntity SpawnSaucer(float x, float y, ProtoPackedEntity playerEntity)
        {
            var config = _configService.SaucerConfig;


            ref var positionData = ref _transformAspect.PositionPool.NewEntity(out var saucerEntity);
            (positionData.x, positionData.y) = (x, y);

            ref var velocityData = ref _transformAspect.VelocityPool.Add(saucerEntity);
            ref var rotationData = ref _transformAspect.RotationPool.Add(saucerEntity);

            if (playerEntity.TryUnpack(_world, out var player))
            {
                var playerPos = _transformAspect.PositionPool.Get(player);

                var dirX = playerPos.x - x;
                var dirY = playerPos.y - y;

                var angle = MathF.Atan2(dirY, dirX) * 180f / MathF.PI;
                rotationData.angle = angle;
            }
            else
            {
                var randomAngle = _randomService.RandomAngleDegrees;
                rotationData.angle = randomAngle;

                var dirX = MathF.Cos(randomAngle);
                var dirY = MathF.Sin(randomAngle);

                velocityData.x = dirX * config.Speed;
                velocityData.y = dirY * config.Speed;
            }


            ref var angularVelocityData = ref _transformAspect.AngularVelocityPool.Add(saucerEntity);

            ref var entityIdData = ref _entityAspect.EntityIdPool.Add(saucerEntity);
            entityIdData.type = EntityType.Saucer;
            entityIdData.id = _idGeneratorService.GetNextId();

            ref var collider = ref _collisionAspect.ColliderPool.Add(saucerEntity);
            collider.radius = config.ColliderRadius;

            ref var saucerData = ref _entityAspect.FollowerPool.Add(saucerEntity);
            saucerData.Target = playerEntity;

            ref var teleportCounter = ref _transformAspect.TeleportCounterPool.Add(saucerEntity);
            teleportCounter.teleportationLimit = config.TeleportationLimit;

            return saucerEntity;
        }
    }

    public interface IObstacleSpawnService
    {
        ProtoEntity SpawnAsteroid(float x, float y);
        ProtoEntity SpawnSaucer(float x, float y, ProtoPackedEntity playerEntity);
    }
}