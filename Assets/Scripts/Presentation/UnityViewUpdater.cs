using System;
using UnityEngine;
using AsteroidsGame.Contracts;
using System.Collections.Generic;

namespace AsteroidsGame.Presentation
{
    public class UnityViewUpdater : MonoBehaviour, IViewUpdater
    {
        private readonly Dictionary<int, Transform> _map = new();
        private readonly HashSet<int> _seenIds = new();
        private readonly List<int> _toRemove = new();

        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _asteroidPrefab;

        private readonly Stack<Transform> _playerPool = new();
        private readonly Stack<Transform> _asteroidPool = new();

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

                    Debug.Log($"Instantiated {v.type} with id {v.id}");
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
                _ => throw new ArgumentOutOfRangeException()
            };

            if (pool.Count > 0)
            {
                var t = pool.Pop();
                t.gameObject.SetActive(true);
                return t;
            }

            var prefab = type == EntityType.Player ? _playerPrefab : _asteroidPrefab;
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

            if (entityTransform.CompareTag("Player"))
                _playerPool.Push(entityTransform);
            else
                _asteroidPool.Push(entityTransform);
        }
    }


    public interface IViewUpdater
    {
        void Apply(IReadOnlyList<ViewData> views);
    }
}