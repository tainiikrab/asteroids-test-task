namespace AsteroidsGame.Contracts
{

    public interface IConfigService
    {
        public IPlayerConfig PlayerConfig { get; }
        public IAsteroidConfig AsteroidConfig { get; }
        public IWorldConfig WorldConfig { get; }
        public IBulletConfig BulletConfig { get; }
    }

    public interface IPlayerConfig
    {
        public float Speed { get; }
        public float RotationSpeed { get; }
        public float Acceleration { get; }
        public float Deceleration { get; }
        public float ColliderRadius { get; }
        
    }
    public interface IAsteroidConfig
    {
        public float Speed { get; }
        public float RotationSpeed { get; }
        public float SpawnInterval { get; }
        public int SpawnAmount { get; }
        public float RandomnessWeight { get; }
        public int TeleportationLimit { get; }
        public float ColliderRadius { get; }
    }
    public interface IWorldConfig
    {
        public float ScreenWrapMargin { get; }
    }

    public interface IBulletConfig
    {
        public float Speed { get; }
        public float ShotInterval { get; }
        public float ColliderRadius { get; }
    }
}