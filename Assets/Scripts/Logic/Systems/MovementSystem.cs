using AsteroidsGame.Contracts;

namespace AsteroidsGame.Logic
{
    using System;
    using Leopotam.EcsProto;
    public sealed class MovementSystem : IProtoInitSystem, IProtoRunSystem
    {
        private PositionAspect _aspect;


        private ProtoWorld _world;
        private ProtoIt _iterator;

        private const float Epsilon = 0.0001f;

        private IDeltaTimeService _deltaTimeService;
        private IGameViewSizeService _viewSizeService;
        private float DeltaTime => _deltaTimeService.DeltaTime;
        private float _screenWrapPadding;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();

            var svc = systems.Services();
            _deltaTimeService = svc[typeof(IDeltaTimeService)] as IDeltaTimeService;
            _viewSizeService = svc[typeof(IGameViewSizeService)] as IGameViewSizeService;
            var gameConfig = svc[typeof(IConfigService)] as IConfigService;
            _screenWrapPadding = gameConfig?.ScreenWrapPadding ?? 0f;

            _aspect = (PositionAspect)_world.Aspect(typeof(PositionAspect));
            _iterator = new ProtoIt(new[] { typeof(PositionCmp), typeof(VelocityCmp) });
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
                
                var hw = _viewSizeService?.HalfWidth ?? 0f;
                var hh = _viewSizeService?.HalfHeight ?? 0f;
                
                if (hw > 0f)
                    p.x = Wrap(p.x, -hw - _screenWrapPadding, hw + _screenWrapPadding);

                if (hh > 0f)
                    p.y = Wrap(p.y, -hh - _screenWrapPadding, hh + _screenWrapPadding);

                if (v.vx != 0f)
                {
                    v.vx -= MathF.Sign(v.vx) * v.deceleration * DeltaTime;
                    if (MathF.Abs(v.vx) < Epsilon) v.vx = 0f;
                }

                if (v.vy != 0f)
                {
                    v.vy -= MathF.Sign(v.vy) * v.deceleration * DeltaTime;
                    if (MathF.Abs(v.vy) < Epsilon) v.vy = 0f;
                }
            }
        }
        private static float Wrap(float value, float min, float max)
        {
            var range = max - min;
            if (range <= 0f) return value;
            var shifted = value - min;
            var mod = shifted - MathF.Floor(shifted / range) * range;
            return min + mod;
        }
    }
}