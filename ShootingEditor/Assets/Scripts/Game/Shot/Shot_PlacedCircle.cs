using System.Collections;

namespace Game
{
    public class Shot_PlacedCircle : IShot
    {
        float angle= 0.75f;
        float speed = 0.04f;
        int count = 10;
        bool halfAngleOffset = false;
        int moveDuration = 12;
        int stopDuration = 24;
        float angle2 = 0.75f;
        float speed2 = 0.02f;

        public IEnumerator Shot(Mover mover)
        {
            float angleStart = angle + ((halfAngleOffset) ? (1.0f / count / 2.0f) : 0.0f);
            for (int i = 0; i < count; ++i)
            {
                PlacedBullet b = GameSystem._Instance.CreateBullet<PlacedBullet>();
                b.Init(BulletName.blue, mover._X, mover._Y, angleStart + (1.0f / count * i), speed, moveDuration, stopDuration, angle2, speed2);
            }
            yield return null;
        }

        public string getDescription()
        {
            // name \n detail
            return string.Format("{0}\n{1}", "Placed Circle", "");
        }
    }
}