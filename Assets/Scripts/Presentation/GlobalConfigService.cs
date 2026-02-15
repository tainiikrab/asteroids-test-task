

namespace AsteroidsGame.Presentation
{
    using System;
    using AsteroidsGame.Contracts;
    using UnityEngine;
    
    [Serializable]
    public class GlobalConfigService : IConfigService
    {
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private AsteroidConfig _asteroidConfig;
        [SerializeField] private WorldConfig _worldConfig;

        public IPlayerConfig PlayerConfig => _playerConfig;
        public IAsteroidConfig AsteroidConfig => _asteroidConfig;
        public IWorldConfig WorldConfig => _worldConfig;
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
    }
    [Serializable]
    public struct WorldConfig : IWorldConfig
    {
        [field: SerializeField] public float ScreenWrapMargin { get; set; }
    }
}