using System.Collections;

namespace Game
{
    public class Shot_NWay : IShot
    {
        float angle = 0.6f;
        float angleRange = 0.25f;
        float speed = 0.02f;
        int count = 6;
        public IEnumerator Shot(Mover mover)
        {
            for (int i = 0; i < count; ++i)
            {
                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(BulletName.blue, mover._X, mover._Y,
                       angle + angleRange * ((float)i / (count - 1) - 0.5f), speed);
            }
            yield return null;
        }

        public string getDescription()
        {
            // name \n detail
            return string.Format("{0}\n{1}", "NWay", "");
        }
    }
}