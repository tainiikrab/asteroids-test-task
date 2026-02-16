namespace AsteroidsGame.Logic
{
    using System.Collections.Generic;
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;

    public sealed class CollisionDetectionSystem : IProtoInitSystem, IProtoRunSystem {
        
        private EntityAspect _entityAspect;
        private TransformAspect _transformAspect;
        private CollisionAspect _collisionAspect;
        
        private ProtoWorld _world;
        
        private ProtoIt _sensorIterator;  
        private ProtoIt _colliderIterator; 
        
        private readonly List<ProtoEntity> _collidersCache = new List<ProtoEntity>(128);

        public void Init(IProtoSystems systems) {
            _world = systems.World();
            _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));
            _transformAspect = (TransformAspect)_world.Aspect(typeof(TransformAspect));
            _collisionAspect = (CollisionAspect)_world.Aspect(typeof(CollisionAspect));

            _sensorIterator   = new ProtoIt(new[] { typeof(CollisionSensorCmp), typeof(ColliderCmp), typeof(PositionCmp) });
            _colliderIterator = new ProtoIt(new[] { typeof(ColliderCmp), typeof(PositionCmp) });

            _sensorIterator.Init(_world);
            _colliderIterator.Init(_world);
        }

        public void Run() {
            _collidersCache.Clear();
            foreach (ProtoEntity e in _colliderIterator) {
                _collidersCache.Add(e);
            }
            
            foreach (ProtoEntity sensorEntity in _sensorIterator) {
                ref var position1 = ref _transformAspect.PositionPool.Get(sensorEntity);
                ref var collider1 = ref _collisionAspect.ColliderPool.Get(sensorEntity);
                
                for (int i = 0, n = _collidersCache.Count; i < n; i++) {
                    var other = _collidersCache[i];

                    if (other.Equals(sensorEntity)) continue;

                    ref var position2 = ref _transformAspect.PositionPool.Get(other);
                    ref var collider2 = ref _collisionAspect.ColliderPool.Get(other);

                    float dx = position1.x - position2.x;
                    float dy = position1.y - position2.y;
                    float dist2 = dx * dx + dy * dy;
                    float rad = collider1.radius + collider2.radius;
                    
                    if (dist2 <= rad * rad) {
                        ref var collisionEvent = ref _collisionAspect.CollisionEventPool.NewEntity(out ProtoEntity collisionEventEntity);
                        collisionEvent.SensorEntity = _world.PackEntity(sensorEntity);
                        collisionEvent.OtherEntity = _world.PackEntity(other);
                    }
                }
            }

        }
    }
}
    