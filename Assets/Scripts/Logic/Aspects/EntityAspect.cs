using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using AsteroidsGame.Contracts;

namespace AsteroidsGame.Logic
{
    public sealed class EntityAspect : IProtoAspect
    {
        private ProtoWorld _world;

        public ProtoWorld World()
        {
            return _world;
        }


        public ProtoPool<EntityIdComponent> EntityIdPool;
        public ProtoPool<PlayerData> PlayerPool;

        public void Init(ProtoWorld world)
        {
            _world = world;
            _world.AddAspect(this);

            EntityIdPool = new ProtoPool<EntityIdComponent>();
            PlayerPool = new ProtoPool<PlayerData>();

            _world.AddPool(EntityIdPool);
            _world.AddPool(PlayerPool);
        }

        public void PostInit()
        {
        }
    }
}