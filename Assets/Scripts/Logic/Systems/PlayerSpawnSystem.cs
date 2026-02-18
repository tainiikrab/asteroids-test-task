namespace AsteroidsGame.Logic
{
    using AsteroidsGame.Contracts;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;

    public class PlayerSpawnSystem : IProtoInitSystem
    {
        private ProtoWorld _world;

        private TransformAspect _transformAspect;
        private EntityAspect _entityAspect;
        private CollisionAspect _collisionAspect;

        private IConfigService _configService;
        private IIdGeneratorService _idGeneratorService;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();

            var svc = systems.Services();
            _configService = svc[typeof(IConfigService)] as IConfigService;
            _idGeneratorService = svc[typeof(IIdGeneratorService)] as IIdGeneratorService;
            _transformAspect = (TransformAspect)_world.Aspect(typeof(TransformAspect));
            _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));
            _collisionAspect = (CollisionAspect)_world.Aspect(typeof(CollisionAspect));

            SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            ref var positionData = ref _transformAspect.PositionPool.NewEntity(out var playerEntity);
            positionData.x = 0;
            positionData.y = 0;

            ref var velocityData = ref _transformAspect.VelocityPool.Add(playerEntity);
            velocityData.x = 0;
            velocityData.y = 0;
            velocityData.deceleration = _configService.PlayerConfig.Deceleration;

            ref var rotationData = ref _transformAspect.RotationPool.Add(playerEntity);
            rotationData.angle = 0f;

            ref var angularVelocityData = ref _transformAspect.AngularVelocityPool.Add(playerEntity);
            angularVelocityData.omega = 0f;

            ref var entityIdComponent = ref _entityAspect.EntityIdPool.Add(playerEntity);
            var packed = _world.PackEntity(playerEntity);
            entityIdComponent.id = _idGeneratorService.GetNextId();
            entityIdComponent.packedEntity = packed;
            entityIdComponent.type = EntityType.Player;

            ref var player = ref _entityAspect.PlayerPool.Add(playerEntity);
            ref var bulletShooter = ref _entityAspect.BulletShooterPool.Add(playerEntity);
            ref var laserShooter = ref _entityAspect.LaserShooterPool.Add(playerEntity);

            ref var collider = ref _collisionAspect.CircleColliderPool.Add(playerEntity);
            collider.radius = _configService.PlayerConfig.ColliderRadius;

            ref var collisionSensor = ref _collisionAspect.CollisionSensorPool.Add(playerEntity);
        }
    }
}