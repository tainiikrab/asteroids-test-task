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

        public readonly ProtoPool<PositionCmp> PositionPool = new();
        public readonly ProtoPool<VelocityCmp> VelocityPool = new();
        public readonly ProtoPool<RotationCmp> RotationPool = new();
        public readonly ProtoPool<AngularVelocityCmp> AngularVelocityPool = new();
        public readonly ProtoPool<TeleportCounterCmp> TeleportCounterPool = new();


        public void Init(ProtoWorld world)
        {
            _world = world;
            _world.AddAspect(this);

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