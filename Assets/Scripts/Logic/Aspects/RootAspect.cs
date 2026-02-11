using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using AsteroidsGame.Contracts;

namespace AsteroidsGame.Logic
{
    public sealed class RootAspect : IProtoAspect
    {
        private ProtoWorld _world;

        public EntityAspect EntityAspect { get; private set; }
        public PositionAspect PositionAspect { get; private set; }

        public ProtoWorld World()
        {
            return _world;
        }



        public void Init(ProtoWorld world)
        {
            EntityAspect = new EntityAspect();
            PositionAspect = new PositionAspect();

            EntityAspect.Init(world);
            PositionAspect.Init(world);

            EntityAspect.PostInit();
            PositionAspect.PostInit();

            world.AddAspect(this);

        }

        public void PostInit()
        {
        }
    }
}