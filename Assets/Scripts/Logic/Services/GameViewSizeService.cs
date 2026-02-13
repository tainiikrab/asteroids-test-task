namespace AsteroidsGame.Logic
{
    public class GameViewSizeService : IGameViewSizeControllerService
    {
        public float Width { get; private set; }
        public float Height { get; private set; }
        public float HalfWidth => Width * 0.5f;
        public float HalfHeight => Height * 0.5f;

        public void SetSize(float width, float height)
        {
            Width = width;
            Height = height;
        }
    }
    
    public interface IGameViewSizeControllerService : IGameViewSizeService
    {
        void SetSize(float width, float height);
    }
    public interface IGameViewSizeService
    {
        float Width { get; }
        float Height { get; }
        float HalfWidth { get; }
        float HalfHeight { get; }
    }
}