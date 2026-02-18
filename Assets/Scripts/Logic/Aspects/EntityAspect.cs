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
        public readonly ProtoPool<ChildCmp> ChildPool = new();
        public readonly ProtoPool<AsteroidCmp> AsteroidPool = new();
        public readonly ProtoPool<FollowerCmp> FollowerPool = new();
        public readonly ProtoPool<TimerCmp> TimerPool = new();

        public readonly ProtoPool<BulletShooterCmp> BulletShooterPool = new();
        public readonly ProtoPool<LaserShooterCmp> LaserShooterPool = new();

        public readonly ProtoPool<HealthCmp> HealthPool = new();
        public readonly ProtoPool<ScoreCmp> ScorePool = new();


        public void Init(ProtoWorld world)
        {
            _world = world;
            _world.AddAspect(this);

            _world.AddPool(EntityIdPool);
            _world.AddPool(PlayerPool);
            _world.AddPool(DestroyTagPool);
            _world.AddPool(ChildPool);
            _world.AddPool(AsteroidPool);
            _world.AddPool(FollowerPool);
            _world.AddPool(TimerPool);

            _world.AddPool(BulletShooterPool);
            _world.AddPool(LaserShooterPool);

            _world.AddPool(HealthPool);
            _world.AddPool(ScorePool);
        }

        public void PostInit()
        {
        }
    }
}