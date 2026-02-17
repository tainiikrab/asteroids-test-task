namespace AsteroidsGame.Logic
{
    using System;
    using AsteroidsGame.Contracts;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;

    public sealed class LaserSpawnSystem : IProtoInitSystem, IProtoRunSystem
    {
        private EntityAspect _entityAspect;
        private TransformAspect _transformAspect;
        private CollisionAspect _collisionAspect;
        private IConfigService _configService;
        private IDeltaTimeService _deltaTimeService;
        private IIdGeneratorService _idGeneratorService;

        private ProtoWorld _world;
        private ProtoIt _eventIterator;


        public void Init(IProtoSystems systems)
        {
            _world = systems.World();

            var svc = systems.Services();
            _configService = svc[typeof(IConfigService)] as IConfigService;
            _deltaTimeService = svc[typeof(IDeltaTimeService)] as IDeltaTimeService;
            _idGeneratorService = svc[typeof(IIdGeneratorService)] as IIdGeneratorService;


            _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));
            _transformAspect = (TransformAspect)_world.Aspect(typeof(TransformAspect));
            _collisionAspect = (CollisionAspect)_world.Aspect(typeof(CollisionAspect));

            _eventIterator = new ProtoIt(new[] { typeof(PlayerCmp) });
            _eventIterator.Init(_world);
        }

        public void Run()
        {
            var deltaTime = _deltaTimeService.DeltaTime;

            var laserConfig = _configService.LaserConfig;
            var shotInterval = laserConfig.ShotInterval;
            var maxLasers = laserConfig.MaxLasers;

            foreach (var playerEntity in _eventIterator)
            {
                ref var p = ref _entityAspect.PlayerPool.Get(playerEntity);

                p.laserIntervalTime += deltaTime;
                p.timeSinceLaserShot += deltaTime;

                if (p.laserIntervalTime >= shotInterval)
                {
                    p.laserIntervalTime -= shotInterval;
                    var nextCount = p.laserCount + 1;
                    p.laserCount = nextCount > maxLasers ? maxLasers : nextCount;
                }

                if (!p.isShootingLaser)
                    continue;
                p.isShootingLaser = false;

                if (p.timeSinceLaserShot <= _configService.LaserConfig.Duration) continue;
                p.timeSinceLaserShot = 0f;

                if (p.laserCount < 1)
                    continue;
                p.laserCount--;


                SpawnLaser(playerEntity);
            }
        }


        private void SpawnLaser(ProtoEntity playerEntity)
        {
            var playerPosition = _transformAspect.PositionPool.Get(playerEntity);
            var playerVelocity = _transformAspect.VelocityPool.Get(playerEntity);
            var playerRotation = _transformAspect.RotationPool.Get(playerEntity);

            ref var positon = ref _transformAspect.PositionPool.NewEntity(out var laserEntity);
            positon.x = playerPosition.x;
            positon.y = playerPosition.y;

            ref var velocity = ref _transformAspect.VelocityPool.Add(laserEntity);

            var angleRad = playerRotation.angle * (MathF.PI / 180f);

            velocity.x = playerVelocity.x;
            velocity.y = playerVelocity.y;

            ref var rotation = ref _transformAspect.RotationPool.Add(laserEntity);
            rotation.angle = playerRotation.angle;
            var _ = _transformAspect.AngularVelocityPool.Add(laserEntity);

            ref var laserCollider = ref _collisionAspect.LaserColliderPool.Add(laserEntity);
            laserCollider.radius = _configService.LaserConfig.ColliderRadius;

            var collisionSensor = _collisionAspect.CollisionSensorPool.Add(laserEntity);

            ref var entityId = ref _entityAspect.EntityIdPool.Add(laserEntity);
            entityId.id = _idGeneratorService.GetNextId();
            entityId.type = EntityType.Laser;

            ref var childComponent = ref _entityAspect.ChildPool.Add(laserEntity);
            childComponent.parent = _world.PackEntity(playerEntity);
            childComponent.followsParent = true;

            ref var timer = ref _entityAspect.TimerPool.Add(laserEntity);
            timer.interval = _configService.LaserConfig.Duration;
        }
    }
}