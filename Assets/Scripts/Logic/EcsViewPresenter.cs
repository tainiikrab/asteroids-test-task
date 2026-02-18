namespace AsteroidsGame.Logic
{
    using System.Collections.Generic;
    using AsteroidsGame.Contracts;
    using Leopotam.EcsProto;

    public class EcsGamePresenter : IGamePresenter
    {
        private readonly IGameView _gameView;

        private readonly ProtoIt _viewIterator;
        private readonly List<ViewData> _viewsBuffer = new();

        private readonly ProtoPool<EntityIdCmp> _entityIdPool;
        private readonly ProtoPool<PositionCmp> _positionPool;
        private readonly ProtoPool<RotationCmp> _rotationPool;

        public EcsGamePresenter(ProtoWorld world, IGameView gameView)
        {
            _gameView = gameView;
            var entityAspect = world.Aspect(typeof(EntityAspect)) as EntityAspect;
            var positionAspect = world.Aspect(typeof(TransformAspect)) as TransformAspect;

            _entityIdPool = entityAspect?.EntityIdPool;
            _positionPool = positionAspect?.PositionPool;
            _rotationPool = positionAspect?.RotationPool;

            _viewIterator = new ProtoIt(new[]
                { typeof(EntityIdCmp), typeof(PositionCmp), typeof(RotationCmp) });
            _viewIterator.Init(world);
        }

        public void UpdateGame()
        {
            _viewsBuffer.Clear();

            foreach (var e in _viewIterator)
            {
                ref var idComp = ref _entityIdPool.Get(e);
                ref var p = ref _positionPool.Get(e);
                ref var rot = ref _rotationPool.Get(e);
                _viewsBuffer.Add(new ViewData
                {
                    id = idComp.id,
                    x = p.x,
                    y = p.y,
                    angle = rot.angle,
                    type = idComp.type
                });
            }

            _gameView.RenderGame(_viewsBuffer);
        }
    }

    public interface IGamePresenter
    {
        void UpdateGame();
    }
}