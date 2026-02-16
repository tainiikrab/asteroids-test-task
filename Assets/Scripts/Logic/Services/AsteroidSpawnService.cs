namespace AsteroidsGame.Logic
{
    using Leopotam.EcsProto;
    using System;
    using Contracts;

    public class AsteroidSpawnService : IAsteroidSpawnService
    {
        private readonly EntityAspect _entityAspect;
        private readonly TransformAspect _transformAspect;
        private readonly CollisionAspect _collisionAspect;

        private readonly IIdGeneratorService _idGeneratorService;
        private readonly IConfigService _configService;
        private readonly IRandomService _randomService;


        public AsteroidSpawnService(IProtoSystems systems)
        {
            var svc = systems.Services();
            var world = systems.World();

            _transformAspect = (TransformAspect)world.Aspect(typeof(TransformAspect));
            _collisionAspect = (CollisionAspect)world.Aspect(typeof(CollisionAspect));
            _entityAspect = (EntityAspect)world.Aspect(typeof(EntityAspect));

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

            var randomAngle = MathF.PI * 2f * (float)_randomService.NextFloat;

            ref var rotationData = ref _transformAspect.RotationPool.Add(asteroidEntity);
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
    }

    public interface IAsteroidSpawnService
    {
        ProtoEntity SpawnAsteroid(float x, float y);
    }
}