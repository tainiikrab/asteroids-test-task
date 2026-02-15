using AsteroidsGame.Contracts;

namespace AsteroidsGame.Logic
{
    using System;
    using Leopotam.EcsProto;
    public sealed class MovementSystem : IProtoInitSystem, IProtoRunSystem
    {
        private TransformAspect _transformAspect;
        private EntityAspect _entityAspect;


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
            _screenWrapPadding = gameConfig?.WorldConfig.ScreenWrapMargin ?? 0f;

            _transformAspect = (TransformAspect)_world.Aspect(typeof(TransformAspect));
            _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));
            
            _iterator = new ProtoIt(new[] { typeof(PositionCmp), typeof(VelocityCmp) });
            _iterator.Init(_world);
        }

        public void Run()
        {
            foreach (var e in _iterator)
            {
                ref var p = ref _transformAspect.PositionPool.Get(e);
                ref var v = ref _transformAspect.VelocityPool.Get(e);

                p.x += v.vx * DeltaTime;
                p.y += v.vy * DeltaTime;
                
                var hw = _viewSizeService?.HalfWidth ?? 0f;
                var hh = _viewSizeService?.HalfHeight ?? 0f;
                
                var wrapped = false;

                if (hw > 0f)
                {
                    p.x = TryWrap(p.x, -hw - _screenWrapPadding, hw + _screenWrapPadding, out var wrappedW);
                    wrapped |= wrappedW;
                }

                if (hh > 0f)
                {
                    p.y = TryWrap(p.y, -hh - _screenWrapPadding, hh + _screenWrapPadding, out var wrappedH);
                    wrapped |= wrappedH;
                }
                if (wrapped) CountExit(e);
                
                
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

        private void CountExit(ProtoEntity entity)
        {
            if (_transformAspect.TeleportCounterPool.Has(entity))
            {
                ref var counter = ref _transformAspect.TeleportCounterPool.Get(entity);
                counter.teleportationCount++;
            }
        }
        private static float TryWrap(float value, float min, float max, out bool wrapped)
        {
            float range = max - min;
            if (range <= 0f || value >= min && value <= max)
            {
                wrapped = false;
                return value;
            }
            wrapped = true;

            float t = (value - min) % range;
            if (t < 0f) t += range;
            return min + t;
        }
    }
}