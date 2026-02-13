using AsteroidsGame.Contracts;
using Leopotam.EcsProto.QoL;

namespace AsteroidsGame.Logic
{
    public struct PositionCmp
    {
        public float x, y;
    }

    public struct VelocityCmp
    {
        public float vx, vy;
        public float deceleration;
    }

    public struct RotationCmp
    {
        public float angle;
    }

    public struct AngularVelocityCmp
    {
        public float omega;
    }

    public struct PlayerCmp
    {
    }

    public struct AsteroidCmp
    {
    }

    public struct EntityIdCmp
    {
        public int id;
        public ProtoPackedEntity packedEntity;
        public EntityType type;
    }

    public struct NeedsIdCmp
    {
    }
}