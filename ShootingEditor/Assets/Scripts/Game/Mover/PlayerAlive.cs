using UnityEngine;

namespace Game
{
    public class PlayerAlive : Player
    {
        GunDecorator gun;
        public override void Init(string shapeSubPath, float x, float y, float angle)
        {
            base.Init(shapeSubPath, x, y, angle);
            // 0
            NormalGun _gun = new NormalGun();
            _gun.Init(0.24f, new Vector2(0.1f, 0.01f));
            _gun.SetComponent(new Guns());
            // 1
            NormalGun _gun1 = new NormalGun();
            _gun1.Init(0.25f, new Vector2(0.05f, 0.01f));
            _gun1.SetComponent(_gun);
            // 2
            NormalGun _gun2 = new NormalGun();
            _gun2.Init(0.25f, new Vector2(-0.05f, 0.01f));
            _gun2.SetComponent(_gun1);
            // 3
            NormalGun _gun3 = new NormalGun();
            _gun3.Init(0.26f, new Vector2(-0.1f, 0.01f));
            _gun3.SetComponent(_gun2);
            // finally
            gun = _gun3;
        }

        public override void Move()
        {
            // Shot
            TryShot();
            // Move
            if (GameSystem._Instance._moveInputArea.IsTouching())
            {
                MoveByTouch();
            }
            else
            {
                MoveByKey();
            }
            // Check Hit
            if (IsHit(GameSystem._Instance._Bullets) != null || IsHit(GameSystem._Instance._Enemys) != null)
            {
                _alive = false;
                GameSystem._Instance.SetGameOver();
                PlayerCrash playerCrash = GameSystem._Instance.CreatePlayer<PlayerCrash>();
                playerCrash.Init(PlayName.effect_crack, _X, _Y, _angle);
            }
        }

        private void TryShot()
        {
            gun.Operation(this);
        }

        public const float speed = 0.02f;


        private void MoveByTouch()
        {
            float moveRate = 1.2f;
            Vector2 delta = GameSystem._Instance._moveInputArea.GetDelta();

            // 이동경계
            float mx = GameSystem._Instance._MaxX - _shape._size;
            float my = GameSystem._Instance._MaxY - _shape._size;

            _X = Mathf.Clamp(_X + delta.x * moveRate, -mx, mx);
            _Y = Mathf.Clamp(_Y + delta.y * moveRate, -my, my);
        }

        private void MoveByKey()
        {
            float vx = Input.GetAxis("Horizontal");
            float vy = Input.GetAxis("Vertical");

            // 이동경계
            float mx = GameSystem._Instance._MaxX - _shape._size;
            float my = GameSystem._Instance._MaxY - _shape._size;

            // 이동하려는 위치
            float x = _X + vx * speed;
            float y = _Y + vy * speed;
            x = Mathf.Clamp(x, -mx, mx);
            y = Mathf.Clamp(y, -my, my);

            // 변위
            float dx = x - _X;
            float dy = y - _Y;

            float d = Mathf.Sqrt(dx * dx + dy * dy);
            if (d > speed)
            {
                dx *= speed / d;
                dy *= speed / d;
            }

            // 실제 이동
            _X += dx;
            _Y += dy;
        }
    }
}