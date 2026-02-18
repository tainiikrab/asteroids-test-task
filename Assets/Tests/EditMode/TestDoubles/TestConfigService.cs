namespace AsteroidsGame.Tests.EditMode
{
    using AsteroidsGame.Contracts;

    internal sealed class TestConfigService : IConfigService
    {
        public IWorldConfig WorldConfig { get; } = new TestWorldConfig();
        public IPlayerConfig PlayerConfig { get; } = new TestPlayerConfig();
        public IAsteroidConfig AsteroidConfig { get; } = new TestAsteroidConfig();
        public IAsteroidFragmentConfig AsteroidFragmentConfig { get; } = new TestAsteroidFragmentConfig();
        public IBulletConfig BulletConfig { get; } = new TestBulletConfig();
        public ILaserConfig LaserConfig { get; } = new TestLaserConfig();
        public ISaucerConfig SaucerConfig { get; } = new TestSaucerConfig();
    }

    internal sealed class TestWorldConfig : IWorldConfig
    {
        public float ScreenWrapMargin { get; set; } = 0f;
    }

    internal sealed class TestPlayerConfig : IPlayerConfig
    {
        public float Speed { get; set; } = 10f;
        public float RotationSpeed { get; set; } = 180f;
        public float Acceleration { get; set; } = 5f;
        public float Deceleration { get; set; } = 1f;
        public float ColliderRadius { get; set; } = 0.5f;
    }

    internal sealed class TestAsteroidConfig : IAsteroidConfig
    {
        public float Speed { get; set; } = 2f;
        public float RotationSpeed { get; set; } = 45f;
        public float SpawnInterval { get; set; } = 1f;
        public int SpawnAmount { get; set; } = 1;
        public float RandomnessWeight { get; set; } = 0.5f;
        public int TeleportationLimit { get; set; } = 2;
        public float ColliderRadius { get; set; } = 1f;
    }

    internal sealed class TestAsteroidFragmentConfig : IAsteroidFragmentConfig
    {
        public int SpawnCount { get; set; } = 2;
        public float SpeedMultiplier { get; set; } = 1.2f;
        public float RotationSpeedMultiplier { get; set; } = 1.2f;
        public float SpawnScatter { get; set; } = 0.5f;
        public float ColliderRadius { get; set; } = 0.5f;
    }

    internal sealed class TestBulletConfig : IBulletConfig
    {
        public float Speed { get; set; } = 10f;
        public float ShotCooldown { get; set; } = 0.2f;
        public float ColliderRadius { get; set; } = 0.15f;
        public int TeleportationLimit { get; set; } = 1;
    }

    internal sealed class TestLaserConfig : ILaserConfig
    {
        public float ShotCooldown { get; set; } = 2f;
        public float ColliderRadius { get; set; } = 0.2f;
        public float Duration { get; set; } = 0.7f;
        public int MaxLasers { get; set; } = 3;
    }

    internal sealed class TestSaucerConfig : ISaucerConfig
    {
        public float Speed { get; set; } = 8f;
        public float Acceleration { get; set; } = 4f;
        public float TurnSpeed { get; set; } = 90f;
        public float SpawnInterval { get; set; } = 5f;
        public int SpawnAmount { get; set; } = 1;
        public float ColliderRadius { get; set; } = 0.8f;
        public int TeleportationLimit { get; set; } = 2;
    }
}
