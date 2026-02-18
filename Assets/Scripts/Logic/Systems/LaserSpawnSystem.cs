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

            _eventIterator = new ProtoIt(new[] { typeof(LaserShooterCmp) });
            _eventIterator.Init(_world);
        }

        public void Run()
        {
            var deltaTime = _deltaTimeService.DeltaTime;

            var laserConfig = _configService.LaserConfig;
            var shotInterval = laserConfig.ShotCooldown;
            var maxLasers = laserConfig.MaxLasers;

            foreach (var laserEntity in _eventIterator)
            {
                ref var laserShooterCmp = ref _entityAspect.LaserShooterPool.Get(laserEntity);

                if (laserShooterCmp.laserCount < maxLasers)
                    laserShooterCmp.laserReloadTimer += deltaTime;
                else
                    laserShooterCmp.laserReloadTimer = 0f;
                laserShooterCmp.timeSinceLaserShot += deltaTime;

                if (laserShooterCmp.laserReloadTimer >= shotInterval)
                {
                    laserShooterCmp.laserReloadTimer -= shotInterval;
                    var nextCount = laserShooterCmp.laserCount + 1;
                    laserShooterCmp.laserCount = nextCount > maxLasers ? maxLasers : nextCount;
                }

                if (!laserShooterCmp.isShootingLaser)
                    continue;
                laserShooterCmp.isShootingLaser = false;

                if (laserShooterCmp.timeSinceLaserShot <= _configService.LaserConfig.Duration) continue;
                laserShooterCmp.timeSinceLaserShot = 0f;

                if (laserShooterCmp.laserCount < 1)
                    continue;
                laserShooterCmp.laserCount--;


                SpawnLaser(laserEntity);
            }
        }


        private void SpawnLaser(ProtoEntity laserShooterEntity)
        {
            var shooterPosition = _transformAspect.PositionPool.Get(laserShooterEntity);
            var shooterVelocity = _transformAspect.VelocityPool.Get(laserShooterEntity);
            var shooterRotation = _transformAspect.RotationPool.Get(laserShooterEntity);

            ref var positon = ref _transformAspect.PositionPool.NewEntity(out var laserEntity);
            positon.x = shooterPosition.x;
            positon.y = shooterPosition.y;

            ref var velocity = ref _transformAspect.VelocityPool.Add(laserEntity);

            var angleRad = shooterRotation.angle * (MathF.PI / 180f);

            velocity.x = shooterVelocity.x;
            velocity.y = shooterVelocity.y;

            ref var rotation = ref _transformAspect.RotationPool.Add(laserEntity);
            rotation.angle = shooterRotation.angle;
            _ = _transformAspect.AngularVelocityPool.Add(laserEntity);

            ref var laserCollider = ref _collisionAspect.LaserColliderPool.Add(laserEntity);
            laserCollider.radius = _configService.LaserConfig.ColliderRadius;

            var collisionSensor = _collisionAspect.CollisionSensorPool.Add(laserEntity);

            ref var entityId = ref _entityAspect.EntityIdPool.Add(laserEntity);
            entityId.id = _idGeneratorService.GetNextId();
            entityId.type = EntityType.Laser;

            ref var childComponent = ref _entityAspect.ChildPool.Add(laserEntity);
            childComponent.parent = _world.PackEntity(laserShooterEntity);
            childComponent.followsParent = true;

            ref var timer = ref _entityAspect.TimerPool.Add(laserEntity);
            timer.interval = _configService.LaserConfig.Duration;
        }
    }
}