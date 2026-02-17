using AsteroidsGame.Contracts;
using Leopotam.EcsProto;

namespace AsteroidsGame.Logic
{
    using System;

    public class RandomService : IRandomService
    {
        private Random Random { get; } = new();
        public float RandomSign => Random.Next(0, 2) * 2f - 1f;
        public float RandomNormalizedFloat => ((float)Random.NextDouble() - 0.5f) * 2f;
        public float NextFloat => (float)Random.NextDouble();
        public float RandomAngleDegrees => MathF.PI * 2f * NextFloat;

        private IGameViewSizeService _viewSizeService;
        private IConfigService _configService;

        public RandomService(IProtoSystems systems)
        {
            _viewSizeService = systems.Services()[typeof(IGameViewSizeService)] as IGameViewSizeService;
            _configService = systems.Services()[typeof(IConfigService)] as IConfigService;
        }

        public (float, float) CalculateRandomSpawnPosition()
        {
            var halfWidth = _viewSizeService.HalfWidth;
            var halfHeight = _viewSizeService.HalfHeight;
            var margin = _configService.WorldConfig.ScreenWrapMargin;

            var horizontal = NextFloat < 0.5;

            float x, y;

            if (horizontal)
            {
                var signX = RandomSign;
                x = signX * (halfWidth + margin);

                var t = NextFloat;
                y = -halfHeight + t * (halfHeight - -halfHeight);
            }
            else
            {
                var signY = RandomSign;
                y = signY * (halfHeight + margin);

                var t = NextFloat;
                x = -halfWidth + t * (halfWidth - -halfWidth);
            }

            return (x, y);
        }
    }

    public interface IRandomService
    {
        float RandomSign { get; }
        float RandomNormalizedFloat { get; }

        float RandomAngleDegrees { get; }

        float NextFloat { get; }

        (float, float) CalculateRandomSpawnPosition();
    }
}