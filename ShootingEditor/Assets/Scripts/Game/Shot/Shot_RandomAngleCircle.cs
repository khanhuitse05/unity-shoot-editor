using System.Collections;

namespace Game
{
    public class Shot_RandomAngleCircle : IShot
    {
        float speed = 0.02f;
        int count = 12;
        int interval = 30;
        int repeatCount = 5;
        public IEnumerator Shot(Mover mover)
        {
            for (int i = 0; i < repeatCount; ++i)
            {
                CircleBullet(mover, BulletName.red, GameSystem._Instance.GetRandom01(), speed, count, true);
                yield return new WaitForFrames(interval);
            }
        }
        void CircleBullet(Mover mover, string shape, float angle, float speed, int count, bool halfAngleOffset)
        {
            float angleStart = angle + ((halfAngleOffset) ? (1.0f / count / 2.0f) : 0.0f);
            for (int i = 0; i < count; ++i)
            {
                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(shape, mover._X, mover._Y, angleStart + (1.0f / count * i), speed);
            }
        }
        public string getDescription()
        {
            // name \n detail
            return string.Format("{0}\n{1}", "Random Angle Circle", "");
        }
    }
}