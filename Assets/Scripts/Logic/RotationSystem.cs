using AsteroidsGame.Contracts;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;

namespace AsteroidsGame.Logic
{
    public sealed class RotationSystem : IProtoInitSystem, IProtoRunSystem, IDeltaTimeUser
    {
        private PositionAspect _aspect;
        private ProtoWorld _world;
        private ProtoIt _iterator;

        public float DeltaTime { get; set; }

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();
            _aspect = (PositionAspect)_world.Aspect(typeof(PositionAspect));
            _iterator = new ProtoIt(new[] { typeof(RotationData), typeof(AngularVelocityData) });
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