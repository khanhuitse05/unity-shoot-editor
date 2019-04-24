using System.Collections;
using UnityEngine;

namespace Game
{
    public class Shot_Aiming : IShot
    {
        float speed = 0.02f;
        public IEnumerator Shot(Mover mover)
        {
            Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.red, mover._X, mover._Y, GetPlayerAngle(mover._X, mover._Y), speed);
            yield return null;
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
            return string.Format("{0}\n{1}", "Aiming", "");
        }
    }
}