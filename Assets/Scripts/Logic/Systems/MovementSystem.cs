using AsteroidsGame.Contracts;
using Leopotam.EcsProto.QoL;

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
            foreach (var entity in _iterator)
            {
                ref var position = ref _transformAspect.PositionPool.Get(entity);
                ref var velocity = ref _transformAspect.VelocityPool.Get(entity);

                position.x += velocity.x * DeltaTime;
                position.y += velocity.y * DeltaTime;

                var hw = _viewSizeService?.HalfWidth ?? 0f;
                var hh = _viewSizeService?.HalfHeight ?? 0f;

                var wrapped = false;

                if (hw > 0f)
                {
                    position.x = TryWrap(position.x, -hw - _screenWrapPadding, hw + _screenWrapPadding,
                        out var wrappedW);
                    wrapped |= wrappedW;
                }

                if (hh > 0f)
                {
                    position.y = TryWrap(position.y, -hh - _screenWrapPadding, hh + _screenWrapPadding,
                        out var wrappedH);
                    wrapped |= wrappedH;
                }

                if (wrapped) CountExit(entity);


                if (velocity.x != 0f)
                {
                    velocity.x -= MathF.Sign(velocity.x) * velocity.deceleration * DeltaTime;
                    if (MathF.Abs(velocity.x) < Epsilon) velocity.x = 0f;
                }

                if (velocity.y != 0f)
                {
                    velocity.y -= MathF.Sign(velocity.y) * velocity.deceleration * DeltaTime;
                    if (MathF.Abs(velocity.y) < Epsilon) velocity.y = 0f;
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
            var range = max - min;
            if (range <= 0f || (value >= min && value <= max))
            {
                wrapped = false;
                return value;
            }

            wrapped = true;

            var t = (value - min) % range;
            if (t < 0f) t += range;
            return min + t;
        }
    }
}