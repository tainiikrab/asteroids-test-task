

namespace AsteroidsGame.Logic
{
    using Leopotam.EcsProto;
    public sealed class TransformAspect : IProtoAspect
    {
        private ProtoWorld _world;

        public ProtoWorld World()
        {
            return _world;
        }

        public ProtoPool<PositionCmp> PositionPool;
        public ProtoPool<VelocityCmp> VelocityPool;
        public ProtoPool<RotationCmp> RotationPool;
        public ProtoPool<AngularVelocityCmp> AngularVelocityPool;
        public ProtoPool<TeleportCounterCmp> TeleportCounterPool;


        public void Init(ProtoWorld world)
        {
            _world = world;
            _world.AddAspect(this);

            PositionPool = new ProtoPool<PositionCmp>();
            VelocityPool = new ProtoPool<VelocityCmp>();
            RotationPool = new ProtoPool<RotationCmp>();
            AngularVelocityPool = new ProtoPool<AngularVelocityCmp>();
            TeleportCounterPool = new ProtoPool<TeleportCounterCmp>();

            _world.AddPool(PositionPool);
            _world.AddPool(VelocityPool);
            _world.AddPool(RotationPool);
            _world.AddPool(AngularVelocityPool);
            _world.AddPool(TeleportCounterPool);
        }

        public void PostInit()
        {
        }
    }
}