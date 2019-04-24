using System.Collections;
using UnityEngine;

namespace Game
{
    public class Shot_Spirals : IShot
    {

        float angle = 0.125f;
        float angleRate = 0.005f;
        float speed = 0.05f;
        int count = 4;
        int interval = 2;
        int duration = 410;


        public IEnumerator Shot(Mover mover)
        {
            float shotAngle = angle;
            for (int frame = 0; frame < duration; ++frame)
            {
                if ((frame % interval) == 0)
                {
                    // 지정된 발사 수 만큼 발사
                    for (int i = 0; i < count; ++i)
                    {
                        Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                        b.Init(BulletName.red, mover._X, mover._Y, shotAngle + ((float)i / count), speed);
                    }

                    shotAngle += angleRate;
                    shotAngle -= Mathf.Floor(shotAngle);
                }
                yield return null;
            }
        }

        void SpiralBullet(Mover mover, string shape, float angle, float angleRate, float speed, int interval, int duration)
        {
            for (int frame = 0; frame < duration; ++frame)
            {
                if ((frame % interval) == 0)
                {
                    Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                    b.Init(shape, mover._X, mover._Y, angle, speed);

                    angle += angleRate;
                    angle -= Mathf.Floor(angle);
                }
            }
        }
        public string getDescription()
        {
            // name \n detail
            return string.Format("{0}\n{1}", "", "");
        }
    }
}