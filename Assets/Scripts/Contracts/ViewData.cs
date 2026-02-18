namespace AsteroidsGame.Contracts
{
    using System.Collections.Generic;

    public interface IGameView
    {
        void RenderGame(IReadOnlyList<ViewData> views);

        void Clear();
    }

    public struct ViewData
    {
        public int id;
        public float x, y;
        public float angle;
        public EntityType type;
    }
}