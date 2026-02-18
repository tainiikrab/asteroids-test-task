using Leopotam.EcsProto;

namespace AsteroidsGame.Logic
{
    public class HealthSystem : IProtoInitSystem, IProtoRunSystem
    {
        private ProtoWorld _world;
        private EntityAspect _entityAspect;
        private ProtoIt _iterator;


        public void Init(IProtoSystems systems)
        {
            _world = systems.World();

            _entityAspect = _world.Aspect(typeof(EntityAspect)) as EntityAspect;
            _iterator = new ProtoIt(new[] { typeof(HealthCmp), typeof(DestroyTagCmp) });
            _iterator.Init(_world);
        }

        public void Run()
        {
            foreach (var entity in _iterator)
            {
                ref var healthCmp = ref _entityAspect.HealthPool.Get(entity);
                if (healthCmp.current <= 1) continue;
                healthCmp.current -= 1;
                _entityAspect.DestroyTagPool.Del(entity);
            }
        }
    }
}