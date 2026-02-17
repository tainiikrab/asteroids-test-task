using Leopotam.EcsProto.QoL;

namespace AsteroidsGame.Logic
{
    using Leopotam.EcsProto;

    public sealed class ChildFollowSystem : IProtoInitSystem, IProtoRunSystem
    {
        private TransformAspect _transformAspect;
        private EntityAspect _entityAspect;
        private ProtoWorld _world;
        private ProtoIt _iterator;


        public void Init(IProtoSystems systems)
        {
            _world = systems.World();

            _transformAspect = (TransformAspect)_world.Aspect(typeof(TransformAspect));
            _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));
            _iterator = new ProtoIt(new[] { typeof(RotationCmp), typeof(PositionCmp), typeof(ChildCmp) });
            _iterator.Init(_world);
        }

        public void Run()
        {
            foreach (var entity in _iterator)
            {
                ref var rotation = ref _transformAspect.RotationPool.Get(entity);
                ref var position = ref _transformAspect.PositionPool.Get(entity);

                var child = _entityAspect.ChildPool.Get(entity);
                if (!child.followsParent) continue;

                var packedParent = child.parent;
                if (!packedParent.TryUnpack(_world, out var parentEntity) ||
                    _entityAspect.DestroyTagPool.Has(parentEntity))
                {
                    var _ = _entityAspect.DestroyTagPool.Add(entity);
                    continue;
                }

                rotation.angle = _transformAspect.RotationPool.Get(parentEntity).angle;
                position.x = _transformAspect.PositionPool.Get(parentEntity).x;
                position.y = _transformAspect.PositionPool.Get(parentEntity).y;
            }
        }
    }
}