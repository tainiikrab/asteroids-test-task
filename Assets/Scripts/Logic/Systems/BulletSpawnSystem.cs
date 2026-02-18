namespace AsteroidsGame.Logic
{
    using System;
    using AsteroidsGame.Contracts;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;

    public sealed class BulletSpawnSystem : IProtoInitSystem, IProtoRunSystem
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

            _eventIterator = new ProtoIt(new[] { typeof(BulletShooterCmp) });
            _eventIterator.Init(_world);
        }

        public void Run()
        {
            foreach (var shooterEntity in _eventIterator)
            {
                ref var shooterCmp =
                    ref _entityAspect.BulletShooterPool.Get(shooterEntity);

                shooterCmp.bulletReloadTimer += _deltaTimeService.DeltaTime;

                if (shooterCmp.bulletReloadTimer < _configService.BulletConfig.ShotCooldown) return;

                if (shooterCmp.isShootingBullet)
                {
                    SpawnBullet(shooterEntity);
                    shooterCmp.isShootingBullet = false;
                    shooterCmp.bulletReloadTimer = 0f;
                }
            }
        }

        private void SpawnBullet(ProtoEntity shooterEntity)
        {
            var shooterPosition = _transformAspect.PositionPool.Get(shooterEntity);
            var shooterVelocity = _transformAspect.VelocityPool.Get(shooterEntity);
            var shooterRotation = _transformAspect.RotationPool.Get(shooterEntity);

            ref var positon = ref _transformAspect.PositionPool.NewEntity(out var bulletEntity);
            positon.x = shooterPosition.x;
            positon.y = shooterPosition.y;

            ref var velocity = ref _transformAspect.VelocityPool.Add(bulletEntity);

            var angleRad = shooterRotation.angle * (MathF.PI / 180f);
            var dirX = MathF.Cos(angleRad);
            var dirY = MathF.Sin(angleRad);
            velocity.x = shooterVelocity.x + dirX * _configService.BulletConfig.Speed;
            velocity.y = shooterVelocity.y + dirY * _configService.BulletConfig.Speed;

            ref var rotation = ref _transformAspect.RotationPool.Add(bulletEntity);
            rotation.angle = shooterRotation.angle;

            ref var collider = ref _collisionAspect.CircleColliderPool.Add(bulletEntity);
            collider.radius = _configService.BulletConfig.ColliderRadius;

            ref var _ = ref _collisionAspect.CollisionSensorPool.Add(bulletEntity);

            ref var entityId = ref _entityAspect.EntityIdPool.Add(bulletEntity);
            entityId.id = _idGeneratorService.GetNextId();
            entityId.type = EntityType.Bullet;

            ref var childComponent = ref _entityAspect.ChildPool.Add(bulletEntity);
            childComponent.parent = _world.PackEntity(shooterEntity);

            ref var teleportCounter = ref _transformAspect.TeleportCounterPool.Add(bulletEntity);
            teleportCounter.teleportationLimit = _configService.BulletConfig.TeleportationLimit;
        }
    }
}