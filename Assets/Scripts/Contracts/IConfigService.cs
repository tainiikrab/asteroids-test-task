namespace AsteroidsGame.Contracts
{
    public interface IConfigService
    {
        public IWorldConfig WorldConfig { get; }
        public IPlayerConfig PlayerConfig { get; }
        public IAsteroidConfig AsteroidConfig { get; }
        public IAsteroidFragmentConfig AsteroidFragmentConfig { get; }
        public IBulletConfig BulletConfig { get; }
        public ILaserConfig LaserConfig { get; }
        public ISaucerConfig SaucerConfig { get; }
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

    public interface IAsteroidFragmentConfig
    {
        public int SpawnCount { get; }
        public float SpeedMultiplier { get; }
        public float RotationSpeedMultiplier { get; }
        public float SpawnScatter { get; }
        public float ColliderRadius { get; }
    }

    public interface IWorldConfig
    {
        public float ScreenWrapMargin { get; }
    }

    public interface IBulletConfig
    {
        public float Speed { get; }
        public float ShotCooldown { get; }
        public float ColliderRadius { get; }
        public int TeleportationLimit { get; }
    }

    public interface ILaserConfig
    {
        public float ShotCooldown { get; }
        public float ColliderRadius { get; }
        public float Duration { get; }
        public int MaxLasers { get; }
    }

    public interface ISaucerConfig
    {
        public float Speed { get; }
        public float Acceleration { get; }
        public float TurnSpeed { get; }
        public float SpawnInterval { get; }
        public int SpawnAmount { get; }
        public float ColliderRadius { get; }

        public int TeleportationLimit { get; }
    }
}