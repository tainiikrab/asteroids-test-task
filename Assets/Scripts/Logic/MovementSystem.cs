using System;
using Leopotam.EcsProto;
using AsteroidsGame.Contracts;

namespace AsteroidsGame.Logic
{
    public sealed class MovementSystem : IProtoInitSystem, IProtoRunSystem, IDeltaTimeUser
    {
        private PositionAspect _aspect;
        public float DeltaTime { get; set; }
        private ProtoWorld _world;
        private ProtoIt _iterator;

        private float _deceleration = 1f;

        private const float Epsilon = 0.0001f;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();
            _aspect = (PositionAspect)_world.Aspect(typeof(PositionAspect));
            _iterator = new ProtoIt(new[] { typeof(PositionData), typeof(VelocityData) });
            _iterator.Init(_world);
        }

        public void Run()
        {
            foreach (var e in _iterator)
            {
                ref var p = ref _aspect.PositionPool.Get(e);
                ref var v = ref _aspect.VelocityPool.Get(e);

                p.x += v.vx * DeltaTime;
                p.y += v.vy * DeltaTime;

                if (v.vx != 0f)
                {
                    v.vx -= MathF.Sign(v.vx) * _deceleration * DeltaTime;
                    if (MathF.Abs(v.vx) < Epsilon) v.vx = 0f;
                }

                if (v.vy != 0f)
                {
                    v.vy -= MathF.Sign(v.vy) * _deceleration * DeltaTime;
                    if (MathF.Abs(v.vy) < Epsilon) v.vy = 0f;
                }
            }
        }
    }
}