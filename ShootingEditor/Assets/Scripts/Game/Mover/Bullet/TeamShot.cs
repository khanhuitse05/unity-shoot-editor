using UnityEngine;

namespace Game
{
    public class TeamShot : Bullet
    {
        public TeamShot()
            : base()
        {
        }
        
        public override void Move()
        {
            base.Move();

            // 살아있다면 추가동작
            if (_alive)
            {
                // 적기와 충돌 체크
                Enemy enemy = IsHit(GameSystem._Instance._Enemys);
                if (enemy != null)
                {
                    _alive = false;
                    enemy.OnHit(); 
                }
            }
        }
    }
}