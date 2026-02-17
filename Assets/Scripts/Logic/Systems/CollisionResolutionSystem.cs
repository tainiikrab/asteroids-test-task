using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;

namespace AsteroidsGame.Logic
{
    public sealed class CollisionResolutionSystem : IProtoInitSystem, IProtoRunSystem
    {
        private CollisionAspect _collisionAspect;
        private EntityAspect _entityAspect;

        private ProtoWorld _world;
        private ProtoIt _eventIterator;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();

            _collisionAspect = (CollisionAspect)_world.Aspect(typeof(CollisionAspect));
            _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));

            _eventIterator = new ProtoIt(new[] { typeof(CollisionEventCmp) });
            _eventIterator.Init(_world);
        }

        public void Run()
        {
            foreach (var eventEntity in _eventIterator)
            {
                var collisionEvent =
                    _collisionAspect.CollisionEventPool.Get(eventEntity);

                _collisionAspect.CollisionEventPool.Del(eventEntity);

                if (!collisionEvent.SensorEntity.TryUnpack(_world, out var sensor) ||
                    !collisionEvent.OtherEntity.TryUnpack(_world, out var other))
                    continue;

                if (ShouldIgnoreCollision(sensor, other, collisionEvent))
                    continue;

                if (_collisionAspect.LaserColliderPool.Has(sensor))
                {
                    if (!_entityAspect.DestroyTagPool.Has(other))
                        _entityAspect.DestroyTagPool.Add(other);

                    continue;
                }

                if (!_entityAspect.DestroyTagPool.Has(sensor))
                    _entityAspect.DestroyTagPool.Add(sensor);

                if (!_entityAspect.DestroyTagPool.Has(other))
                    _entityAspect.DestroyTagPool.Add(other);
            }
        }

        private bool ShouldIgnoreCollision(
            ProtoEntity sensor,
            ProtoEntity other,
            in CollisionEventCmp collisionEvent)
        {
            if (IsOwner(sensor, collisionEvent.OtherEntity) ||
                IsOwner(other, collisionEvent.SensorEntity))
                return true;

            if (_entityAspect.ChildPool.Has(sensor) &&
                _entityAspect.ChildPool.Has(other))
                return true;

            return false;
        }

        private bool IsOwner(ProtoEntity bulletEntity, ProtoPackedEntity target)
        {
            if (!_entityAspect.ChildPool.Has(bulletEntity))
                return false;

            ref var bullet = ref _entityAspect.ChildPool.Get(bulletEntity);
            return bullet.parent == target;
        }
    }
}