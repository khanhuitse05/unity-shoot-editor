using UnityEngine;
using System.Collections;

namespace Game
{
    namespace Round_00
    {
        // 보스
        public class Boss : Enemy
        {
            private const int _score = 10;
            public Boss() : base()
            {
            }

            public override void Init(string shapeSubPath, float x, float y, float angle)
            {
                base.Init(shapeSubPath, x, y, angle);
                CoroutineManager.instance.RegisterCoroutine(MoveMain());
                CoroutineManager.instance.ResetFrame();
            }

            public override void Move()
            {
                CoroutineManager.instance.UpdateAllCoroutines();
            }
            void Log(string message)
            {
                GameSystem.SetNote(message);
            }
            // 메인 코루틴
            private IEnumerator MoveMain()
            {
                #region Base
                Log("Fist\nMove (30s)");
                StartCoroutine(_Logic.MoveConstantVelocity(this, new Vector2(0.0f, 0.75f), 30));
                yield return new WaitForFrames(100);

                Log("Shot form TOP & BOTTOM\n(30s, 2 times)");
                StartCoroutine(_Logic.ShotFormBorder(0, 0.02f, 30, 2));
                StartCoroutine(_Logic.ShotFormBorder(1, 0.02f, 30, 2));
                yield return new WaitForFrames(150);

                Log("Shot form LEFT & RIGHT\n(30s, 2 times)");
                StartCoroutine(_Logic.ShotFormBorder(3, 0.02f, 30, 2));
                StartCoroutine(_Logic.ShotFormBorder(4, 0.02f, 30, 2));
                yield return new WaitForFrames(150);

                Log("Shot form CORNER\n(30s, 2 times)");
                StartCoroutine(_Logic.ShotFormCorner(0.02f, 30, 2));
                yield return new WaitForFrames(150);

                Log("AimingLineBullets\n (5 intevel, 5 times)");
                StartCoroutine(_Logic.AimingLineBullets(this, BulletName.blue, 0.01f, 5, 5));
                yield return new WaitForFrames(200);

                Log("AimingNWayLineBullets\n (5 intevel, 3 times)");
                StartCoroutine(_Logic.AimingNWayLineBullets(this, BulletName.red, 0.01f, 5, 9, 0.125f, 3));
                yield return new WaitForFrames(200);

                Log("RollingNWayBullets\n (5 intevel, 3 times)");
                StartCoroutine(_Logic.RollingNWayBullets(this, BulletName.blue, -0.15f, 0.02f, 0.02f, 0.01f, 5, 1, 5, 9));
                yield return new WaitForFrames(200);

                Log("RandomSpreadBullet\ncount 24 ");
                _Logic.RandomSpreadBullet(this, BulletName.red, 0.2f, 0.02f, 0.02f, 24);
                yield return new WaitForFrames(200);

                Log("AimimgBullet\nspeed 2 ");
                StartCoroutine(_Logic.RollingAimingBullets(this, BulletName.red, 0.02f, 18, 0.5f, 1, 2));
                yield return new WaitForFrames(200);

                Log("Circle Bullet");
                _Logic.CircleBullet(this, BulletName.blue, 0.25f, 0.01f, 20, true);
                yield return new WaitForFrames(120);

                Log("Circle Bullet Repeat\n(60s, 4 times)");
                StartCoroutine(_Logic.CircleBullets(this, BulletName.red, 0.25f, 0.01f, 20, true, 60, 4));
                yield return new WaitForFrames(400);

                Log("Placed Circle Bullet");
                _Logic.DropCircleBullet(this, BulletName.blue, 0.75f, 0.04f, 10, false, 12, 24, 0.75f, 0.02f);
                yield return new WaitForFrames(120);

                Log("Random Circle Bullets\n(30s, 4 times)");
                StartCoroutine(_Logic.RandomCircleBullets(this, BulletName.red, 0.02f, 30, 30, 60));
                yield return new WaitForFrames(300);

                Log("Random Angle Circle Bullets\n(30s, 4 times)");
                StartCoroutine(_Logic.RandomAngleCircleBullets(this, BulletName.blue, 0.01f, 20, 60, 4));
                yield return new WaitForFrames(300);

                Log("NWayBullet\n2 times");
                _Logic.NWayBullet(this, BulletName.red, 0.75f, 0.125f, 0.01f, 6);
                yield return new WaitForFrames(60);
                _Logic.NWayBullet(this, BulletName.red, 0.75f, 0.125f, 0.01f, 6);
                yield return new WaitForFrames(150);

                Log("GapBullets\n(30s, 4 times)");
                StartCoroutine(_Logic.RandomNWayBullets(this, BulletName.red, 0.95f, 0.005f, 20, 30, 4));
                yield return new WaitForFrames(200);

                Log("CustomGapBullets\n120s");
                StartCoroutine(_Logic.MoveDamp(this, Vector2.zero, 120, 0.05f));
                yield return new WaitForFrames(120);
                StartCoroutine(_Logic.RandomNWayBullets(this, BulletName.blue, 0.25f, 0.005f, 10, 60, 5));
                yield return new WaitForFrames(4 * 120);
                //  back
                StartCoroutine(_Logic.MoveConstantVelocity(this, new Vector2(0.0f, 0.75f), 60));
                yield return new WaitForFrames(60);

                Log("SpiralBullets\n200s");
                StartCoroutine(_Logic.SpiralBullets(this, BulletName.red, 0.0f, 0.02f, 0.01f, 4, 5, 193));
                yield return new WaitForFrames(300);

                Log("BiDirectionalSpiralBullets\n200s");
                StartCoroutine(_Logic.BiDirectionalSpiralBullets(this, BulletName.blue, 0.0f, 0.03f, -0.02f, 0.01f, 4, 5, 200));
                yield return new WaitForFrames(300);

                Log("AimimgBullet\nspeed 2 ");
                _Logic.AimimgBullet(this, BulletName.red, 2);
                yield return new WaitForFrames(200);
                #endregion Base

                yield return new WaitForFrames(120);
                _Logic.CircleBullet(this, BulletName.red, 0.25f, 0.025f, 20, false);
                {
                    Effect crashEffect = GameSystem._Instance.CreateEffect<Effect>();
                    crashEffect.Init(BossName.effect_crack_blue, _X, _Y, 0.0f);
                }
                yield return new WaitForFrames(60);
                GameSystem._Instance.SetGameWin();
                _alive = false;
            }

            // 피격시
            public override void OnHit()
            {
                GameSystem._Instance.SetScoreDelta(_score);
            }

            public override void OnDestroy()
            {
                base.OnDestroy();
                CoroutineManager.instance.StopAllCoroutines();
            }

            private GameLogic _Logic
            {
                get { return GameSystem._Instance.GetLogic<GameLogic>(); }
            }

            CoroutineNode StartCoroutine(IEnumerator fiber)
            {
                return CoroutineManager.instance.StartCoroutine(fiber);
            }
        } // Boss
    } // IevanPolkka
} // Game