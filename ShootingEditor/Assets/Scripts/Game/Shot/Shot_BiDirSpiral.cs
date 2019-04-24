using System.Collections;
using UnityEngine;

namespace Game
{
    public class Shot_BiDirSpiral : IShot
    {

        float angle= 0.0f;
        float angleRate1 = 0.03f;
        float angleRate2 = -0.02f;
        float speed = 0.01f;
        int count = 4;
        int interval = 20;
        int duration = 200;
        public IEnumerator Shot(Mover mover)
        {
            const int directionCount = 2;
            float[] shotAngle = { angle, angle };
            float[] shotAngleRate = { angleRate1, angleRate2 };

            for (int frame = 0; frame < duration; ++frame)
            {
                if ((frame % interval) == 0)
                {
                    for (int j = 0; j < directionCount; ++j)
                    {
                        for (int i = 0; i < count; ++i)
                        {
                            Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                            b.Init(BulletName.blue, mover._X, mover._Y, shotAngle[j] + ((float)i / count), speed);
                        }

                        shotAngle[j] += shotAngleRate[j];
                        shotAngle[j] -= Mathf.Floor(shotAngle[j]);
                    }
                }
                yield return null;
            }
        }

        public string getDescription()
        {
            // name \n detail
            return string.Format("{0}\n{1}", "", "");
        }
    }
}