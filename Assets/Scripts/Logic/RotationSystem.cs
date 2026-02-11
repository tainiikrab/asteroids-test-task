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

        public void SetDeltaTime(float dt)
        {
            _deltaTime = dt;
        }

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();
            _aspect = (GameAspect)_world.Aspect(typeof(GameAspect));
        }

        public void Run()
        {
            var it = new ProtoIt(new[] { typeof(RotationData), typeof(AngularVelocityData) });
            it.Init(_world);

            foreach (var e in it)
            {
                ref var rot = ref _aspect.RotationPool.Get(e);
                ref var ang = ref _aspect.AngularVelocityPool.Get(e);

                rot.angle -= ang.omega * _deltaTime;
            }
        }
    }
}