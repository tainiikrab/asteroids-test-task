

namespace AsteroidsGame.Logic
{
    using Leopotam.EcsProto;
    public sealed class RootAspect : IProtoAspect
    {
        private ProtoWorld _world;

        public EntityAspect EntityAspect { get; private set; }
        public TransformAspect TransformAspect { get; private set; }
        public CollisionAspect CollisionAspect { get; private set; }

        public ProtoWorld World()
        {
            return _world;
        }



        public void Init(ProtoWorld world)
        {
            EntityAspect = new EntityAspect();
            TransformAspect = new TransformAspect();
            CollisionAspect = new CollisionAspect();

            EntityAspect.Init(world);
            TransformAspect.Init(world);
            CollisionAspect.Init(world);

            EntityAspect.PostInit();
            TransformAspect.PostInit();
            CollisionAspect.PostInit();

            world.AddAspect(this);

        }

        public void PostInit()
        {
        }
    }
}