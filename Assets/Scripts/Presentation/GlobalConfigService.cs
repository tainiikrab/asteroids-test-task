namespace AsteroidsGame.Presentation
{
    using System;
    using Contracts;
    using UnityEngine;

    [Serializable]
    public class GlobalConfigService : IConfigService
    {
        [SerializeField] private WorldConfig _worldConfig;
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private BulletConfig _bulletConfig;
        [SerializeField] private AsteroidConfig _asteroidConfig;
        [SerializeField] private AsteroidFragmentationConfig _asteroidFragmentationConfig;

        public IWorldConfig WorldConfig => _worldConfig;
        public IPlayerConfig PlayerConfig => _playerConfig;
        public IAsteroidConfig AsteroidConfig => _asteroidConfig;
        public IBulletConfig BulletConfig => _bulletConfig;
        public IAsteroidFragmentationConfig AsteroidFragmentationConfig => _asteroidFragmentationConfig;
    }

    [Serializable]
    public struct PlayerConfig : IPlayerConfig
    {
        [field: SerializeField] public float Speed { get; set; }
        [field: SerializeField] public float RotationSpeed { get; set; }
        [field: SerializeField] public float Acceleration { get; set; }
        [field: SerializeField] public float Deceleration { get; set; }
        [field: SerializeField] public float ColliderRadius { get; set; }
    }

    [Serializable]
    public struct AsteroidConfig : IAsteroidConfig
    {
        [field: SerializeField] public float Speed { get; set; }
        [field: SerializeField] public float RotationSpeed { get; set; }
        [field: SerializeField] public float SpawnInterval { get; set; }
        [field: SerializeField] public int SpawnAmount { get; set; }
        [field: SerializeField] public float RandomnessWeight { get; set; }
        [field: SerializeField] public int TeleportationLimit { get; set; }
        [field: SerializeField] public float ColliderRadius { get; set; }
    }

    [Serializable]
    public struct WorldConfig : IWorldConfig
    {
        [field: SerializeField] public float ScreenWrapMargin { get; set; }
    }

    [Serializable]
    public struct BulletConfig : IBulletConfig
    {
        [field: SerializeField] public float Speed { get; set; }
        [field: SerializeField] public float ShotInterval { get; set; }
        [field: SerializeField] public float ColliderRadius { get; set; }
        [field: SerializeField] public int TeleportationLimit { get; set; }
    }

    [Serializable]
    public struct AsteroidFragmentationConfig : IAsteroidFragmentationConfig
    {
        [field: SerializeField] public int SpawnCount { get; set; }
        [field: SerializeField] public float SpeedMultiplier { get; set; }
        [field: SerializeField] public float RotationSpeedMultiplier { get; set; }
        [field: SerializeField] public float SpawnScatter { get; set; }
        [field: SerializeField] public float ColliderRadius { get; set; }
    }
}