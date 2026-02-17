using Leopotam.EcsProto.QoL;

namespace AsteroidsGame.Logic
{
    using Leopotam.EcsProto;

    public sealed class RotationSystem : IProtoInitSystem, IProtoRunSystem
    {
        private TransformAspect _transformAspect;
        private EntityAspect _entityAspect;
        private ProtoWorld _world;
        private ProtoIt _iterator;


        private IDeltaTimeService _deltaTimeService;
        private float DeltaTime => _deltaTimeService.DeltaTime;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();

            var svc = systems.Services();
            _deltaTimeService = svc[typeof(IDeltaTimeService)] as IDeltaTimeService;
            _transformAspect = (TransformAspect)_world.Aspect(typeof(TransformAspect));
            _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));
            _iterator = new ProtoIt(new[] { typeof(RotationCmp), typeof(AngularVelocityCmp) });
            _iterator.Init(_world);
        }

        public void Run()
        {
            foreach (var entity in _iterator)
            {
                ref var rotation = ref _transformAspect.RotationPool.Get(entity);

                if (_entityAspect.ChildPool.Has(entity))
                {
                    var child = _entityAspect.ChildPool.Get(entity);
                    if (child.followsParent)
                    {
                        var parent = child.parent;
                        if (parent.TryUnpack(_world, out var parentEntity))
                        {
                            rotation.angle = _transformAspect.RotationPool.Get(parentEntity).angle;
                            continue;
                        }
                    }
                }

                ref var angularVelocity = ref _transformAspect.AngularVelocityPool.Get(entity);


                rotation.angle -= angularVelocity.omega * DeltaTime;
            }
        }
    }
}