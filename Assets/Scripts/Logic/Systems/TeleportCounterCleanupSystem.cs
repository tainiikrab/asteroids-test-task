using Leopotam.EcsProto;

namespace AsteroidsGame.Logic
{
    public class TeleportCounterCleanupSystem : IProtoInitSystem, IProtoRunSystem
    {
        private ProtoWorld _world;
        private TransformAspect _transformAspect;
        private EntityAspect _entityAspect;
        private ProtoIt _iterator;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();
            _transformAspect = _world.Aspect(typeof(TransformAspect)) as TransformAspect;
            _entityAspect = _world.Aspect(typeof(EntityAspect)) as EntityAspect;
            _iterator = new ProtoIt(new[] { typeof(TeleportCounterCmp) });
            _iterator.Init(_world);
        }
        public void Run()
        {
            foreach (var entity in _iterator)
            {
                ref var counter = ref _transformAspect.TeleportCounterPool.Get(entity);
                if (counter.teleportationCount > counter.teleportationLimit && !_entityAspect.DestroyTagPool.Has(entity))
                {
                    ref var _ = ref _entityAspect.DestroyTagPool.Add(entity);
                }
            }
        }
    }
}