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


        public ProtoPool<ColliderCmp> ColliderPool;
        public ProtoPool<CollisionSensorCmp> CollisionSensorPool;
        public ProtoPool<CollisionEventCmp> CollisionEventPool;

        public void Init(ProtoWorld world)
        {
            _world = world;
            _world.AddAspect(this);

            ColliderPool = new ProtoPool<ColliderCmp>();
            CollisionSensorPool = new ProtoPool<CollisionSensorCmp>();
            CollisionEventPool = new ProtoPool<CollisionEventCmp>();
            
            _world.AddPool(ColliderPool);
            _world.AddPool(CollisionSensorPool);
            _world.AddPool(CollisionEventPool);
        }

        public void PostInit()
        {
        }
    }
}