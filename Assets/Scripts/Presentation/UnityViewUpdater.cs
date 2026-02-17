namespace AsteroidsGame.Presentation
{
    using System;
    using UnityEngine;
    using Contracts;
    using System.Collections.Generic;

    public class UnityViewUpdater : MonoBehaviour, IViewUpdater
    {
        private readonly Dictionary<int, Transform> _map = new();
        private readonly HashSet<int> _seenIds = new();
        private readonly List<int> _toRemove = new();

        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _asteroidPrefab;
        [SerializeField] private GameObject _asteroidFragmentPrefab;
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private GameObject _saucerPrefab;
        [SerializeField] private GameObject _laserPrefab;

        private readonly Stack<Transform> _playerPool = new();
        private readonly Stack<Transform> _asteroidPool = new();
        private readonly Stack<Transform> _asteroidFragmentPool = new();
        private readonly Stack<Transform> _bulletPool = new();
        private readonly Stack<Transform> _saucerPool = new();
        private readonly Stack<Transform> _laserPool = new();

        private const string PlayerTag = "Player";
        private const string AsteroidTag = "Asteroid";
        private const string AsteroidFragmentTag = "AsteroidFragment";
        private const string BulletTag = "Bullet";
        private const string SaucerTag = "Saucer";
        private const string LaserTag = "Laser";

        public void Apply(IReadOnlyList<ViewData> views)
        {
            _seenIds.Clear();

            foreach (var v in views)
            {
                _seenIds.Add(v.id);

                if (!_map.TryGetValue(v.id, out var entityTransform))
                {
                    entityTransform = GetFromPool(v.type);
                    _map[v.id] = entityTransform;
                }

                entityTransform.position = new Vector2(v.x, v.y);
                entityTransform.rotation = Quaternion.Euler(0f, 0f, v.angle);
            }

            if (_map.Count > _seenIds.Count)
                CleanupRemoved();
        }

        private Transform GetFromPool(EntityType type)
        {
            var pool = type switch
            {
                EntityType.Player => _playerPool,
                EntityType.Asteroid => _asteroidPool,
                EntityType.AsteroidFragment => _asteroidFragmentPool,
                EntityType.Bullet => _bulletPool,
                EntityType.Saucer => _saucerPool,
                EntityType.Laser => _laserPool,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (pool.Count > 0)
            {
                var t = pool.Pop();
                t.gameObject.SetActive(true);
                return t;
            }

            var prefab = type switch
            {
                EntityType.Player => _playerPrefab,
                EntityType.Asteroid => _asteroidPrefab,
                EntityType.AsteroidFragment => _asteroidFragmentPrefab,
                EntityType.Bullet => _bulletPrefab,
                EntityType.Saucer => _saucerPrefab,
                EntityType.Laser => _laserPrefab,
                _ => throw new ArgumentOutOfRangeException()
            };
            return Instantiate(prefab).transform;
        }

        private void CleanupRemoved()
        {
            _toRemove.Clear();

            foreach (var key in _map.Keys)
                if (!_seenIds.Contains(key))
                    _toRemove.Add(key);

            foreach (var key in _toRemove)
            {
                var entityTransform = _map[key];
                ReturnToPool(entityTransform);
                _map.Remove(key);
            }
        }

        private void ReturnToPool(Transform entityTransform)
        {
            entityTransform.gameObject.SetActive(false);

            switch (entityTransform.tag)
            {
                case PlayerTag:
                    _playerPool.Push(entityTransform);
                    break;
                case AsteroidTag:
                    _asteroidPool.Push(entityTransform);
                    break;

                case AsteroidFragmentTag:
                    _asteroidFragmentPool.Push(entityTransform);
                    break;

                case BulletTag:
                    _bulletPool.Push(entityTransform);
                    break;
                case SaucerTag:
                    _saucerPool.Push(entityTransform);
                    break;
                case LaserTag:
                    _laserPool.Push(entityTransform);
                    break;
            }
        }
    }
}