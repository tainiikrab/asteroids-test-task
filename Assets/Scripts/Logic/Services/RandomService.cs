namespace AsteroidsGame.Logic
{
    using System;

    public class RandomService : IRandomService
    {
        private Random Random { get; } = new();
        public float RandomSign => Random.Next(0, 2) * 2f - 1f;
        public float RandomNormalizedFloat => ((float)Random.NextDouble() - 0.5f) * 2f;
        public float NextFloat => (float)Random.NextDouble();
    }

    public interface IRandomService
    {
        float RandomSign { get; }
        float RandomNormalizedFloat { get; }

        float NextFloat { get; }
    }
}