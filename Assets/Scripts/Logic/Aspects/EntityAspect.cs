
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


        public ProtoPool<EntityIdCmp> EntityIdPool;
        public ProtoPool<PlayerCmp> PlayerPool;
        public ProtoPool<AsteroidCmp> AsteroidPool;

        public void Init(ProtoWorld world)
        {
            _world = world;
            _world.AddAspect(this);

            EntityIdPool = new ProtoPool<EntityIdCmp>();
            PlayerPool = new ProtoPool<PlayerCmp>();
            AsteroidPool = new ProtoPool<AsteroidCmp>();

            _world.AddPool(EntityIdPool);
            _world.AddPool(PlayerPool);
            _world.AddPool(AsteroidPool);
        }

        public void PostInit()
        {
        }
    }
}