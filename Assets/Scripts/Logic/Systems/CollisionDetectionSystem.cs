namespace AsteroidsGame.Logic
{
    using System.Collections.Generic;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;

    public sealed class CollisionDetectionSystem : IProtoInitSystem, IProtoRunSystem
    {
        private EntityAspect _entityAspect;
        private TransformAspect _transformAspect;
        private CollisionAspect _collisionAspect;

        private ProtoWorld _world;

        private ProtoIt _sensorIterator;
        private ProtoIt _colliderIterator;

        private readonly List<ProtoEntity> _collidersCache = new(128);

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();
            _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));
            _transformAspect = (TransformAspect)_world.Aspect(typeof(TransformAspect));
            _collisionAspect = (CollisionAspect)_world.Aspect(typeof(CollisionAspect));

            _sensorIterator =
                new ProtoIt(new[] { typeof(CollisionSensorCmp), typeof(CircleColliderCmp), typeof(PositionCmp) });
            _colliderIterator = new ProtoIt(new[] { typeof(CircleColliderCmp), typeof(PositionCmp) });

            _sensorIterator.Init(_world);
            _colliderIterator.Init(_world);
        }

        public void Run()
        {
            _collidersCache.Clear();
            foreach (var e in _colliderIterator) _collidersCache.Add(e);

            foreach (var sensorEntity in _sensorIterator) CheckForCollisions(sensorEntity);
        }

        private void CheckForCollisions(ProtoEntity sensorEntity)
        {
            ref var position1 = ref _transformAspect.PositionPool.Get(sensorEntity);
            ref var collider1 = ref _collisionAspect.CircleColliderPool.Get(sensorEntity);

            for (int i = 0, n = _collidersCache.Count; i < n; i++)
            {
                var other = _collidersCache[i];

                if (other.Equals(sensorEntity)) continue;

                ref var position2 = ref _transformAspect.PositionPool.Get(other);
                ref var collider2 = ref _collisionAspect.CircleColliderPool.Get(other);

                var dx = position1.x - position2.x;
                var dy = position1.y - position2.y;
                var dist2 = dx * dx + dy * dy;
                var rad = collider1.radius + collider2.radius;

                if (dist2 > rad * rad) continue;

                ref var collisionEvent =
                    ref _collisionAspect.CollisionEventPool.NewEntity(out _);
                collisionEvent.SensorEntity = _world.PackEntity(sensorEntity);
                collisionEvent.OtherEntity = _world.PackEntity(other);
            }
        }
    }
}