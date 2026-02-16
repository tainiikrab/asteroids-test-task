namespace AsteroidsGame.Logic
{
    using Leopotam.EcsProto;
    using Leopotam.EcsProto.QoL;

    public sealed class DestroyByTagSystem : IProtoInitSystem, IProtoRunSystem
    {
        private ProtoWorld _world;
        private ProtoIt _destroyIterator;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();

            _destroyIterator = new ProtoIt(new[] { typeof(DestroyTagCmp) });
            _destroyIterator.Init(_world);
        }

        public void Run()
        {
            foreach (var eventEntity in _destroyIterator) _world.DelEntity(eventEntity);
        }
    }
}