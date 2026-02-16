namespace AsteroidsGame.Presentation
{
    using System;
    using UnityEngine;
    using AsteroidsGame.Contracts;
    using System.Collections.Generic;
    public class UnityViewUpdater : MonoBehaviour, IViewUpdater
    {
        private readonly Dictionary<int, Transform> _map = new();
        private readonly HashSet<int> _seenIds = new();
        private readonly List<int> _toRemove = new();

        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _asteroidPrefab;
        [SerializeField] private GameObject _bulletPrefab;

        private readonly Stack<Transform> _playerPool = new();
        private readonly Stack<Transform> _asteroidPool = new();
        private readonly Stack<Transform> _bulletPool = new();
        
        private const string PlayerTag = "Player";
        private const string AsteroidTag = "Asteroid";
        private const string BulletTag = "Bullet";

        public void Apply(IReadOnlyList<ViewData> views)
        {
            _seenIds.Clear();

            foreach (var v in views)
            {
                _seenIds.Add(v.id);

                if (!_map.TryGetValue(v.id, out var transform))
                {
                    transform = GetFromPool(v.type);
                    _map[v.id] = transform;

                }

                transform.position = new Vector2(v.x, v.y);
                transform.rotation = Quaternion.Euler(0f, 0f, v.angle);
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
                EntityType.Bullet => _bulletPool,
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
                EntityType.Bullet => _bulletPrefab,
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

            if (entityTransform.CompareTag(PlayerTag))
                _playerPool.Push(entityTransform);
            else if (entityTransform.CompareTag(AsteroidTag))
                _asteroidPool.Push(entityTransform);
            else if (entityTransform.CompareTag(BulletTag))
                _bulletPool.Push(entityTransform);
        }
    }
    
}