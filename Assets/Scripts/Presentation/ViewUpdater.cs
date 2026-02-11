using UnityEngine;
using AsteroidsGame.Contracts;
using System.Collections.Generic;

namespace AsteroidsGame.Presentation
{
    public class ViewUpdater : MonoBehaviour
    {
        private Dictionary<int, GameObject> _map = new();

        public void Apply(ViewData[] views)
        {
            foreach (var v in views)
            {
                if (!_map.TryGetValue(v.id, out var go))
                {
                    go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    _map[v.id] = go;
                }

                go.transform.position = new Vector3(v.x, v.y);
                go.transform.rotation = Quaternion.Euler(0f, 0f, v.angle);
            }
        }
    }
}