using AsteroidsGame.Contracts;

namespace AsteroidsGame.Logic
{
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;
    using System.Diagnostics;

    public sealed class AsteroidFragmentationSystem : IProtoInitSystem, IProtoRunSystem
    {
        private ProtoWorld _world;
        private ProtoIt _fragmentationIterator;

        private EntityAspect _entityAspect;
        private TransformAspect _transformAspect;
        private CollisionAspect _collisionAspect;

        private IEntitySpawnService _entitySpawnService;
        private IConfigService _configService;
        private IRandomService _randomService;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();
            _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));
            _transformAspect = (TransformAspect)_world.Aspect(typeof(TransformAspect));
            _collisionAspect = (CollisionAspect)_world.Aspect(typeof(CollisionAspect));

            var svc = systems.Services();
            _entitySpawnService = svc[typeof(IEntitySpawnService)] as IEntitySpawnService;
            _configService = svc[typeof(IConfigService)] as IConfigService;
            _randomService = svc[typeof(IRandomService)] as IRandomService;

            _fragmentationIterator = new ProtoIt(new[] { typeof(DestroyTagCmp), typeof(AsteroidCmp) });
            _fragmentationIterator.Init(_world);
        }

        public void Run()
        {
            foreach (var asteroidEntity in _fragmentationIterator)
            {
                var parentAsteroidComponent = _entityAspect.AsteroidPool.Get(asteroidEntity);
                if (parentAsteroidComponent.isFragment) continue;

                var parentPositionComponent = _transformAspect.PositionPool.Get(asteroidEntity);
                var parentVelocityComponent = _transformAspect.VelocityPool.Get(asteroidEntity);
                var parentAngularVelocityComponent = _transformAspect.AngularVelocityPool.Get(asteroidEntity);
                var fragmentCount = _configService.AsteroidFragmentationConfig.SpawnCount;

                var parentX = parentPositionComponent.x;
                var parentY = parentPositionComponent.y;
                var parentVelocityX = parentVelocityComponent.x;
                var parentVelocityY = parentVelocityComponent.y;
                var scatter = _configService.AsteroidFragmentationConfig.SpawnScatter;
                var speedMultiplier = _configService.AsteroidFragmentationConfig.SpeedMultiplier;

                for (var i = 0; i < fragmentCount; i++)
                {
                    var x = parentX + scatter * _randomService.RandomNormalizedFloat;
                    var y = parentY + scatter * _randomService.RandomNormalizedFloat;

                    var fragmentEntity = _entitySpawnService.SpawnAsteroidFragment(x, y);

                    // ref var fragmentComponent = ref _entityAspect.AsteroidPool.Get(fragmentEntity);
                    // fragmentComponent.isFragment = true;
                    //
                    // ref var velocityComponent = ref _transformAspect.VelocityPool.Get(fragmentEntity);
                    // velocityComponent.x = parentVelocityX * (speedMultiplier + _randomService.RandomNormalizedFloat *
                    //     _configService.AsteroidConfig.RandomnessWeight);
                    // velocityComponent.y = parentVelocityY * (speedMultiplier + _randomService.RandomNormalizedFloat *
                    //     _configService.AsteroidConfig.RandomnessWeight);
                    //
                    // ref var angularVelocityComponent = ref _transformAspect.AngularVelocityPool.Get(fragmentEntity);
                    // angularVelocityComponent.omega = parentAngularVelocityComponent.omega *
                    //                                  _configService.AsteroidFragmentationConfig.RotationSpeedMultiplier;
                    //
                    // ref var colliderComponent = ref _collisionAspect.ColliderPool.Get(fragmentEntity);
                    // colliderComponent.radius = _configService.AsteroidFragmentationConfig.ColliderRadius;
                    //
                    // ref var entityComponent = ref _entityAspect.EntityIdPool.Get(fragmentEntity);
                    // entityComponent.type = EntityType.AsteroidFragment;
                }
            }
        }
    }
}