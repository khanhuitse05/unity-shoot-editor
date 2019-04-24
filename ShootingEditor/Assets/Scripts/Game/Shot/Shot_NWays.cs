using System.Collections;

namespace Game
{
    public class Shot_NWays : IShot
    {
        float angleRange = 0.25f;
        float speed = 0.01f;
        int count = 6;
        int interval = 30;
        int repeatCount = 4;
        public IEnumerator Shot(Mover mover)
        {
            for (int i = 0; i < repeatCount; ++i)
            {
                float angle = GameSystem._Instance.GetRandom01();
                NWayBullet(mover, BulletName.blue, angle, angleRange, speed, count);
                yield return new WaitForFrames(interval);
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
            return string.Format("{0}\n{1}", "Gap", "");
        }
    }
}