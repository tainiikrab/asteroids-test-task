using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using AsteroidsGame.Contracts;

namespace AsteroidsGame.Logic
{
    public sealed class PositionAspect : IProtoAspect
    {
        private ProtoWorld _world;

        public ProtoWorld World()
        {
            return _world;
        }

        public ProtoPool<PositionCmp> PositionPool;
        public ProtoPool<VelocityCmp> VelocityPool;
        public ProtoPool<RotationCmp> RotationPool;
        public ProtoPool<AngularVelocityCmp> AngularVelocityPool;


        public void Init(ProtoWorld world)
        {
            _world = world;
            _world.AddAspect(this);

            PositionPool = new ProtoPool<PositionCmp>();
            VelocityPool = new ProtoPool<VelocityCmp>();
            RotationPool = new ProtoPool<RotationCmp>();
            AngularVelocityPool = new ProtoPool<AngularVelocityCmp>();

            _world.AddPool(PositionPool);
            _world.AddPool(VelocityPool);
            _world.AddPool(RotationPool);
            _world.AddPool(AngularVelocityPool);
        }

        public void PostInit()
        {
        }
    }
}