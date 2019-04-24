using System.Collections;
using UnityEngine;

namespace Game
{
    public class Shot_RandomSpread : IShot
    {

        float angleRange = 0.2f;
        float speed = 0.02f;
        float speedRange = 0.02f;
        int count = 24;

        public IEnumerator Shot(Mover mover)
        {
            float angle = GetPlayerAngle(mover._X, mover._Y);
            for (int i = 0; i < count; ++i)
            {
                // 탄 별로 각도와 속도를 랜덤으로 설정
                float bulletAngle = angle + angleRange * (GameSystem._Instance.GetRandom01() - 0.5f);
                float bulletSpeed = speed + speedRange * GameSystem._Instance.GetRandom01();
                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(BulletName.red, mover._X, mover._Y, bulletAngle, bulletSpeed);
            }
            yield return null;
        }

        float GetPlayerAngle(float x, float y)
        {
            Vector2 playerPos = GameSystem._Instance.player._pos;
            return Mathf.Atan2(playerPos.y - y, playerPos.x - x) / Mathf.PI / 2.0f;
        }

        public string getDescription()
        {
            // name \n detail
            return string.Format("{0}\n{1}", "Random Spread", "");
        }
    }
}