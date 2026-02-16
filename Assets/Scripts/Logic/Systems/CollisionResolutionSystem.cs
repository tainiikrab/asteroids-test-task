// Assets/Scripts/Logic/CollisionResolutionSystem.cs
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;

namespace AsteroidsGame.Logic
{
    public sealed class CollisionResolutionSystem : IProtoInitSystem, IProtoRunSystem {


        CollisionAspect _collisionAspect;
        EntityAspect _entityAspect;

        ProtoWorld _world;
        ProtoIt _eventIterator;

        public void Init(IProtoSystems systems) {
            _world = systems.World();

            _collisionAspect = (CollisionAspect)_world.Aspect(typeof(CollisionAspect));
            _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));

            _eventIterator = new ProtoIt(new[] { typeof(CollisionEventCmp) });
            _eventIterator.Init(_world);
        }

        public void Run()
        {
            foreach (ProtoEntity eventEntity in _eventIterator)
            {
                ref var collisionEvent =
                    ref _collisionAspect.CollisionEventPool.Get(eventEntity);

                bool sensorOk = collisionEvent.SensorEntity.TryUnpack(_world, out ProtoEntity sensor);
                bool otherOk  = collisionEvent.OtherEntity.TryUnpack(_world, out ProtoEntity other);

                if (sensorOk && otherOk &&
                    (IsOwner(sensor, collisionEvent.OtherEntity) || IsOwner(other, collisionEvent.SensorEntity)))
                {
                    _collisionAspect.CollisionEventPool.Del(eventEntity);
                    continue;
                }

                if (sensorOk && !_entityAspect.DestroyTagPool.Has(sensor))
                    _entityAspect.DestroyTagPool.Add(sensor);

                if (otherOk && !_entityAspect.DestroyTagPool.Has(other))
                    _entityAspect.DestroyTagPool.Add(other);

                _collisionAspect.CollisionEventPool.Del(eventEntity);
            }
        }
        bool IsOwner(ProtoEntity bulletEntity, ProtoPackedEntity target)
        {
            if (!_entityAspect.BulletPool.Has(bulletEntity))
                return false;

            ref var bullet = ref _entityAspect.BulletPool.Get(bulletEntity);
            return bullet.owner == target;
        }
    }
}