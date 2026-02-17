using Leopotam.EcsProto.QoL;

namespace AsteroidsGame.Logic
{
    using System;
    using Leopotam.EcsProto;
    using Contracts;

    public sealed class SaucerHomingSystem : IProtoInitSystem, IProtoRunSystem
    {
        private IConfigService _configService;
        private IDeltaTimeService _deltaTimeService;

        private EntityAspect _entityAspect;
        private TransformAspect _transformAspect;
        private ProtoWorld _world;

        private IProtoIt _saucerIterator;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();
            var src = systems.Services();

            _configService = (IConfigService)src[typeof(IConfigService)];
            _deltaTimeService = (IDeltaTimeService)src[typeof(IDeltaTimeService)];

            _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));
            _transformAspect = (TransformAspect)_world.Aspect(typeof(TransformAspect));

            _saucerIterator = new ProtoIt(new[] { typeof(FollowerCmp) });
            _saucerIterator.Init(_world);
        }

        public void Run()
        {
            var dt = _deltaTimeService.DeltaTime;

            foreach (var saucerEntity in _saucerIterator)
            {
                ref var follower = ref _entityAspect.FollowerPool.Get(saucerEntity);
                if (!follower.Target.TryUnpack(_world, out var targetEntity))
                    continue;

                var targetPos = _transformAspect.PositionPool.Get(targetEntity);
                ref var position = ref _transformAspect.PositionPool.Get(saucerEntity);
                ref var rotation = ref _transformAspect.RotationPool.Get(saucerEntity);
                ref var velocity = ref _transformAspect.VelocityPool.Get(saucerEntity);

                var dirX = targetPos.x - position.x;
                var dirY = targetPos.y - position.y;
                var dist = MathF.Sqrt(dirX * dirX + dirY * dirY);

                if (dist <= 0f)
                    continue;

                var desiredAngle = MathF.Atan2(dirY, dirX) * 180f / MathF.PI;
                var angleError = NormalizeAngleDegrees(desiredAngle - rotation.angle);

                var maxTurnStep = _configService.SaucerConfig.TurnSpeed * dt;
                rotation.angle += Math.Clamp(angleError, -maxTurnStep, maxTurnStep);

                var angleRad = rotation.angle * MathF.PI / 180f;
                var fwdX = MathF.Cos(angleRad);
                var fwdY = MathF.Sin(angleRad);

                var invDist = 1f / dist;
                var toTargetX = dirX * invDist;
                var toTargetY = dirY * invDist;

                var alignment = fwdX * toTargetX + fwdY * toTargetY;
                if (alignment < 0f)
                    alignment = 0f;

                var maxSpeed = _configService.SaucerConfig.Speed;
                var desiredForwardSpeed = maxSpeed * alignment;

                var currentForwardSpeed = velocity.x * fwdX + velocity.y * fwdY;

                var accel = _configService.SaucerConfig.Acceleration;
                var maxDeltaSpeed = accel * dt;

                var newForwardSpeed =
                    MoveTowards(currentForwardSpeed, desiredForwardSpeed, maxDeltaSpeed);

                velocity.x = fwdX * newForwardSpeed;
                velocity.y = fwdY * newForwardSpeed;
            }
        }

        private static float NormalizeAngleDegrees(float angle)
        {
            angle %= 360f;
            if (angle <= -180f) angle += 360f;
            if (angle > 180f) angle -= 360f;
            return angle;
        }

        private static float MoveTowards(float current, float target, float maxDelta)
        {
            var diff = target - current;
            if (MathF.Abs(diff) <= maxDelta)
                return target;

            return current + MathF.Sign(diff) * maxDelta;
        }
    }
}