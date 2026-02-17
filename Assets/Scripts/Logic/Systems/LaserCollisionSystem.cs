namespace AsteroidsGame.Logic
{
    using System;
    using System.Collections.Generic;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;

    public sealed class LaserCollisionSystem : IProtoInitSystem, IProtoRunSystem
    {
        private TransformAspect _transformAspect;
        private CollisionAspect _collisionAspect;

        private ProtoWorld _world;

        private ProtoIt _laserIterator;
        private ProtoIt _colliderIterator;

        private readonly List<ProtoEntity> _circleColliders = new(128);

        private const float DegToRad = MathF.PI / 180f;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();
            _transformAspect = (TransformAspect)_world.Aspect(typeof(TransformAspect));
            _collisionAspect = (CollisionAspect)_world.Aspect(typeof(CollisionAspect));

            _laserIterator = new ProtoIt(new[]
            {
                typeof(CollisionSensorCmp), typeof(LaserColliderCmp), typeof(PositionCmp), typeof(RotationCmp)
            });

            _colliderIterator = new ProtoIt(new[]
            {
                typeof(CircleColliderCmp), typeof(PositionCmp)
            });

            _laserIterator.Init(_world);
            _colliderIterator.Init(_world);
        }

        public void Run()
        {
            if (_laserIterator.IsEmptySlow()) return;

            _circleColliders.Clear();
            foreach (var entity in _colliderIterator)
                _circleColliders.Add(entity);

            foreach (var laser in _laserIterator)
                ProcessLaser(laser);
        }

        private void ProcessLaser(ProtoEntity laser)
        {
            ref var laserPosition = ref _transformAspect.PositionPool.Get(laser);
            ref var laserRotation = ref _transformAspect.RotationPool.Get(laser);
            ref var laserCollider = ref _collisionAspect.LaserColliderPool.Get(laser);

            var originX = laserPosition.x;
            var originY = laserPosition.y;

            var angleRad = laserRotation.angle * DegToRad;
            var directionX = MathF.Cos(angleRad);
            var directionY = MathF.Sin(angleRad);

            var halfWidth = laserCollider.radius;

            foreach (var target in _circleColliders)
            {
                if (target == laser) continue;

                if (!IntersectsLaser(target, originX, originY, directionX, directionY, halfWidth)) continue;

                ref var collision = ref _collisionAspect.CollisionEventPool.NewEntity(out _);

                collision.SensorEntity = _world.PackEntity(laser);
                collision.OtherEntity = _world.PackEntity(target);
            }
        }

        private bool IntersectsLaser(ProtoEntity target, float originX, float originY, float dirX, float dirY,
            float laserHalfWidth)
        {
            ref var targetPosition = ref _transformAspect.PositionPool.Get(target);
            ref var targetCollider = ref _collisionAspect.CircleColliderPool.Get(target);

            var toTargetX = targetPosition.x - originX;
            var toTargetY = targetPosition.y - originY;

            var projection = toTargetX * dirX + toTargetY * dirY;

            var distanceSquared = toTargetX * toTargetX + toTargetY * toTargetY;

            var combinedRadius = laserHalfWidth + targetCollider.radius;
            var combinedRadiusSquared = combinedRadius * combinedRadius;

            if (projection < -combinedRadius)
                return false;

            if (projection < 0f)
                return distanceSquared <= combinedRadiusSquared;

            var perpendicularSquared = distanceSquared - projection * projection;
            return perpendicularSquared <= combinedRadiusSquared;
        }
    }
}