using AsteroidsGame.Contracts;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;

namespace AsteroidsGame.Logic
{
    public sealed class RotationSystem : IProtoInitSystem, IProtoRunSystem
    {
        private GameAspect _aspect;
        private float _deltaTime;
        private ProtoWorld _world;
        private ProtoIt _iterator;

        public void SetDeltaTime(float dt)
        {
            _deltaTime = dt;
        }

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();
            _aspect = (GameAspect)_world.Aspect(typeof(GameAspect));
            _iterator = new ProtoIt(new[] { typeof(RotationData), typeof(AngularVelocityData) });
            _iterator.Init(_world);
        }

        public void Run()
        {
            foreach (var e in _iterator)
            {
                ref var rot = ref _aspect.RotationPool.Get(e);
                ref var ang = ref _aspect.AngularVelocityPool.Get(e);

                rot.angle -= ang.omega * _deltaTime;
            }
        }
    }
}