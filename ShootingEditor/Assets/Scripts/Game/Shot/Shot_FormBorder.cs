using System.Collections;
using UnityEngine;

namespace Game
{
    public class Shot_FormBorder : IShot
    {
        float speed = 0.02f;
        int interval = 30;
        int count = 8;
        public IEnumerator Shot(Mover mover)
        {
            for (int i = 0; i < count; i++)
            {
                int dir = Random.Range(0, 4);
                OneShot(dir);
                if (i < count - 1)
                {
                    yield return new WaitForFrames(interval);
                }
            }
        }

        /// <param name="dir">Direction. 0:Up, 1:Down, 2:Left, 3:Right</param>
        public void OneShot(int dir)
        {
            float x, y, angle;
            if (dir == 0)
            {
                x = GameSystem._Instance.player._X;
                y = GameSystem._Instance._MaxY;
                angle = 0.75f;
            }
            else if (dir == 1)
            {
                x = GameSystem._Instance.player._X;
                y = GameSystem._Instance._MinY;
                angle = 0.25f;
            }
            else if (dir == 2)
            {
                x = GameSystem._Instance._MinX;
                y = GameSystem._Instance.player._Y;
                angle = 0.0f;
            }
            else
            {
                x = GameSystem._Instance._MaxX;
                y = GameSystem._Instance.player._Y;
                angle = 0.50f;
            }

            Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.blue, x, y, angle, speed);
        }
        public string getDescription()
        {
            // name \n detail
            return string.Format("{0}\n{1}", "Shot Form Border", "(count:" + count + ")");
        }
    }
}