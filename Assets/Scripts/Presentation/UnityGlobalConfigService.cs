namespace AsteroidsGame.Presentation
{
    using System;
    using Contracts;
    using UnityEngine;

    [Serializable]
    public sealed class UnityGlobalConfigService : IConfigService
    {
        [SerializeField] private WorldConfig _worldConfig;
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private BulletConfig _bulletConfig;
        [SerializeField] private LaserConfig _laserConfig;
        [SerializeField] private AsteroidConfig _asteroidConfig;
        [SerializeField] private AsteroidFragmentConfig _asteroidFragmentConfig;
        [SerializeField] private SaucerConfig _saucerConfig;

        public IWorldConfig WorldConfig => _worldConfig;
        public IPlayerConfig PlayerConfig => _playerConfig;
        public ILaserConfig LaserConfig => _laserConfig;
        public IBulletConfig BulletConfig => _bulletConfig;
        public IAsteroidConfig AsteroidConfig => _asteroidConfig;
        public IAsteroidFragmentConfig AsteroidFragmentConfig => _asteroidFragmentConfig;
        public ISaucerConfig SaucerConfig => _saucerConfig;
    }

    [Serializable]
    public sealed class WorldConfig : IWorldConfig
    {
        [field: SerializeField] public float ScreenWrapMargin { get; set; } = 0.5f;
    }

    [Serializable]
    public sealed class PlayerConfig : IPlayerConfig
    {
        [field: SerializeField] public float Speed { get; set; } = 10f;
        [field: SerializeField] public float RotationSpeed { get; set; } = 240f;
        [field: SerializeField] public float Acceleration { get; set; } = 20f;
        [field: SerializeField] public float Deceleration { get; set; } = 6f;
        [field: SerializeField] public float ColliderRadius { get; set; } = 0.5f;
    }

    [Serializable]
    public sealed class BulletConfig : IBulletConfig
    {
        [field: SerializeField] public float Speed { get; set; } = 10f;
        [field: SerializeField] public float ShotCooldown { get; set; } = 0.15f;
        [field: SerializeField] public float ColliderRadius { get; set; } = 0.27f;
        [field: SerializeField] public int TeleportationLimit { get; set; } = 0;
    }

    [Serializable]
    public sealed class LaserConfig : ILaserConfig
    {
        [field: SerializeField] public float ShotCooldown { get; set; } = 3f;
        [field: SerializeField] public float ColliderRadius { get; set; } = 0.5f;
        [field: SerializeField] public float Duration { get; set; } = 2f;
        [field: SerializeField] public int MaxLasers { get; set; } = 3;
    }

    [Serializable]
    public sealed class AsteroidConfig : IAsteroidConfig
    {
        [field: SerializeField] public float Speed { get; set; } = 2;
        [field: SerializeField] public float RotationSpeed { get; set; } = 140f;
        [field: SerializeField] public float SpawnInterval { get; set; } = 3f;
        [field: SerializeField] public int SpawnAmount { get; set; } = 3;
        [field: SerializeField] public float RandomnessWeight { get; set; } = 0.8f;
        [field: SerializeField] public int TeleportationLimit { get; set; } = 4;
        [field: SerializeField] public float ColliderRadius { get; set; } = 0.5f;
    }


    [Serializable]
    public sealed class AsteroidFragmentConfig : IAsteroidFragmentConfig
    {
        [field: SerializeField] public int SpawnCount { get; set; } = 3;
        [field: SerializeField] public float SpeedMultiplier { get; set; } = 1.3f;
        [field: SerializeField] public float RotationSpeedMultiplier { get; set; } = 1.3f;
        [field: SerializeField] public float SpawnScatter { get; set; } = 0.2f;
        [field: SerializeField] public float ColliderRadius { get; set; } = 0.25f;
    }


    [Serializable]
    public sealed class SaucerConfig : ISaucerConfig
    {
        [field: SerializeField] public float Speed { get; set; } = 5;
        [field: SerializeField] public float ColliderRadius { get; set; } = 0.5f;
        [field: SerializeField] public float SpawnInterval { get; set; } = 5f;
        [field: SerializeField] public int SpawnAmount { get; set; } = 2;
        [field: SerializeField] public int TeleportationLimit { get; set; } = 3;

        [field: SerializeField] public float TurnSpeed { get; set; } = 200;
        [field: SerializeField] public float Acceleration { get; set; } = 10;
    }
}