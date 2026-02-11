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

        public ProtoPool<PositionData> PositionPool;
        public ProtoPool<VelocityData> VelocityPool;
        public ProtoPool<RotationData> RotationPool;
        public ProtoPool<AngularVelocityData> AngularVelocityPool;


        public void Init(ProtoWorld world)
        {
            _world = world;
            _world.AddAspect(this);

            PositionPool = new ProtoPool<PositionData>();
            VelocityPool = new ProtoPool<VelocityData>();
            RotationPool = new ProtoPool<RotationData>();
            AngularVelocityPool = new ProtoPool<AngularVelocityData>();

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