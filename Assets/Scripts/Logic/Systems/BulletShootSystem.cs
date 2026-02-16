// Assets/Scripts/Logic/CollisionResolutionSystem.cs

using System;
using AsteroidsGame.Contracts;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;

namespace AsteroidsGame.Logic
{
    public sealed class BulletShootSystem : IProtoInitSystem, IProtoRunSystem
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
            _transformAspect = (TransformAspect)_world.Aspect((typeof(TransformAspect)));
            _collisionAspect = (CollisionAspect)_world.Aspect(typeof(CollisionAspect));

            _eventIterator = new ProtoIt(new[] { typeof(PlayerCmp) });
            _eventIterator.Init(_world);
            
        }

        public void Run()
        {
            
            foreach (ProtoEntity playerEntity in _eventIterator)
            {
                ref var playerComponent =
                    ref _entityAspect.PlayerPool.Get(playerEntity);
                
                playerComponent.bulletIntervalTime += _deltaTimeService.DeltaTime;
                
                if (playerComponent.bulletIntervalTime < _configService.BulletConfig.ShotInterval) return;
                
                if (playerComponent.isShootingBullet)
                {
                    SpawnBullet(playerEntity);
                    playerComponent.isShootingBullet = false;
                    playerComponent.bulletIntervalTime = 0f;
                }
            }
        }

        private void SpawnBullet(ProtoEntity playerEntity)
        {
            ref var playerPosition = ref _transformAspect.PositionPool.Get(playerEntity);
            ref var playerRotation = ref _transformAspect.RotationPool.Get(playerEntity);
            
            ref var positon = ref _transformAspect.PositionPool.NewEntity(out var bulletEntity);
            positon.x = playerPosition.x;
            positon.y = playerPosition.y;
            
            ref var velocity = ref _transformAspect.VelocityPool.Add(bulletEntity);

            var angleRad = playerRotation.angle * (MathF.PI / 180f);
            var dirX = MathF.Cos(angleRad);
            var dirY = MathF.Sin(angleRad);
            velocity.x = dirX * _configService.BulletConfig.Speed;
            velocity.y = dirY * _configService.BulletConfig.Speed;
            
            ref var rotation = ref _transformAspect.RotationPool.Add(bulletEntity);
            rotation.angle = playerRotation.angle;
            
            ref var collider = ref _collisionAspect.ColliderPool.Add(bulletEntity);
            collider.radius = _configService.BulletConfig.ColliderRadius;
            
            ref var collisionSensor = ref _collisionAspect.CollisionSensorPool.Add(bulletEntity);
            
            ref var entityId = ref _entityAspect.EntityIdPool.Add(bulletEntity);
            entityId.id = _idGeneratorService.GetNextId();
            entityId.type = EntityType.Bullet;
            
            ref var bulletComponent = ref _entityAspect.BulletPool.Add(bulletEntity);
            bulletComponent.owner = _world.PackEntity(playerEntity);
            
        }

}
}