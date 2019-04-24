
namespace Game
{
    public abstract class Player : Mover
    {
        public override void Init(string shapeSubPath, float x, float y, float angle)
        {
            base.Init(shapeSubPath, x, y, angle);
            GameSystem._Instance.SetPlayer(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GameSystem._Instance.SetPlayer(null);
        }
    }
}