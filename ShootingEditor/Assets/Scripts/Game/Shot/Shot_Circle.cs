using System.Collections;

namespace Game
{
    public class Shot_Circle : IShot
    {
        float angle = 0.0f;
        float speed = 0.02f;
        int count = 12;
        bool halfAngleOffset = true;
        public IEnumerator Shot(Mover mover)
        {
            float angleStart = angle + ((halfAngleOffset) ? (1.0f / count / 2.0f) : 0.0f);
            for (int i = 0; i < count; ++i)
            {
                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(BulletName.blue, mover._X, mover._Y, angleStart + (1.0f / count * i), speed);
            }
            yield return null;
        }

        public string getDescription()
        {
            // name \n detail
            return string.Format("{0}\n{1}", "Circle",
                                 "(angle: " + angle + ", count: " + count + ", half: " + halfAngleOffset + ")");
        }
    }
}