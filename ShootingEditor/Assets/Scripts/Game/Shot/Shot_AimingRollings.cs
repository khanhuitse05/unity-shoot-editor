using System.Collections;
using UnityEngine;

namespace Game
{
    public class Shot_AimingRollings : IShot
    {
        float angle = -0.15f;
        float angleRange = 0.05f;
        float angleRate = 0.05f;
        float speed = 0.01f;
        int count = 4;
        int groupCount = 1;
        int interval = 20;
        int repeatCount = 9;

        public IEnumerator Shot(Mover mover)
        {
            for (int repeat = 0; repeat < repeatCount; ++repeat)
            {
                for (int group = 0; group < groupCount; ++group)
                {
                    float nwayAngle = angle + (float)group / groupCount;
                    NWayBullet(mover, BulletName.blue, nwayAngle, angleRange, speed, count);
                }

                angle += angleRate;
                angle -= Mathf.Floor(angle);

                if (repeat < repeatCount - 1)
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

        public string getDescription()
        {
            // name \n detail
            return string.Format("{0}\n{1}", "", "");
        }
    }
}