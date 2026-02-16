using System;
using System.Collections.Generic;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using AsteroidsGame.Contracts;

namespace AsteroidsGame.Logic
{
    public interface IEntitySpawnService
    {
        ProtoEntity SpawnEntity(EntitySpawnService.EntitySpawnConfig config);
        ProtoEntity SpawnAsteroid(float x, float y);
        ProtoEntity SpawnAsteroidFragment(float x, float y);
        ProtoEntity SpawnPlayer();
        ProtoEntity SpawnBulletFromPlayer(ProtoEntity playerEntity);
        void RegisterSpawner(EntityType type, Func<EntitySpawnService.EntitySpawnConfig, ProtoEntity> spawner);
    }

    public sealed class EntitySpawnService : IEntitySpawnService
    {
        private readonly EntityAspect _entityAspect;
        private readonly TransformAspect _transformAspect;
        private readonly CollisionAspect _collisionAspect;

        private readonly IIdGeneratorService _idGeneratorService;
        private readonly IConfigService _configService;
        private readonly IRandomService _randomService;

        private readonly Dictionary<EntityType, Func<EntitySpawnConfig, ProtoEntity>> _spawners = new();

        private ProtoWorld _world;

        public struct EntitySpawnConfig
        {
            public float SpawnPosX;
            public float SpawnPosY;
            public EntityType EntityType;

            public static EntitySpawnConfig For(EntityType t, float x, float y)
            {
                return new EntitySpawnConfig { EntityType = t, SpawnPosX = x, SpawnPosY = y };
            }
        }

        public EntitySpawnService(IProtoSystems systems)
        {
            var svc = systems.Services();
            _world = systems.World();

            _transformAspect = (TransformAspect)_world.Aspect(typeof(TransformAspect));
            _collisionAspect = (CollisionAspect)_world.Aspect(typeof(CollisionAspect));
            _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));

            _idGeneratorService = svc[typeof(IIdGeneratorService)] as IIdGeneratorService;
            _configService = svc[typeof(IConfigService)] as IConfigService;
            _randomService = svc[typeof(IRandomService)] as IRandomService;

            _spawners[EntityType.Asteroid] =
                cfg => SpawnAsteroidInternal(cfg.SpawnPosX, cfg.SpawnPosY);

            _spawners[EntityType.AsteroidFragment] =
                cfg => SpawnAsteroidFragmentInternal(cfg.SpawnPosX, cfg.SpawnPosY);

            _spawners[EntityType.Player] =
                cfg => SpawnPlayerInternal(cfg.SpawnPosX, cfg.SpawnPosY);

