using UnityEngine;

namespace Game
{
    public class Bullet : Mover
    {
        public float _speed;

        public Bullet()
            : base()
        {
        }

        public void Init(string shapeSubPath, float x, float y, float angle, float speed)
        {
            base.Init(shapeSubPath, x, y, angle);
            _speed = speed;
        }
        
        public override void Move()
        {
            float rad = _angle * Mathf.PI * 2.0f;
            
            _X += _speed * Mathf.Cos(rad);
            _Y += _speed * Mathf.Sin(rad);

            if (!IsInStage())
            {
                _alive = false;
            }
        }
    }
}