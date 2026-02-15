
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
        public ProtoPool<DestroyTagCmp> DestroyTagPool;
      

        public void Init(ProtoWorld world)
        {
            _world = world;
            _world.AddAspect(this);

            EntityIdPool = new ProtoPool<EntityIdCmp>();
            PlayerPool = new ProtoPool<PlayerCmp>();
            DestroyTagPool = new ProtoPool<DestroyTagCmp>();
            
            _world.AddPool(EntityIdPool);
            _world.AddPool(PlayerPool);
            _world.AddPool(DestroyTagPool);
    
        }

        public void PostInit()
        {
        }
    }
}