            _spawners[EntityType.Bullet] =
                cfg => SpawnBulletInternal(cfg.SpawnPosX, cfg.SpawnPosY);
        }

        public ProtoEntity SpawnEntity(EntitySpawnConfig config)
        {
            if (_spawners.TryGetValue(config.EntityType, out var spawner))
                return spawner(config);

            throw new ArgumentOutOfRangeException(nameof(config.EntityType), config.EntityType,
                "No spawner registered");
        }

        public ProtoEntity SpawnAsteroid(float x, float y)
        {
            return SpawnEntity(EntitySpawnConfig.For(EntityType.Asteroid, x, y));
        }

        public ProtoEntity SpawnAsteroidFragment(float x, float y)
        {
            return SpawnEntity(EntitySpawnConfig.For(EntityType.AsteroidFragment, x, y));
        }

        public ProtoEntity SpawnPlayer()
        {
            return SpawnEntity(EntitySpawnConfig.For(EntityType.Player, 0f, 0f));
        }

        public ProtoEntity SpawnBulletFromPlayer(ProtoEntity playerEntity)
        {
            var playerPosition = _transformAspect.PositionPool.Get(playerEntity);
            var playerVelocity = _transformAspect.VelocityPool.Get(playerEntity);
            var playerRotation = _transformAspect.RotationPool.Get(playerEntity);

            var bullet = SpawnEntity(EntitySpawnConfig.For(EntityType.Bullet, playerPosition.x, playerPosition.y));

            ref var bulletComponent = ref _entityAspect.BulletPool.Get(bullet);
            bulletComponent.owner = _world.PackEntity(playerEntity);

            var angleRad = playerRotation.angle * (MathF.PI / 180f);
            ref var velocity = ref _transformAspect.VelocityPool.Get(bullet);
            velocity.x = playerVelocity.x + MathF.Cos(angleRad) * _configService.BulletConfig.Speed;
            velocity.y = playerVelocity.y + MathF.Sin(angleRad) * _configService.BulletConfig.Speed;

            ref var rotation = ref _transformAspect.RotationPool.Get(bullet);
            rotation.angle = playerRotation.angle;

            ref var teleport = ref _transformAspect.TeleportCounterPool.Get(bullet);
            teleport.teleportationLimit = _configService.BulletConfig.TeleportationLimit;

            return bullet;
        }

        public void RegisterSpawner(EntityType type, Func<EntitySpawnConfig, ProtoEntity> spawner)
        {
            _spawners[type] = spawner;
        }

        private ProtoEntity NewEntityAt(float x, float y)
        {
            ref var pos = ref _transformAspect.PositionPool.NewEntity(out var e);
            pos.x = x;
            pos.y = y;

            ref var id = ref _entityAspect.EntityIdPool.Add(e);
            id.id = _idGeneratorService.GetNextId();
            return e;
        }

        private void AddTeleport(ProtoEntity e, int limit)
        {
            ref var t = ref _transformAspect.TeleportCounterPool.Add(e);
            t.teleportationLimit = limit;
        }

        private void AddCollider(ProtoEntity e, float radius)
        {
            ref var c = ref _collisionAspect.ColliderPool.Add(e);
            c.radius = radius;
        }

        private void AddCollisionSensor(ProtoEntity e)
        {
            ref var s = ref _collisionAspect.CollisionSensorPool.Add(e);
            _ = s;
        }

        private float AddRotation(ProtoEntity e, float? angleRadNullable = null)
        {
            var angle = angleRadNullable ?? MathF.PI * 2f * _randomService.NextFloat;
            ref var r = ref _transformAspect.RotationPool.Add(e);
            r.angle = angle;
            return angle;
        }

        private void AddVelocityPolar(ProtoEntity e, float angleRad, float speed, float deceleration = 0f)
        {
            ref var v = ref _transformAspect.VelocityPool.Add(e);
            v.x = MathF.Cos(angleRad) * speed;
            v.y = MathF.Sin(angleRad) * speed;
            v.deceleration = deceleration;
        }

        private void AddAngularRandom(ProtoEntity e, float baseSpeed, float randomnessWeight)
        {
            ref var a = ref _transformAspect.AngularVelocityPool.Add(e);
            a.omega = _randomService.RandomSign * baseSpeed *
                      (1 + _randomService.RandomNormalizedFloat * randomnessWeight);
        }

        private void SetEntityType(ProtoEntity e, EntityType type)
        {
            ref var id = ref _entityAspect.EntityIdPool.Get(e);
            id.type = type;
        }

        private ProtoEntity SpawnAsteroidInternal(float x, float y)
        {
            var cfg = _configService.AsteroidConfig;

            var e = NewEntityAt(x, y);
            AddTeleport(e, cfg.TeleportationLimit);

            var angle = AddRotation(e);
            var finalSpeed = cfg.Speed * (1 + _randomService.RandomNormalizedFloat * cfg.RandomnessWeight);
            AddVelocityPolar(e, angle, finalSpeed);
            AddAngularRandom(e, cfg.RotationSpeed, cfg.RandomnessWeight);

            AddCollider(e, cfg.ColliderRadius);

            SetEntityType(e, EntityType.Asteroid);
            ref var asteroidComp = ref _entityAspect.AsteroidPool.Add(e);
            asteroidComp.isFragment = false;

            return e;
        }

        private ProtoEntity SpawnAsteroidFragmentInternal(float x, float y)
        {
            var fragCfg = _configService.AsteroidFragmentationConfig;

            var e = NewEntityAt(x, y);
            AddTeleport(e, fragCfg.TeleportationLimit);

            var angle = AddRotation(e);
            var speed = fragCfg.SpawnScatter * (0.5f + _randomService.RandomNormalizedFloat);
            AddVelocityPolar(e, angle, speed);

            AddAngularRandom(e, fragCfg.RotationSpeedMultiplier, 0f);
            AddCollider(e, fragCfg.ColliderRadius);

            SetEntityType(e, EntityType.AsteroidFragment);
            ref var asteroidComp = ref _entityAspect.AsteroidPool.Add(e);
            asteroidComp.isFragment = true;

            return e;
        }

        private ProtoEntity SpawnPlayerInternal(float x, float y)
        {
            var pCfg = _configService.PlayerConfig;

            var e = NewEntityAt(x, y);

            ref var vel = ref _transformAspect.VelocityPool.Add(e);
            vel.x = 0f;
            vel.y = 0f;
            vel.deceleration = pCfg.Deceleration;

            AddRotation(e, 0f);
            ref var ang = ref _transformAspect.AngularVelocityPool.Add(e);
            ang.omega = 0f;

            ref var id = ref _entityAspect.EntityIdPool.Get(e);
            id.packedEntity = _world.PackEntity(e);
            SetEntityType(e, EntityType.Player);

            ref var player = ref _entityAspect.PlayerPool.Add(e);
            player.bulletIntervalTime = _configService.BulletConfig.ShotInterval;

            AddCollider(e, pCfg.ColliderRadius);
            AddCollisionSensor(e);

            return e;
        }

        private ProtoEntity SpawnBulletInternal(float x, float y)
        {
            var bCfg = _configService.BulletConfig;

            var e = NewEntityAt(x, y);

            AddRotation(e, 0f);
            ref var vel = ref _transformAspect.VelocityPool.Add(e);
            vel.x = 0f;
            vel.y = 0f;
            vel.deceleration = 0f;

            AddCollider(e, bCfg.ColliderRadius);
            AddCollisionSensor(e);

            SetEntityType(e, EntityType.Bullet);
            ref var bulletComp = ref _entityAspect.BulletPool.Add(e);
            bulletComp.owner = default;

            AddTeleport(e, bCfg.TeleportationLimit);

            return e;
        }
    }
}