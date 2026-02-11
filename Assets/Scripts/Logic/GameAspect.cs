using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;
using AsteroidsGame.Contracts;

namespace AsteroidsGame.Logic
{
    public sealed class GameAspect : IProtoAspect
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
        public ProtoPool<EntityIdComponent> EntityIdPool;

        public void Init(ProtoWorld world)
        {
            world.AddAspect(this);

            PositionPool = new ProtoPool<PositionData>();
            VelocityPool = new ProtoPool<VelocityData>();
            RotationPool = new ProtoPool<RotationData>();
            AngularVelocityPool = new ProtoPool<AngularVelocityData>();
            EntityIdPool = new ProtoPool<EntityIdComponent>();

            world.AddPool(PositionPool);
            world.AddPool(VelocityPool);
            world.AddPool(RotationPool);
            world.AddPool(AngularVelocityPool);
            world.AddPool(EntityIdPool);
        }

        public void PostInit()
        {
        }
    }
}