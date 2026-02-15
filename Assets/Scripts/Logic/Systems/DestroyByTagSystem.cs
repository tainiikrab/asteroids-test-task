// Assets/Scripts/Logic/CollisionResolutionSystem.cs
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;

namespace AsteroidsGame.Logic
{
    public sealed class DestroyByTagSystem : IProtoInitSystem, IProtoRunSystem {
        
        ProtoWorld _world;
        ProtoIt _destroyIterator;

        public void Init(IProtoSystems systems) {
            _world = systems.World();
            
            _destroyIterator = new ProtoIt(new[] { typeof(DestroyTagCmp) });
            _destroyIterator.Init(_world);
        }

        public void Run() {
            foreach (ProtoEntity eventEntity in _destroyIterator)
            {
                _world.DelEntity(eventEntity);
            }
        }
    }
}