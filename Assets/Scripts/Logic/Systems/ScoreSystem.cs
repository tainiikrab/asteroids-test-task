using AsteroidsGame.Contracts;
using Leopotam.EcsProto;

namespace AsteroidsGame.Logic
{
    public class ScoreSystem : IProtoInitSystem, IProtoRunSystem
    {
        private ProtoWorld _world;
        private EntityAspect _entityAspect;
        private ProtoIt _iterator;

        private IScoreConfig _scoreConfig;
        private IScoreService _scoreService;

        public void Init(IProtoSystems systems)
        {
            _world = systems.World();
            _entityAspect = _world.Aspect(typeof(EntityAspect)) as EntityAspect;
            var configService = systems.Services()[typeof(IConfigService)] as IConfigService;
            _scoreService = systems.Services()[typeof(IScoreService)] as IScoreService;

            _scoreConfig = configService?.ScoreConfig;

            _iterator = new ProtoIt(new[] { typeof(DestroyTagCmp), typeof(EntityIdCmp) });
            _iterator.Init(_world);
        }

        public void Run()
        {
            foreach (var entity in _iterator)
            {
                ref var idCmp = ref _entityAspect.EntityIdPool.Get(entity);
                var score = idCmp.type switch
                {
                    EntityType.Asteroid => _scoreConfig.AsteroidScore,
                    EntityType.AsteroidFragment => _scoreConfig.FragmentScore,
                    EntityType.Saucer => _scoreConfig.SaucerScore,
                    _ => 0
                };
                _scoreService.AddScore(score);
            }
        }
    }
}