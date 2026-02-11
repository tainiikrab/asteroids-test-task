using Leopotam.EcsProto;
using AsteroidsGame.Contracts;

namespace AsteroidsGame.Logic
{
    public sealed class MovementSystem : IProtoInitSystem, IProtoRunSystem
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
            var it = new ProtoIt(new[] { typeof(PositionData), typeof(VelocityData) });

            it.Init(_world);

            foreach (var e in it)
            {
                ref var p = ref _aspect.PositionPool.Get(e);
                ref var v = ref _aspect.VelocityPool.Get(e);

                p.x += v.vx * _deltaTime;
                p.y += v.vy * _deltaTime;
            }
        }
    }
}