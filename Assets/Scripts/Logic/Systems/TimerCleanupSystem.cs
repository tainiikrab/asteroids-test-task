using Leopotam.EcsProto;

namespace AsteroidsGame.Logic
{
    public class TimerCleanupSystem : IProtoInitSystem, IProtoRunSystem
    {
        private ProtoWorld _world;
        private TransformAspect _transformAspect;
        private EntityAspect _entityAspect;
        private ProtoIt _iterator;

        private IDeltaTimeService _deltaTimeService;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();
            _deltaTimeService = systems.Services()[typeof(IDeltaTimeService)] as IDeltaTimeService;

            _transformAspect = _world.Aspect(typeof(TransformAspect)) as TransformAspect;
            _entityAspect = _world.Aspect(typeof(EntityAspect)) as EntityAspect;
            _iterator = new ProtoIt(new[] { typeof(TimerCmp) });
            _iterator.Init(_world);
        }

        public void Run()
        {
            foreach (var entity in _iterator)
            {
                ref var timerCmp = ref _entityAspect.TimerPool.Get(entity);
                timerCmp.time += _deltaTimeService.DeltaTime;

                if (timerCmp.time >= timerCmp.interval &&
                    !_entityAspect.DestroyTagPool.Has(entity))
                {
                    ref var _ = ref _entityAspect.DestroyTagPool.Add(entity);
                }
            }
        }
    }
}