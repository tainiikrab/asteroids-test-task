namespace AsteroidsGame.Logic
{
    using Leopotam.EcsProto;

    public sealed class CollisionAspect : IProtoAspect
    {
        private ProtoWorld _world;

        public ProtoWorld World()
        {
            return _world;
        }

        public readonly ProtoPool<CircleColliderCmp> CircleColliderPool = new();
        public readonly ProtoPool<CollisionSensorCmp> CollisionSensorPool = new();
        public readonly ProtoPool<CollisionEventCmp> CollisionEventPool = new();
        public readonly ProtoPool<LaserColliderCmp> LaserColliderPool = new();

        public void Init(ProtoWorld world)
        {
            _world = world;
            _world.AddAspect(this);

            _world.AddPool(CircleColliderPool);
            _world.AddPool(CollisionSensorPool);
            _world.AddPool(CollisionEventPool);
            _world.AddPool(LaserColliderPool);
        }

        public void PostInit()
        {
        }
    }
}