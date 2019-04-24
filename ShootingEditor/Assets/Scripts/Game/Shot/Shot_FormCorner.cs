using System.Collections;
using UnityEngine;

namespace Game
{
    public class Shot_FormCorner : IShot
    {
        float speed = 0.02f;
        int interval = 30;
        int roundCount = 2;
        public IEnumerator Shot(Mover mover)
        {
            for (int i = 0; i < roundCount; ++i)
            {
                // 0: 우상, 1: 좌상, 2: 좌하, 3: 우하
                for (int dir = 0; dir < 4; ++dir)
                {
                    float x, y;
                    if (dir == 0)
                    {
                        x = GameSystem._Instance._MaxX;
                        y = GameSystem._Instance._MaxY;
                    }
                    else if (dir == 1)
                    {
                        x = GameSystem._Instance._MinX;
                        y = GameSystem._Instance._MaxY;
                    }
                    else if (dir == 2)
                    {
                        x = GameSystem._Instance._MinX;
                        y = GameSystem._Instance._MinY;
                    }
                    else
                    {
                        x = GameSystem._Instance._MaxX;
                        y = GameSystem._Instance._MinY;
                    }

                    float angle = GetPlayerAngle(x, y);
                    Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                    b.Init(BulletName.red, x, y, angle, speed);

                    if (!((i == (roundCount - 1) && (dir == (4 - 1)))))
                    {
                        yield return new WaitForFrames(interval);
                    }
                }
            }
        }
        public float GetPlayerAngle(float x, float y)
        {
            Vector2 playerPos = GameSystem._Instance.player._pos;
            return Mathf.Atan2(playerPos.y - y, playerPos.x - x) / Mathf.PI / 2.0f;
        }

        public string getDescription()
        {
            // name \n detail
            return string.Format("{0}\n{1}", "Shot Form Corner", "(Round:" + roundCount+ ")");
        }
    }
}