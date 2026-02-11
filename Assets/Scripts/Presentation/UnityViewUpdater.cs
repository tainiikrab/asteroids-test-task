using UnityEngine;
using AsteroidsGame.Contracts;
using System.Collections.Generic;

namespace AsteroidsGame.Presentation
{
    public class UnityViewUpdater : MonoBehaviour, IViewUpdater
    {
        private Dictionary<int, GameObject> _map = new();

        public void Apply(IReadOnlyList<ViewData> views)
        {
            var seenIds = new HashSet<int>();

            foreach (var v in views)
            {
                seenIds.Add(v.id);

                if (!_map.TryGetValue(v.id, out var go))
                {
                    go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    _map[v.id] = go;
                }

                go.transform.position = new Vector3(v.x, v.y);
                go.transform.rotation = Quaternion.Euler(0f, 0f, v.angle);
            }

            if (_map.Count > seenIds.Count)
            {
                var toRemove = new List<int>();
                foreach (var kvp in _map)
                    if (!seenIds.Contains(kvp.Key))
                        toRemove.Add(kvp.Key);

                foreach (var id in toRemove)
                {
                    if (_map.TryGetValue(id, out var go)) Destroy(go);

                    _map.Remove(id);
                }
            }
        }
    }

    public interface IViewUpdater
    {
        void Apply(IReadOnlyList<ViewData> views);
    }
}