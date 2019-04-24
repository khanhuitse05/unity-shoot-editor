using System.Collections;

namespace Game
{
    public class Shot_RandomCircles : IShot
    {
        float speed = 0.02f;
        int count = 30;
        int interval = 30;
        int duration = 60;
        public IEnumerator Shot(Mover mover)
        {
            for (int frame = 0; frame < duration; ++frame)
            {
                if ((frame % interval) == 0)
                {
                    for (int i = 0; i < count; ++i)
                    {
                        Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                        b.Init(BulletName.blue, mover._X, mover._Y, GameSystem._Instance.GetRandom01(), speed);
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