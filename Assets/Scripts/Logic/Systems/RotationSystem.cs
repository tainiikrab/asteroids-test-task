

namespace AsteroidsGame.Logic
{
    using Leopotam.EcsProto;
    public sealed class RotationSystem : IProtoInitSystem, IProtoRunSystem
    {
        private PositionAspect _aspect;
        private ProtoWorld _world;
        private ProtoIt _iterator;


        private IDeltaTimeService _deltaTimeService;
        private float DeltaTime => _deltaTimeService.DeltaTime;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();

            var svc = systems.Services();
            _deltaTimeService = svc[typeof(IDeltaTimeService)] as IDeltaTimeService;
            _aspect = (PositionAspect)_world.Aspect(typeof(PositionAspect));
            _iterator = new ProtoIt(new[] { typeof(RotationCmp), typeof(AngularVelocityCmp) });
            _iterator.Init(_world);
        }

        public void Run()
        {
            foreach (var e in _iterator)
            {
                ref var rot = ref _aspect.RotationPool.Get(e);
                ref var ang = ref _aspect.AngularVelocityPool.Get(e);

                rot.angle -= ang.omega * DeltaTime;
            }
        }
    }
}