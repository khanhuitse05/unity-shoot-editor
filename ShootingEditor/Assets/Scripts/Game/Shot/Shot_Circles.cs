using System.Collections;

namespace Game
{
    public class Shot_Circles : IShot
    {
        float angle = 0.25f;
        float speed = 0.04f;
        int count = 20;
        bool halfAngleOffset = true;
        int interval = 60;
        int repeatCount = 4;
        public IEnumerator Shot(Mover mover)
        {
            for (int i = 0; i < repeatCount; ++i)
            {
                CircleBullet(mover, angle, speed, count, halfAngleOffset);
                yield return new WaitForFrames(interval);
            }
        }
        void CircleBullet(Mover mover, float angle, float speed, int count, bool halfAngleOffset)
        {
            float angleStart = angle + ((halfAngleOffset) ? (1.0f / count / 2.0f) : 0.0f);
            for (int i = 0; i < count; ++i)
            {
                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(BulletName.red, mover._X, mover._Y, angleStart + (1.0f / count * i), speed);
            }
        }
        public string getDescription()
        {
            // name \n detail
            return string.Format("{0}\n{1}", "Circles", "(repeatCount: " + repeatCount + ")");
        }
    }
}