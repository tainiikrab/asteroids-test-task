using Leopotam.EcsProto;

namespace AsteroidsGame.Logic
{
    public class IdAssignSystem : IProtoInitSystem, IProtoRunSystem
    {
        private ProtoWorld _world;
        private IProtoAspect _entityAspect;
        private ProtoIt _iterator;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();
            _entityAspect = _world.Aspect(typeof(EntityAspect));
            _iterator = new ProtoIt(new[] { typeof(EntityAspect) });
        }

        public void Run()
        {
        }
    }
}