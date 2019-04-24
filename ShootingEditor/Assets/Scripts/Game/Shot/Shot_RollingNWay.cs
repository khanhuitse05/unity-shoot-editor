using System.Collections;
using UnityEngine;

namespace Game
{
    public class Shot_RollingNWay : IShot
    {

        float speed = 0.02f;
        int count = 18;
        float radius = 0.5f;
        int interval = 1;
        int repeatCount = 2;

        public IEnumerator Shot(Mover mover)
        {
            float angle = GetPlayerAngle(mover._X, mover._Y) + 0.25f;    // 시작 각도는 플레이어 방향과 직각
            for (int repeat = 0; repeat < repeatCount; ++repeat)
            {
                for (int i = 0; i < count; ++i)
                {
                    // 발사 위치
                    float spawnAngleRadian = (2.0f * Mathf.PI) * (angle - (1.0f / count * i));  // 반시계방향
                    float spawnX = mover._X + radius * Mathf.Cos(spawnAngleRadian);
                    float spawnY = mover._Y + radius * Mathf.Sin(spawnAngleRadian);
                    // 발사위치로부터 플레이어 방향
                    float bulletAngle = GetPlayerAngle(spawnX, spawnY);

                    Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                    b.Init(BulletName.blue, spawnX, spawnY, bulletAngle, speed);

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