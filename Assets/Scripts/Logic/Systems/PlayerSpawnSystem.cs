using AsteroidsGame.Contracts;
using AsteroidsGame.Logic;
using Leopotam.EcsProto;
using Leopotam.EcsProto.QoL;

public class PlayerSpawnSystem : IProtoInitSystem
{
    private ProtoWorld _world;

    private PositionAspect _positionAspect;
    private EntityAspect _entityAspect;

    private IConfigService _configService;
    private IIdGeneratorService _idGeneratorService;

    public void Init(IProtoSystems systems)
    {
        _world = systems.World();

        var svc = systems.Services();
        _configService = svc[typeof(IConfigService)] as IConfigService;
        _idGeneratorService = svc[typeof(IIdGeneratorService)] as IIdGeneratorService;
        _positionAspect = (PositionAspect)_world.Aspect(typeof(PositionAspect));
        _entityAspect = (EntityAspect)_world.Aspect(typeof(EntityAspect));

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        ref var positionData = ref _positionAspect.PositionPool.NewEntity(out var playerEntity);
        positionData.x = 0;
        positionData.y = 0;

        ref var velocityData = ref _positionAspect.VelocityPool.Add(playerEntity);
        velocityData.vx = 0;
        velocityData.vy = 0;
        velocityData.deceleration = _configService.PlayerDeceleration;

        ref var rotationData = ref _positionAspect.RotationPool.Add(playerEntity);
        rotationData.angle = 0f;

        ref var angularVelocityData = ref _positionAspect.AngularVelocityPool.Add(playerEntity);
        angularVelocityData.omega = 0f;

        ref var entityIdComponent = ref _entityAspect.EntityIdPool.Add(playerEntity);
        var packed = _world.PackEntity(playerEntity);
        entityIdComponent.id = _idGeneratorService.Next();
        entityIdComponent.packedEntity = packed;

        ref var player = ref _entityAspect.PlayerPool.Add(playerEntity);
    }
}