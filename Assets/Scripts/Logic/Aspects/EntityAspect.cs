namespace AsteroidsGame.Logic
{
    using Leopotam.EcsProto;

    public sealed class EntityAspect : IProtoAspect
    {
        private ProtoWorld _world;

        public ProtoWorld World()
        {
            return _world;
        }


        public readonly ProtoPool<EntityIdCmp> EntityIdPool = new();
        public readonly ProtoPool<PlayerCmp> PlayerPool = new();
        public readonly ProtoPool<DestroyTagCmp> DestroyTagPool = new();
        public readonly ProtoPool<BulletCmp> BulletPool = new();
        public readonly ProtoPool<AsteroidCmp> AsteroidPool = new();


        public void Init(ProtoWorld world)
        {
            _world = world;
            _world.AddAspect(this);

            _world.AddPool(EntityIdPool);
            _world.AddPool(PlayerPool);
            _world.AddPool(DestroyTagPool);
            _world.AddPool(BulletPool);
            _world.AddPool(AsteroidPool);
        }

        public void PostInit()
        {
        }
    }
}