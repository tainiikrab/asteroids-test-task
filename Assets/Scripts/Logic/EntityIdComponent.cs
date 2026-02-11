using Leopotam.EcsProto.QoL;
using AsteroidsGame.Contracts;

namespace AsteroidsGame.Logic
{
    public struct EntityIdComponent
    {
        public int Id;
        public ProtoPackedEntity Packed;
        public EntityType Type;
    }
}