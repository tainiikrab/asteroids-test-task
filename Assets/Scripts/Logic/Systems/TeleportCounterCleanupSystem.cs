using Leopotam.EcsProto;

namespace AsteroidsGame.Logic
{
    public class TeleportCounterCleanupSystem : IProtoInitSystem, IProtoRunSystem
    {
        private ProtoWorld _world;
        private EntityAspect _entityAspect;
        private ProtoIt _iterator;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();
            _entityAspect = _world.Aspect(typeof(EntityAspect)) as EntityAspect;
            _iterator = new ProtoIt(new[] { typeof(TeleportCounterCmp) });
            _iterator.Init(_world);
        }
        public void Run()
        {
            foreach (var entity in _iterator)
            {
                ref var counter = ref _entityAspect.TeleportCounterPool.Get(entity);
                if (counter.teleportationCount > counter.teleportationLimit)
                {
                    _world.DelEntity(entity);
                }
            }
        }
    }
}