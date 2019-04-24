using System.Collections;
using UnityEngine;

namespace Game
{
    public class Shot_AimingLines : IShot
    {
        float speed = 0.02f;
        int interval = 15;
        int repeatCount = 5;

        public IEnumerator Shot(Mover mover)
        {
            float angle = GetPlayerAngle(mover._X, mover._Y);

            for (int i = 0; i < repeatCount; ++i)
            {
                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(BulletName.blue, mover._X, mover._Y, angle, speed);

                if (i < repeatCount - 1)
                {
                    yield return new WaitForFrames(interval);
                }
            }
        }

        float GetPlayerAngle(float x, float y)
        {
            Vector2 playerPos = GameSystem._Instance.player._pos;
            return Mathf.Atan2(playerPos.y - y, playerPos.x - x) / Mathf.PI / 2.0f;
        }

        public string getDescription()
        {
            // name \n detail
            return string.Format("{0}\n{1}", "", "");
        }
    }
}