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

            _eventIterator = new ProtoIt(new[] { typeof(PlayerCmp) });
            _eventIterator.Init(_world);
        }

        public void Run()
        {
            foreach (var playerEntity in _eventIterator)
            {
                ref var playerComponent =
                    ref _entityAspect.PlayerPool.Get(playerEntity);

                playerComponent.bulletReloadTimer += _deltaTimeService.DeltaTime;

                if (playerComponent.bulletReloadTimer < _configService.BulletConfig.ShotCooldown) return;

                if (playerComponent.isShootingBullet)
                {
                    SpawnBullet(playerEntity);
                    playerComponent.isShootingBullet = false;
                    playerComponent.bulletReloadTimer = 0f;
                }
            }
        }

        private void SpawnBullet(ProtoEntity playerEntity)
        {
            var playerPosition = _transformAspect.PositionPool.Get(playerEntity);
            var playerVelocity = _transformAspect.VelocityPool.Get(playerEntity);
            var playerRotation = _transformAspect.RotationPool.Get(playerEntity);

            ref var positon = ref _transformAspect.PositionPool.NewEntity(out var bulletEntity);
            positon.x = playerPosition.x;
            positon.y = playerPosition.y;

            ref var velocity = ref _transformAspect.VelocityPool.Add(bulletEntity);

            var angleRad = playerRotation.angle * (MathF.PI / 180f);
            var dirX = MathF.Cos(angleRad);
            var dirY = MathF.Sin(angleRad);
            velocity.x = playerVelocity.x + dirX * _configService.BulletConfig.Speed;
            velocity.y = playerVelocity.y + dirY * _configService.BulletConfig.Speed;

            ref var rotation = ref _transformAspect.RotationPool.Add(bulletEntity);
            rotation.angle = playerRotation.angle;

            ref var collider = ref _collisionAspect.CircleColliderPool.Add(bulletEntity);
            collider.radius = _configService.BulletConfig.ColliderRadius;

            ref var _ = ref _collisionAspect.CollisionSensorPool.Add(bulletEntity);

            ref var entityId = ref _entityAspect.EntityIdPool.Add(bulletEntity);
            entityId.id = _idGeneratorService.GetNextId();
            entityId.type = EntityType.Bullet;

            ref var childComponent = ref _entityAspect.ChildPool.Add(bulletEntity);
            childComponent.parent = _world.PackEntity(playerEntity);

            ref var teleportCounter = ref _transformAspect.TeleportCounterPool.Add(bulletEntity);
            teleportCounter.teleportationLimit = _configService.BulletConfig.TeleportationLimit;
        }
    }
}