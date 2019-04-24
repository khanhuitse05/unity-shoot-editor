using System.Collections;
using UnityEngine;

namespace Game
{
    public class Shot_AimingNWayLine : IShot
    {
        float speed = 0.01f;
        int interval = 15;
        int shotCount = 9;
        float angleRange = 0.1f;
        int wayCount = 3;
        public IEnumerator Shot(Mover mover)
        {
            float angle = GetPlayerAngle(mover._X, mover._Y);

            for (int i = 0; i < shotCount; ++i)
            {
                NWayBullet(mover, BulletName.red, angle, angleRange, speed, wayCount);

                if (i < shotCount - 1)
                {
                    yield return new WaitForFrames(interval);
                }
            }
        }

        void NWayBullet(Mover mover, string shape, float angle, float angleRange, float speed, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(shape, mover._X, mover._Y, angle + angleRange * ((float)i / (count - 1) - 0.5f), speed);
            }
        }

        float GetPlayerAngle(float x, float y)
        {
            // Atan2 의 결과가 라디안이므로 0~1로 변경
            Vector2 playerPos = GameSystem._Instance.player._pos;
            return Mathf.Atan2(playerPos.y - y, playerPos.x - x) / Mathf.PI / 2.0f;
        }
        public string getDescription()
        {
            // name \n detail
            return string.Format("{0}\n{1}", "Aiming NWay Line", "");
        }
    }
}