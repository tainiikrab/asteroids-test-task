namespace AsteroidsGame.Contracts
{
    using System.Collections.Generic;
    public interface IViewUpdater
    {
        void Apply(IReadOnlyList<ViewData> views);
    }
    public struct ViewData
    {
        public int id;
        public float x, y;
        public float angle;
        public EntityType type;
    }
}