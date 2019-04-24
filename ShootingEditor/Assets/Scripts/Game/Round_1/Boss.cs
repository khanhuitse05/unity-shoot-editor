using UnityEngine;
using System.Collections;

namespace Game
{
    namespace Round_01
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
                Log("Fist\nMove (30s)");
                StartCoroutine(_Logic.MoveConstantVelocity(this, new Vector2(0.0f, 0.75f), 30));
                yield return new WaitForFrames(100);
                Log("Pattern_01");
                StartCoroutine(_Logic.Pattern_01(this));
                yield return new WaitForFrames(500);
                Log("Pattern_02");
                StartCoroutine(_Logic.Pattern_02(this));
                yield return new WaitForFrames(500);
                Log("Pattern_03");
                StartCoroutine(_Logic.Pattern_03(this, true));
                yield return new WaitForFrames(1000);
                Log("Pattern_04");
                StartCoroutine(_Logic.Pattern_04(this));
                yield return new WaitForFrames(500);
                Log("Pattern_05");
                StartCoroutine(_Logic.Pattern_05(this));
                yield return new WaitForFrames(500);
                Log("Pattern_06");
                StartCoroutine(_Logic.Pattern_06(this));
                yield return new WaitForFrames(1500);
                Log("Pattern_07");
                StartCoroutine(_Logic.Pattern_07(this));
                yield return new WaitForFrames(1500);
                Log("Pattern_08");
                StartCoroutine(_Logic.Pattern_08(this));
                yield return new WaitForFrames(1500);

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
    } //
}