namespace AsteroidsGame.Logic
{
    using Leopotam.EcsProto;

    public sealed class RootAspect : IProtoAspect
    {
        private ProtoWorld _world;

        public readonly EntityAspect EntityAspect = new();
        public readonly TransformAspect TransformAspect = new();
        public readonly CollisionAspect CollisionAspect = new();

        public ProtoWorld World()
        {
            return _world;
        }


        public void Init(ProtoWorld world)
        {
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