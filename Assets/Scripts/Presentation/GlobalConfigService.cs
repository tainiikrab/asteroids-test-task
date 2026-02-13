

namespace AsteroidsGame.Presentation
{
    using System;
    using AsteroidsGame.Contracts;
    using UnityEngine;
    
    [Serializable]
    public class GlobalConfigService : IConfigService
    {
        [SerializeField] private float _playerSpeed = 6f;

        [SerializeField] private float _playerRotationSpeed = 180f;
        [SerializeField] private float _playerAcceleration = 10f;
        [SerializeField] private float _playerDeceleration = 1f;

        [SerializeField] private float _asteroidSpeed = 10f;
        [SerializeField] private float _asteroidRotationSpeed = 10f;
        [SerializeField] private float _asteroidSpawnInterval = 10f;
        [SerializeField] private int _asteroidSpawnAmount = 10;
        [SerializeField] private float _asteroidRandomnessWeight = 0.3f;
        
        [SerializeField] private float _screenWrapPadding = 1f;
        
        [SerializeField] private int _asteroidTeleportationLimit = 3;

        public float PlayerSpeed => _playerSpeed;
        public float PlayerRotationSpeed => _playerRotationSpeed;
        public float PlayerAcceleration => _playerAcceleration;
        public float PlayerDeceleration => _playerDeceleration;

        public float AsteroidSpeed => _asteroidSpeed;
        public float AsteroidRotationSpeed => _asteroidRotationSpeed;
        public float AsteroidSpawnInterval => _asteroidSpawnInterval;
        public int AsteroidSpawnAmount => _asteroidSpawnAmount;
        public float AsteroidRandomnessWeight => _asteroidRandomnessWeight;
        
        public float ScreenWrapMargin => _screenWrapPadding;
        
        public int AsteroidTeleportationLimit => _asteroidTeleportationLimit;
    }
}